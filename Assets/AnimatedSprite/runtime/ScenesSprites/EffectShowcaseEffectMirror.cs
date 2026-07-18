using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FlexiSprite
{
    [Serializable]
    public class CarouselAnimationsAndSpritesSetup
    {
        public bool enableAutoAnim = true;
        public List<CarouselAnimationsAndSpritesSetupFieldPair> fieldsToAnim = new();
    }

    [Serializable]
    public class CarouselAnimationsAndSpritesSetupFieldPair
    {
        public EffectId effectId;
        public List<string> fieldNames = new List<string>();
    }

    [DefaultExecutionOrder(-10)]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class EffectShowcaseEffectMirror : MonoBehaviour
    {
        [Header("Source (Dummy)")]
        [SerializeField] private AnimatedSpriteShader sourceDummy;

        [Header("Target (This card's sprite)")]
        [SerializeField] private AnimatedSpriteShader target;

        [Header("Effect")]
        [SerializeField] private EffectId effectId;
        [SerializeField] private bool disable = false;

        [Header("What to mirror")]
        [SerializeField] private bool mirrorGlobals = true;
        [SerializeField] private bool mirrorMasksAndPackedMasks = true;
        [SerializeField] private bool mirrorStencil = true;
        [SerializeField] private bool mirrorOutlineGlobals = true;
        [SerializeField] private bool mirrorSpriteRendererSprite = true;

        [Header("Enabling")]
        [SerializeField] private bool forceOnlyThisEffectEnabled = true;
        [SerializeField] private bool runInEditModeLocal = true;

        [Header("Animation Setup")]
        [SerializeField] private CarouselAnimationsAndSpritesSetup globalSetup;

        private static readonly Dictionary<string, FieldInfo> _fiCache = new(512);

        private static readonly string[] GlobalFields =
        {
            "globalEffectFade",
            "edgeCleanup",
            "effectMaskEnabled",
            "effectMaskTexture",
            "pixelArtSnapEnabled",
            "pixelSnapResolution",
            "timeScale",
            "timeFPS",
            "timeMode",
            "useCustomTime",
            "customTime",
            "outlineEffectsOnly",
            "outlineEffectWidth",
            "outlineFixedWidthPx",
            "premulAlpha",
            "spriteFlipX",
            "spriteFlipY",
            "dissolveNoiseSpeed",
            "lutSize",
            "castShadows",
            "receiveShadows",
            "shadowAlphaClip",
            "trailVelocitySource",
            "smearVelocitySource",
            "velocityDecay"
        };

        private static readonly string[] MaskFields =
        {
            "masksEnabled",
            "maskR",
            "maskG",
            "maskB",
            "maskA",
            "packedMasks"
        };

        private static readonly string[] StencilFields =
        {
            "stencilEnabled",
            "stencilRef",
            "stencilComp",
            "stencilPass",
            "stencilReadMask",
            "stencilWriteMask"
        };

        private static readonly string[] OutlineGlobalFields =
        {
            "outlineMode",
            "outlineRings",
            "outlineAA",
            "outlineSDFTex",
            "outlineSDFSoftness",
            "outlineSDFAlphaThreshold",
            "outlineSDFSpread",
            "chromaMode",
            "chromaRadialCenter"
        };

        private static readonly string[] GradientFields =
        {
            "colorRampGradient",
            "colorRampBakedGradient",
            "colorRampTexture",
            "gradientGradient",
            "gradientBakedTexture",
            "radialGradGradient",
            "radialGradBakedTexture"
        };

        private readonly Dictionary<EffectId, HashSet<string>> _effectToFieldsLookup = new();
        private readonly List<FieldInfo> _fieldsToCopy = new();

        private EffectId _lastEffectId;
        private EffectDef _def;

        private ShowcaseAnimClip[] _animClips;
        private FieldInfo[] _animFieldInfos;
        private ParamType[] _animParamTypes;
        private float _animTime;

        private SpriteRenderer _srcSR;
        private SpriteRenderer _dstSR;
        private Sprite _customSprite;
        private bool _hasCustomSprite;

        public CarouselAnimationsAndSpritesSetup GlobalSetup
        {
            get => globalSetup;
            set
            {
                globalSetup = value;
                NotifySetupChanged();
            }
        }

        public void NotifySetupChanged()
        {
            InitializeLookup();
            RefreshLogic();
        }

        private void InitializeLookup()
        {
            _effectToFieldsLookup.Clear();
            if (globalSetup?.fieldsToAnim == null) return;

            foreach (var pair in globalSetup.fieldsToAnim)
            {
                if (pair == null) continue;

                if (!_effectToFieldsLookup.TryGetValue(pair.effectId, out var set))
                {
                    set = new HashSet<string>();
                    _effectToFieldsLookup.Add(pair.effectId, set);
                }

                foreach (var name in pair.fieldNames)
                {
                    if (!string.IsNullOrEmpty(name))
                        set.Add(name);
                }
            }
        }

        private void RebuildFieldList()
        {
            _fieldsToCopy.Clear();
            _def = EffectRegistry.FindById(effectId);
            if (_def == null) return;

            if (_def.hasMask && !string.IsNullOrEmpty(_def.maskField))
            {
                var fi = GetFI(_def.maskField);
                if (fi != null)
                    _fieldsToCopy.Add(fi);
            }

            if (_def.parms == null) return;

            foreach (var p in _def.parms)
            {
                var fi = GetFI(p.field);
                if (fi != null)
                    _fieldsToCopy.Add(fi);
            }
        }

        private void RebuildAnimClips()
        {
            _animClips = null;
            _animFieldInfos = null;
            _animParamTypes = null;
            _animTime = 0f;

            if (globalSetup == null || !globalSetup.enableAutoAnim) return;

            var clips = EffectShowcaseAutoAnim.GetClips(effectId);
            if (clips == null || clips.Length == 0) return;

            _effectToFieldsLookup.TryGetValue(effectId, out var allowedFields);
            if (allowedFields == null || allowedFields.Count == 0) return;

            var paramLookup = new Dictionary<string, ParamType>();
            if (_def?.parms != null)
            {
                foreach (var p in _def.parms)
                    paramLookup[p.field] = p.type;
            }

            var valid = clips
                .Where(c => allowedFields.Contains(c.field))
                .Select(c => (
                    clip: c,
                    fi: GetFI(c.field),
                    type: paramLookup.TryGetValue(c.field, out var pt) ? pt : ParamType.Float))
                .Where(x => x.fi != null)
                .ToList();

            if (valid.Count == 0) return;

            _animClips = valid.Select(x => x.clip).ToArray();
            _animFieldInfos = valid.Select(x => x.fi).ToArray();
            _animParamTypes = valid.Select(x => x.type).ToArray();
        }

        private void OnEnable()
        {
            if (disable) return;

            InitializeLookup();
            FullRefresh();
        }

        private void OnValidate()
        {
            if (disable) return;

            InitializeLookup();
            FullRefresh();
        }

        private void LateUpdate()
        {
            if (disable) return;
            if (_customSprite != null) _dstSR.sprite = _customSprite;
            if (!Application.isPlaying && !runInEditModeLocal) return;
            if (!sourceDummy || !target || ReferenceEquals(sourceDummy, target)) return;

            if (_lastEffectId != effectId)
            {
                _lastEffectId = effectId;
                RefreshLogic();
                return;
            }

            if (globalSetup != null && globalSetup.enableAutoAnim && Application.isPlaying)
                ApplyAnimation();
        }

        private void FullRefresh()
        {
            if (disable) return;

            if (!target)
                target = GetComponent<AnimatedSpriteShader>();

            _srcSR = sourceDummy ? sourceDummy.GetComponent<SpriteRenderer>() : null;
            _dstSR = target.GetComponent<SpriteRenderer>();

            if (target)
                target.EnsureInstancedMode();

            _lastEffectId = effectId;
            RefreshLogic();
        }

        public void SetSource(AnimatedSpriteShader dummy)
        {
            if (disable) return;

            sourceDummy = dummy;
            _srcSR = sourceDummy ? sourceDummy.GetComponent<SpriteRenderer>() : null;

            RebuildFieldList();
            ApplyEnableStateIfNeeded();
            MirrorAll(force: true);
        }

        public void SetCustomSprite(Sprite sprite)
        {
            if (disable) return;

            _customSprite = sprite;
            _hasCustomSprite = sprite != null;
        }

        public void ClearCustomSprite()
        {
            if (disable) return;

            _customSprite = null;
            _hasCustomSprite = false;
        }

        private void RefreshLogic()
        {
            if (disable) return;

            RebuildFieldList();
            RebuildAnimClips();
            ApplyEnableStateIfNeeded();
            MirrorAll(force: true);
        }

        private FieldInfo GetFI(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (_fiCache.TryGetValue(name, out var fi)) return fi;

            fi = typeof(AnimatedSpriteShader).GetField(
                name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            _fiCache[name] = fi;
            return fi;
        }

        private void MirrorAll(bool force)
        {
            if (disable) return;

            bool globalChanged = false;
            bool effectChanged = false;

            MirrorSpriteRenderer(force);

            if (mirrorGlobals) globalChanged |= CopyFields(GlobalFields, force);
            if (mirrorMasksAndPackedMasks) globalChanged |= CopyFields(MaskFields, force);
            if (mirrorStencil) CopyFields(StencilFields, force);
            if (mirrorOutlineGlobals) globalChanged |= CopyFields(OutlineGlobalFields, force);

            effectChanged |= CopyFields(GradientFields, force);

            if (_def != null)
            {
                foreach (var fi in _fieldsToCopy)
                    effectChanged |= CopyField(fi, force);
            }

            if (globalChanged) target.MarkGlobalsDirty();
            if (effectChanged) target.MarkEffectDirty(effectId);
            if (globalChanged || effectChanged) target.IncrementalPush();
        }

        private void MirrorSpriteRenderer(bool force)
        {
            if (!mirrorSpriteRendererSprite || _dstSR == null) return;

            if (_hasCustomSprite && _customSprite != null)
            {
                if (force || _dstSR.sprite != _customSprite)
                    _dstSR.sprite = _customSprite;

                return;
            }

            if (_srcSR == null) return;

            if (force || _dstSR.sprite != _srcSR.sprite)
                _dstSR.sprite = _srcSR.sprite;

            if (force || _dstSR.color != _srcSR.color)
                _dstSR.color = _srcSR.color;
        }

        private void ApplyAnimation()
        {
            if (_animClips == null || _animClips.Length == 0) return;

            _animTime += Time.deltaTime;

            for (int i = 0; i < _animClips.Length; i++)
            {
                float val = _animClips[i].Evaluate(_animTime);
                SetTypedValue(_animFieldInfos[i], _animParamTypes[i], val);
            }

            target.MarkEffectDirty(effectId);
        }

        private void SetTypedValue(FieldInfo fi, ParamType type, float val)
        {
            switch (type)
            {
                case ParamType.Int:
                    fi.SetValue(target, (int)val);
                    break;

                default:
                    fi.SetValue(target, val);
                    break;
            }
        }

        private bool CopyFields(string[] names, bool force)
        {
            bool changed = false;

            foreach (var n in names)
            {
                var fi = GetFI(n);
                if (fi != null)
                    changed |= CopyField(fi, force);
            }

            return changed;
        }

        private bool CopyField(FieldInfo fi, bool force)
        {
            if (!sourceDummy || !target) return false;

            object src = fi.GetValue(sourceDummy);
            object dst = fi.GetValue(target);
            if (!force && Equals(src, dst)) return false;

            fi.SetValue(target, src);
            return true;
        }

        private void ApplyEnableStateIfNeeded()
        {
            if (!forceOnlyThisEffectEnabled || !target) return;

            target.DeactivateAllEffects();
            target.SetEffectEnabled(effectId, true);
            target.ForcePush();
        }

        public void SetEffect(EffectId id)
        {
            effectId = id;
            _animTime = 0f;
            _hasCustomSprite = false;
            RefreshLogic();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CarouselAnimationsAndSpritesSetupFieldPair))]
    public class CarouselSetupDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var listProp = property.FindPropertyRelative("fieldNames");
            float listH = listProp.arraySize == 0 ? 22 : listProp.arraySize * 22;
            return listH + 45;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var idProp = property.FindPropertyRelative("effectId");
            var listProp = property.FindPropertyRelative("fieldNames");

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18), idProp);

            EffectId currentId = (EffectId)idProp.enumValueIndex;
            string[] availableFields = GetFieldsForEffect(currentId);
            float yOffset = position.y + 22;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                var elementRect = new Rect(position.x + 15, yOffset, position.width - 45, 18);
                var removeRect = new Rect(position.x + position.width - 25, yOffset, 20, 18);
                var element = listProp.GetArrayElementAtIndex(i);

                int currentIndex = Array.IndexOf(availableFields, element.stringValue);
                int newIndex = EditorGUI.Popup(elementRect, currentIndex < 0 ? 0 : currentIndex, availableFields);
                if (newIndex >= 0 && newIndex < availableFields.Length)
                    element.stringValue = availableFields[newIndex];

                if (GUI.Button(removeRect, "x"))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                yOffset += 22;
            }

            if (GUI.Button(new Rect(position.x + 15, yOffset, 80, 18), "+ Add Prop"))
                listProp.InsertArrayElementAtIndex(listProp.arraySize);

            if (GUI.Button(new Rect(position.x + 100, yOffset, 130, 18), "Sync from Registry"))
                SyncFields(listProp, availableFields);

            EditorGUI.EndProperty();
        }

        private void SyncFields(SerializedProperty listProp, string[] fields)
        {
            listProp.ClearArray();
            if (fields.Length == 1 && (fields[0].Contains("None") || fields[0].Contains("No Params"))) return;

            for (int i = 0; i < fields.Length; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).stringValue = fields[i];
            }
        }

        private string[] GetFieldsForEffect(EffectId id)
        {
            var def = EffectRegistry.FindById(id);
            if (def == null) return new[] { "None Found in Registry" };

            var fields = new List<string>();
            if (def.hasMask && !string.IsNullOrEmpty(def.maskField))
                fields.Add(def.maskField);

            if (def.parms != null)
            {
                foreach (var p in def.parms)
                {
                    if (!string.IsNullOrEmpty(p.field))
                        fields.Add(p.field);
                }
            }

            return fields.Count > 0 ? fields.ToArray() : new[] { "No Params in Registry" };
        }
    }
#endif
}
