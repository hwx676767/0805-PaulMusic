using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Persistent project-wide settings for the AnimatedSprite shader system.
/// Stores pipeline target, group rendering order, and per-effect order within groups.
/// </summary>

namespace FlexiSprite
{
    [CreateAssetMenu(menuName = "AllInOne Sprite Shader/Project Settings", fileName = "AnimSpriteProjectSettings")]
    public class AnimSpriteProjectSettings : ScriptableObject
    {
        const string AssetPath = "Assets/AnimatedSprite/Resources/AnimSpriteProjectSettings.asset";
        public EditorKeywordMode editorKeywordMode = EditorKeywordMode.NoKeywords;
        public BuildKeywordMode buildKeywordMode = BuildKeywordMode.MultiCompile;
        [SerializeField] bool _buildKeywordModeInitialized;
        public List<AnimatedSpriteShader.ShaderVariant> excludedVariants = new List<AnimatedSpriteShader.ShaderVariant>();

        /// <summary>
        /// Existing projects created before the build override existed have no serialized value for the
        /// initialization flag. Force the default to MultiCompile once, then preserve user changes after that.
        /// </summary>
        public void EnsureBuildKeywordModeDefault()
        {
            if (_buildKeywordModeInitialized) return;
            buildKeywordMode = BuildKeywordMode.MultiCompile;
            _buildKeywordModeInitialized = true;
    #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
    #endif
        }

        // GLOBAL LIGHTING (project-wide multipliers)

        /// <summary>
        /// Scales how strongly direct lights (main light + additional lights) affect all AnimatedSprite
        /// sprites across the project. 0 = fully unlit, 1 = standard, >1 = over-bright. Applied on top
        /// of the lit shader paths (URP 3D Lit, Built-In Lit, HDRP Lit). URP 2D Lit uses Unity's own
        /// light loop — this multiplier is applied post-composition for 2D as well.
        /// </summary>
        [Range(0f, 10f)] public float globalLightIntensity = 1f;

        /// <summary>
        /// Scales the ambient/indirect (SH probe) contribution for all sprites. 0 = no ambient,
        /// 1 = standard. Useful for pushing a "fully lit by direct only" look or compensating for
        /// over-bright ambient in outdoor HDRP scenes.
        /// </summary>
        [Range(0f, 10f)] public float ambientLightIntensity = 1f;

        // Hdrp-Specific Environment Controls
        // Only shown in MasterSetup when Pipeline == HDRP. Applied in the HDRP Lit composition path.
        // These are local to the sprite shader — they do NOT modify the HDRP Volume system.

        /// <summary>HDRP exposure compensation in stops. Added to the scene's exposure multiplier. 0 = unchanged.</summary>
        [Range(-8f, 8f)] public float hdrpExposureCompensation = 0f;

        /// <summary>HDRP ambient (SH) multiplier. Stacks with <see cref="ambientLightIntensity"/>.</summary>
        [Range(0f, 4f)] public float hdrpAmbientMultiplier = 1f;

        /// <summary>HDRP indirect/bounce light multiplier (currently affects same SH path as ambient).</summary>
        [Range(0f, 4f)] public float hdrpIndirectMultiplier = 1f;

        // Pipeline

        [SerializeField] RenderPipelineTarget _pipeline = RenderPipelineTarget.BuiltIn;
        public RenderPipelineTarget Pipeline
        {
            get => _pipeline;
            set
            {
                if (_pipeline == value) return;
                _pipeline = value;
    #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
    #endif
            }
        }


#if UNITY_EDITOR
        static bool _globalRefreshQueued;

        void OnValidate()
        {
            QueueGlobalRefresh();
        }

        static void QueueGlobalRefresh()
        {
            if (_globalRefreshQueued)
                return;

            _globalRefreshQueued = true;
            EditorApplication.delayCall += () =>
            {
                _globalRefreshQueued = false;

                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                AnimatedSpriteShader.BroadcastGlobalsDirty(true);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            };
        }
#endif

        public bool IsSRP => _pipeline == RenderPipelineTarget.URP || _pipeline == RenderPipelineTarget.HDRP;
        public int VariantCount => 1 << (int)EffectGroup._Count;

        public string PipelineDefine => _pipeline switch
        {
            RenderPipelineTarget.URP => "AIO_URP",
            RenderPipelineTarget.HDRP => "AIO_HDRP",
            _ => "AIO_BUILTIN"
        };

        public static RenderPipelineTarget DetectPipeline()
        {
            var rp = GetActiveRenderPipelineAsset();

            if (rp == null)
                return RenderPipelineTarget.BuiltIn;

            string typeName = rp.GetType().FullName;

            if (typeName.Contains("Universal") || typeName.Contains("URP"))
                return RenderPipelineTarget.URP;

            if (typeName.Contains("HighDefinition") || typeName.Contains("HDRP"))
                return RenderPipelineTarget.HDRP;

            return RenderPipelineTarget.BuiltIn;
        }

        static UnityEngine.Object GetActiveRenderPipelineAsset()
        {
            var graphicsSettingsType = typeof(UnityEngine.Rendering.GraphicsSettings);
            var qualitySettingsType = typeof(QualitySettings);

            var currentPipeline = GetStaticObjectProperty(graphicsSettingsType, "currentRenderPipeline");
            if (currentPipeline != null)
                return currentPipeline;

            var qualityPipeline = GetStaticObjectProperty(qualitySettingsType, "renderPipeline");
            if (qualityPipeline != null)
                return qualityPipeline;

            var defaultPipeline = GetStaticObjectProperty(graphicsSettingsType, "defaultRenderPipeline");
            if (defaultPipeline != null)
                return defaultPipeline;

            // Older Unity versions expose the same setting with this name.
            return GetStaticObjectProperty(graphicsSettingsType, "renderPipelineAsset");
        }

        static UnityEngine.Object GetStaticObjectProperty(System.Type type, string propertyName)
        {
            var property = type.GetProperty(
                propertyName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            return property?.GetValue(null, null) as UnityEngine.Object;
        }

        // Group Order

        [System.Serializable]
        public class GroupConfig
        {
            public EffectGroup group;
            public bool enabled = true;
            public int renderOrder;
            public bool foldout;
        }

        [SerializeField] List<GroupConfig> _groups = new();
        public List<GroupConfig> Groups => _groups;

        static readonly EffectGroup[] DefaultGroupOrder =
        {
            EffectGroup.UV,
            EffectGroup.Filter,
            EffectGroup.Color,
            EffectGroup.ColorExt,
            EffectGroup.Edge,
            EffectGroup.Dissolve,
            EffectGroup.VFX,
            EffectGroup.Game,
        };

        public static bool IsLockedGroup(EffectGroup group) => group == EffectGroup.UV;

        public void EnsureGroups()
        {
            bool changed = false;
            var seen = new HashSet<EffectGroup>();

            for (int i = _groups.Count - 1; i >= 0; i--)
            {
                var group = _groups[i].group;
                if (group < 0 || group >= EffectGroup._Count || !seen.Add(group))
                {
                    _groups.RemoveAt(i);
                    changed = true;
                }
            }

            for (int i = 0; i < DefaultGroupOrder.Length; i++)
            {
                var group = DefaultGroupOrder[i];
                if (_groups.Any(g => g.group == group))
                    continue;

                _groups.Add(new GroupConfig { group = group, enabled = true, renderOrder = i });
                changed = true;
            }

            if (ApplyLockedGroupRules())
                changed = true;

            if (EnsureDefaultEffectOrderMigration())
                changed = true;

    #if UNITY_EDITOR
            if (changed)
                EditorUtility.SetDirty(this);
    #endif
        }

        bool ApplyLockedGroupRules()
        {
            bool changed = false;
            var uv = _groups.FirstOrDefault(g => g.group == EffectGroup.UV);
            if (uv != null)
            {
                if (!uv.enabled) { uv.enabled = true; changed = true; }
                if (uv.renderOrder != 0) { uv.renderOrder = 0; changed = true; }
            }

            var ordered = _groups
                .Where(g => g.group != EffectGroup.UV)
                .OrderBy(g => g.renderOrder)
                .ThenBy(g => System.Array.IndexOf(DefaultGroupOrder, g.group))
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                int wantedOrder = i + 1;
                if (ordered[i].renderOrder == wantedOrder)
                    continue;

                ordered[i].renderOrder = wantedOrder;
                changed = true;
            }

            return changed;
        }

        public List<GroupConfig> GetGroupsInRenderOrder()
        {
            EnsureGroups();
            return _groups.OrderBy(g => g.renderOrder).ToList();
        }

        // Effect Order

        [System.Serializable]
        public class EffectOrder
        {
            public string enableShader; // key: e.g. "_StrongTint_En"
            public int order;
        }

        [SerializeField] List<EffectOrder> _effectOrders = new();

        const string HueShiftEnableShader = "_HueShift_En";
        const string GradientEnableShader = "_Gradient_En";

        static readonly string[] DefaultEdgeEffectOrder =
        {
            "_Shadow_En",
            "_SoftShadow_En",
            "_Glow_En",
            "_Outline_En",
            "_InnerOutline_En",
            "_TexOutline_En",
            "_RainbowOutline_En",
            "_DistortOutline_En",
            "_DirOutline_En",
            "_OutlineOnly_En",
            "_GradOutline_En",
            "_AnimOutline_En",
            "_SelectHighlight_En",
            "_EnergyAura_En",
            "_FakeLighting_En",
        };

        public int GetEffectOrder(string enableShader)
        {
            foreach (var eo in _effectOrders)
                if (eo.enableShader == enableShader) return eo.order;
            return GetDefaultEffectOrder(enableShader);
        }

        public int GetDefaultEffectOrder(string enableShader)
        {
            int index = FindRegistryOrder(enableShader);
            if (index < 0)
                return 999;

            int edgeOrder = FindDefaultEdgeEffectOrder(enableShader);
            if (edgeOrder >= 0)
                return edgeOrder;

            int hueShiftIndex = FindRegistryOrder(HueShiftEnableShader);
            int gradientIndex = FindRegistryOrder(GradientEnableShader);

            if (hueShiftIndex >= 0 && gradientIndex >= 0)
            {
                if (enableShader == HueShiftEnableShader) return gradientIndex;
                if (enableShader == GradientEnableShader) return hueShiftIndex;
            }

            return index;
        }

        static int FindDefaultEdgeEffectOrder(string enableShader)
        {
            for (int i = 0; i < DefaultEdgeEffectOrder.Length; i++)
                if (DefaultEdgeEffectOrder[i] == enableShader)
                    return i;
            return -1;
        }

        static int FindRegistryOrder(string enableShader)
        {
            for (int i = 0; i < EffectRegistry.All.Length; i++)
                if (EffectRegistry.All[i].enableShader == enableShader)
                    return i;
            return -1;
        }

        int FindStoredEffectOrderIndex(string enableShader)
        {
            for (int i = 0; i < _effectOrders.Count; i++)
                if (_effectOrders[i].enableShader == enableShader)
                    return i;
            return -1;
        }

        bool EnsureDefaultEffectOrderMigration()
        {
            int hueShiftIndex = FindRegistryOrder(HueShiftEnableShader);
            int gradientIndex = FindRegistryOrder(GradientEnableShader);
            if (hueShiftIndex < 0 || gradientIndex < 0)
                return false;

            int storedHueShift = FindStoredEffectOrderIndex(HueShiftEnableShader);
            int storedGradient = FindStoredEffectOrderIndex(GradientEnableShader);
            if (storedHueShift < 0 || storedGradient < 0)
                return false;

            bool stillOldDefault =
                _effectOrders[storedHueShift].order == hueShiftIndex &&
                _effectOrders[storedGradient].order == gradientIndex;

            if (!stillOldDefault)
                return false;

            _effectOrders[storedHueShift].order = gradientIndex;
            _effectOrders[storedGradient].order = hueShiftIndex;
            return true;
        }

        public void SetEffectOrder(string enableShader, int order)
        {
            for (int i = 0; i < _effectOrders.Count; i++)
            {
                if (_effectOrders[i].enableShader == enableShader)
                {
                    _effectOrders[i].order = order;
    #if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
    #endif
                    return;
                }
            }
            _effectOrders.Add(new EffectOrder { enableShader = enableShader, order = order });
    #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
    #endif
        }

        /// <summary>Get effects for a group sorted by configured order.</summary>
        public List<EffectDef> GetOrderedEffects(EffectGroup group)
        {
            var effects = EffectRegistry.GetByGroup(group);
            effects.Sort((a, b) => GetEffectOrder(a.enableShader).CompareTo(GetEffectOrder(b.enableShader)));
            return effects;
        }

        /// <summary>Apply current order settings back to EffectDef.orderInGroup for generator.</summary>
        public void ApplyOrderToRegistry()
        {
            foreach (var def in EffectRegistry.All)
                def.orderInGroup = GetEffectOrder(def.enableShader);
        }

        // Shader File Paths

        [SerializeField] string _shaderFolder = "Assets/AnimatedSprite/Shaders";
        public string ShaderFolder { get => _shaderFolder; set { _shaderFolder = value;
    #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
    #endif
        } }

        // Singleton

        static AnimSpriteProjectSettings _instance;
        public static AnimSpriteProjectSettings Instance
        {
            get
            {
                if (_instance)
                {
                    _instance.EnsureBuildKeywordModeDefault();
                    return _instance;
                }
                _instance = Resources.Load<AnimSpriteProjectSettings>("AnimSpriteProjectSettings");
    #if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<AnimSpriteProjectSettings>();
                    _instance._pipeline = DetectPipeline();
                    _instance.EnsureGroups();
                    string dir = System.IO.Path.GetDirectoryName(AssetPath);
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    AssetDatabase.CreateAsset(_instance, AssetPath);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"[AnimSprite] Created settings. Pipeline: {_instance._pipeline}");
                }
    #endif
                _instance.EnsureBuildKeywordModeDefault();
                return _instance;
            }
        }
    }
}
