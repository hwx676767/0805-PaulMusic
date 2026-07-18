using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


namespace FlexiSprite
{
    public enum ShowcaseMode { Carousel, Grid }

    [Serializable]
    public class CategoryOrderSettings
    {
        public EffectCategory category;
        public List<EffectId> effects = new();
    }

    [Serializable]
    public class GridEffectEntry
    {
        public EffectId effectId;
        public Sprite spriteOverride;
    }

    public sealed class EffectShowcaseCarousel : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private ShowcaseMode mode = ShowcaseMode.Carousel;

        [Header("Wiring")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Transform rig;
        [SerializeField] private EffectShowcaseCard cardPrefab;

        [Header("Progress Bar (Sprite Renderer)")]
        [SerializeField] private Material progressBarMaterial;
        [SerializeField] private float barWidth = 6.0f;
        [SerializeField] private float barHeight = 0.2f;
        [SerializeField] private float bottomMargin = 1.2f;
        [SerializeField] private float minThumbWidth = 0.05f;

        [Header("Dummy Source")]
        [SerializeField] private AnimatedSpriteShader dummySource;

        [Header("Keyboard Controls")]
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        [SerializeField] private KeyCode catPrevKey = KeyCode.W;
        [SerializeField] private KeyCode catNextKey = KeyCode.S;

        [Header("Touch Controls")]
        [SerializeField] private float minSwipeDistance = 50f;

        [Header("Category UI")]
        [SerializeField] private TMP_Text categoryLabel;
        [SerializeField] private TMP_Text effectCountLabel;

        [Header("Fullscreen Transition")]
        [SerializeField] private UnityEngine.UI.Image transitionOverlay;
        [SerializeField] private Color transitionColor = Color.black;
        [SerializeField] private float transitionPeakAlpha = 0.85f;

        [Header("Safe Framing")]
        [SerializeField] private float viewportMargin = 0.06f;
        [SerializeField] private float cardWidthAtScale1 = 2.4f;
        [SerializeField] private float offscreenPad = 1.2f;

        [Header("Pose Settings")]
        [SerializeField] private float baseZ = 6.0f;
        [SerializeField] private float zStep = 0.9f;
        [SerializeField] private float scaleNear = 1.0f;
        [SerializeField] private float scaleMid = 0.75f;
        [SerializeField] private float scaleFar = 0.55f;
        [SerializeField] private float yawMid = 8f;
        [SerializeField] private float yawFar = 14f;
        [SerializeField] private float alphaNear = 1.0f;
        [SerializeField] private float alphaMid = 0.65f;
        [SerializeField] private float alphaFar = 0.18f;

        [Header("Animation Timings")]
        [SerializeField] private float slideDuration = 0.35f;
        [SerializeField] private float categoryOutDuration = 0.25f;
        [SerializeField] private float categoryInDuration = 0.35f;
        [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Category Switch Motion")]
        [SerializeField] private float categoryPushTowardCamera = 1.0f;
        [SerializeField] private float categorySpawnBehind = 1.6f;

        [Header("Effect Filter & Order")]
        [SerializeField] private List<CategoryOrderSettings> perCategoryOrder = new();
        [SerializeField] private List<EffectId> excludedEffects = new();

        [Header("Per-Effect Custom Sprites")]
        [SerializeField] private List<EffectSpriteOverride> spriteOverrides = new();

        [Header("Per-Effect Custom Card Prefabs")]
        [SerializeField] private List<EffectCardOverride> cardOverrides = new();

        [Header("Grid Mode")]
        [Tooltip("Effects to show in grid order. Each entry can have its own sprite override.")]
        [SerializeField] private List<GridEffectEntry> gridEffects = new();

        [Tooltip("Number of columns.")]
        [SerializeField] private int gridColumns = 10;

        [Tooltip("World-space spacing between grid cells.")]
        [SerializeField] private float gridCellSpacing = 2.8f;

        [Tooltip("Uniform scale for each grid card.")]
        [SerializeField] private float gridCardScale = 1.0f;

        [Tooltip("Z distance from camera for the grid plane.")]
        [SerializeField] private float gridZ = 8.0f;

        [Tooltip("Vertical offset to shift entire grid up/down.")]
        [SerializeField] private float gridYOffset = 0f;

        [Tooltip("Optional card prefab for grid cells (falls back to main cardPrefab).")]
        [SerializeField] private EffectShowcaseCard gridCardPrefab;

        [Serializable]
        public struct EffectCardOverride
        {
            public EffectId effectId;
            public EffectShowcaseCard cardPrefab;
        }

        [Serializable]
        public struct EffectSpriteOverride
        {
            public EffectId effectId;
            public Sprite sprite;
        }

        private const int SlotCount = 7;
        private const int CenterSlot = 3;

        private EffectShowcaseCard[] _slots = new EffectShowcaseCard[SlotCount];
        private Dictionary<EffectId, EffectShowcaseCard> _allCards = new();
        private PoseData[] _poses;
        private bool _animating;
        private List<EffectCategory> _activeCatOrder = new();
        private Dictionary<EffectCategory, List<EffectDef>> _listsByCat = new();
        private Dictionary<EffectId, Sprite> _spriteOverrideMap = new();
        private Dictionary<EffectId, EffectShowcaseCard> _cardOverrideMap = new();
        private int _catIndex;
        private int _centerIndex;

        private SpriteRenderer _barRenderer;
        private Material _barRuntimeMat;

        private Vector2 _touchStartPos;
        private bool _isSwiping;

        private List<EffectShowcaseCard> _gridCards = new();

        struct PoseData
        {
            public Vector3 pos;
            public Quaternion rot;
            public float scale;
            public float alpha;
            public float z;
        }

        private void Awake()
        {
            if (!targetCamera) targetCamera = Camera.main;
            if (!rig) rig = transform;

            BuildSpriteOverrideMap();

            if (mode == ShowcaseMode.Grid)
                InitGrid();
            else
                InitCarousel();
        }

        private void Update()
        {
            if (mode == ShowcaseMode.Grid) return;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStartPos = touch.position;
                    _isSwiping = true;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (_isSwiping && !_animating)
                    {
                        Vector2 delta = touch.position - _touchStartPos;

                        if (delta.magnitude >= minSwipeDistance)
                        {
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            {
                                if (delta.x < 0) Slide(+1); // Swipe Left -> Show Next Item
                                else Slide(-1);             // Swipe Right -> Show Previous Item
                            }
                            else
                            {
                                if (delta.y > 0) SwitchCategory(+1); // Swipe Up -> Next Category
                                else SwitchCategory(-1);             // Swipe Down -> Prev Category
                            }
                        }
                    }
                    _isSwiping = false;
                }
            }

            if (_animating) return;

            if (Input.GetKeyDown(leftKey)) Slide(-1);
            if (Input.GetKeyDown(rightKey)) Slide(+1);
            if (Input.GetKeyDown(catPrevKey)) SwitchCategory(-1);
            if (Input.GetKeyDown(catNextKey)) SwitchCategory(+1);
        }

        private void InitGrid()
        {
            if (categoryLabel) categoryLabel.gameObject.SetActive(false);
            if (effectCountLabel) effectCountLabel.gameObject.SetActive(false);
            if (transitionOverlay) transitionOverlay.gameObject.SetActive(false);

            if (gridEffects == null || gridEffects.Count == 0) return;

            int count = gridEffects.Count;
            int cols = Mathf.Max(1, gridColumns);
            int rows = Mathf.CeilToInt((float)count / cols);

            float totalW = (cols - 1) * gridCellSpacing;
            float totalH = (rows - 1) * gridCellSpacing;
            float startX = -totalW * 0.5f;
            float startY = totalH * 0.5f + gridYOffset;

            var basePrefab = gridCardPrefab != null ? gridCardPrefab : cardPrefab;

            for (int i = 0; i < count; i++)
            {
                var entry = gridEffects[i];
                var def = EffectRegistry.FindById(entry.effectId);
                if (def == null) continue;

                EffectShowcaseCard prefab = basePrefab;
                if (_cardOverrideMap.TryGetValue(entry.effectId, out var overridePrefab))
                    prefab = overridePrefab;

                var card = Instantiate(prefab, rig);
                card.name = $"Grid_{def.id}";
                card.SetSourceDummy(dummySource);

                Sprite sprite = entry.spriteOverride;
                if (sprite == null)
                    sprite = GetSpriteOverride(entry.effectId);

                card.BindEffect(def, sprite);

                int col = i % cols;
                int row = i / cols;
                float x = startX + col * gridCellSpacing;
                float y = startY - row * gridCellSpacing;

                card.ApplyPose(new Vector3(x, y, gridZ), Quaternion.identity, gridCardScale, 1f);

                card.HideLabel();

                _gridCards.Add(card);
            }
        }

        private void InitCarousel()
        {
            CreateProgressBar();
            BuildCategoryLists();
            BuildPoses();
            BuildAllCards();
            RefreshSlots();
            UpdateCategoryLabel();
            RefreshProgressBar();
            SetTransitionAlpha(0f);
        }

        private void CreateProgressBar()
        {
            GameObject barObj = new GameObject("CarouselProgressBar");
            barObj.transform.SetParent(rig);

            _barRenderer = barObj.AddComponent<SpriteRenderer>();
            _barRenderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f);

            if (progressBarMaterial != null)
            {
                _barRuntimeMat = new Material(progressBarMaterial);
                _barRenderer.material = _barRuntimeMat;
            }

            float zPos = baseZ - 0.5f;
            float frustumHeight = 2.0f * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * zPos;

            barObj.transform.localPosition = new Vector3(0, -(frustumHeight / 2f) + bottomMargin, zPos);
            barObj.transform.localScale = new Vector3(barWidth, barHeight, 1f);
        }

        private void RefreshProgressBar(float tOffset = 0f)
        {
            if (_barRuntimeMat == null) return;

            var list = CurrentList();
            int count = (list != null && list.Count > 0) ? list.Count : 1;

            float thumbWidth = Mathf.Max(minThumbWidth, 1f / count);
            float exactIndex = _centerIndex + tOffset;
            float thumbCenter = Mathf.Repeat(exactIndex / count, 1f);

            EffectCategory currentCat = _activeCatOrder[_catIndex];
            ColorUtility.TryParseHtmlString(GetColorForCategory(currentCat), out Color catColor);

            _barRuntimeMat.SetFloat("_ThumbWidth", thumbWidth);
            _barRuntimeMat.SetFloat("_ThumbCenter", thumbCenter);
            _barRuntimeMat.SetColor("_ThumbColor", catColor);
        }

        private void UpdateCategoryLabel()
        {
            if (_activeCatOrder.Count == 0) return;

            EffectCategory currentCat = _activeCatOrder[_catIndex];
            string hexColor = GetColorForCategory(currentCat);

            if (categoryLabel)
                categoryLabel.text = $"Category: <color={hexColor}>{currentCat}</color>";

            if (effectCountLabel)
            {
                var list = CurrentList();
                int count = list != null ? list.Count : 0;
                effectCountLabel.text = $"<color={hexColor}>{count}</color> effects";
            }
        }

        private string GetColorForCategory(EffectCategory category)
        {
            switch (category)
            {
                case EffectCategory.Color: return "#EF3131";
                case EffectCategory.Edge: return "#11F131";
                case EffectCategory.UV: return "#EF31FF";
                case EffectCategory.Dissolve: return "#22A2F2";
                case EffectCategory.Special: return "#92F100";
                default: return "#FFFFFF";
            }
        }

        private void Slide(int dir)
        {
            var list = CurrentList();
            if (list == null || list.Count == 0) return;
            StartCoroutine(CoSlide(dir));
        }

        private IEnumerator CoSlide(int dir)
        {
            _animating = true;
            var list = CurrentList();
            int count = list.Count;

            int exitEdge = dir > 0 ? 0 : SlotCount - 1;
            int incomingEdge = dir > 0 ? SlotCount - 1 : 0;
            var exitingCard = _slots[exitEdge];

            _centerIndex = Wrap(_centerIndex + dir, count);

            var incomingDef = list[Wrap(_centerIndex + (incomingEdge - CenterSlot), count)];
            var incomingCard = _allCards[incomingDef.id];
            Vector3 incomingStart = OffscreenStartForPose(incomingEdge, dir);
            incomingCard.ApplyPose(incomingStart, _poses[incomingEdge].rot, _poses[incomingEdge].scale, 0f);

            int n = SlotCount + 1;
            var cards = new EffectShowcaseCard[n];
            var sPos = new Vector3[n];
            var sRot = new Quaternion[n];
            var sScale = new float[n];
            var sAlpha = new float[n];
            var tPos = new Vector3[n];
            var tRot = new Quaternion[n];
            var tScale = new float[n];
            var tAlpha = new float[n];

            for (int i = 0; i < SlotCount; i++)
            {
                cards[i] = _slots[i];
                sPos[i] = _slots[i].transform.localPosition;
                sRot[i] = _slots[i].transform.localRotation;
                sScale[i] = _slots[i].transform.localScale.x;
                sAlpha[i] = _poses[i].alpha;
            }
            cards[SlotCount] = incomingCard;
            sPos[SlotCount] = incomingStart;
            sRot[SlotCount] = _poses[incomingEdge].rot;
            sScale[SlotCount] = _poses[incomingEdge].scale;
            sAlpha[SlotCount] = 0f;

            Vector3 exitTarget = OffscreenStartForPose(exitEdge, -dir);
            for (int i = 0; i < SlotCount; i++)
            {
                if (cards[i] == exitingCard)
                {
                    tPos[i] = exitTarget;
                    tRot[i] = _poses[exitEdge].rot;
                    tScale[i] = _poses[exitEdge].scale;
                    tAlpha[i] = 0f;
                }
                else
                {
                    var p = _poses[i - dir];
                    tPos[i] = p.pos;
                    tRot[i] = p.rot;
                    tScale[i] = p.scale;
                    tAlpha[i] = p.alpha;
                }
            }
            var ip = _poses[incomingEdge];
            tPos[SlotCount] = ip.pos;
            tRot[SlotCount] = ip.rot;
            tScale[SlotCount] = ip.scale;
            tAlpha[SlotCount] = ip.alpha;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / slideDuration;
                float k = ease.Evaluate(Mathf.Clamp01(t));
                for (int i = 0; i < n; i++)
                {
                    cards[i].ApplyPose(
                        Vector3.Lerp(sPos[i], tPos[i], k),
                        Quaternion.Slerp(sRot[i], tRot[i], k),
                        Mathf.Lerp(sScale[i], tScale[i], k),
                        Mathf.Lerp(sAlpha[i], tAlpha[i], k));
                }

                RefreshProgressBar(-dir * (1f - k));
                yield return null;
            }

            ShiftSlotArray(dir);
            _slots[incomingEdge] = incomingCard;
            RefreshSlots();
            RefreshProgressBar(0f);
            exitingCard.ApplyPose(HiddenPos(), Quaternion.identity, 1f, 0f);
            _animating = false;
        }

        private void SwitchCategory(int dir)
        {
            if (_activeCatOrder.Count <= 1) return;
            StartCoroutine(CoSwitchCategory(dir));
        }

        private IEnumerator CoSwitchCategory(int dir)
        {
            _animating = true;
            float tOut = 0f;
            while (tOut < 1f)
            {
                tOut += Time.deltaTime / categoryOutDuration;
                float k = ease.Evaluate(Mathf.Clamp01(tOut));
                for (int s = 0; s < SlotCount; s++)
                {
                    var p = _poses[s];
                    Vector3 pushedPos = p.pos + new Vector3(0f, 0f, -categoryPushTowardCamera);
                    _slots[s].ApplyPose(
                        Vector3.Lerp(p.pos, pushedPos, k),
                        p.rot,
                        p.scale,
                        Mathf.Lerp(p.alpha, 0f, k));
                }
                SetTransitionAlpha(k * transitionPeakAlpha);
                FadeLabelAlpha(1f - k);
                yield return null;
            }

            _catIndex = Wrap(_catIndex + dir, _activeCatOrder.Count);
            _centerIndex = 0;
            UpdateCategoryLabel();
            RefreshSlots();
            RefreshProgressBar(0f);

            float tIn = 0f;
            while (tIn < 1f)
            {
                tIn += Time.deltaTime / categoryInDuration;
                float k = ease.Evaluate(Mathf.Clamp01(tIn));
                for (int s = 0; s < SlotCount; s++)
                {
                    var p = _poses[s];
                    Vector3 behind = p.pos + new Vector3(0, 0, categorySpawnBehind);
                    _slots[s].ApplyPose(
                        Vector3.Lerp(behind, p.pos, k),
                        p.rot,
                        p.scale,
                        Mathf.Lerp(0f, p.alpha, k));
                }
                SetTransitionAlpha(transitionPeakAlpha * (1f - k));
                FadeLabelAlpha(k);
                yield return null;
            }
            SetTransitionAlpha(0f);
            _animating = false;
        }

        private void BuildCategoryLists()
        {
            _listsByCat.Clear();
            _activeCatOrder.Clear();
            var excluded = new HashSet<EffectId>(excludedEffects);
            foreach (var catSetting in perCategoryOrder)
            {
                _activeCatOrder.Add(catSetting.category);
                var list = new List<EffectDef>();
                foreach (var id in catSetting.effects)
                {
                    if (excluded.Contains(id)) continue;
                    var def = EffectRegistry.FindById(id);
                    if (def != null && def.category == catSetting.category) list.Add(def);
                }
                _listsByCat[catSetting.category] = list;
            }
        }

        private void RefreshSlots()
        {
            var list = CurrentList();
            if (list == null || list.Count == 0) return;
            foreach (var card in _allCards.Values) card.ApplyPose(HiddenPos(), Quaternion.identity, 1f, 0f);
            for (int s = 0; s < SlotCount; s++)
            {
                int offset = s - CenterSlot;
                var def = list[Wrap(_centerIndex + offset, list.Count)];
                _slots[s] = _allCards[def.id];
                var p = _poses[s];
                _slots[s].ApplyPose(p.pos, p.rot, p.scale, p.alpha);
            }
        }

        private void BuildPoses()
        {
            _poses = new PoseData[SlotCount];
            float zFar = baseZ + zStep * 2f;
            float zMid = baseZ + zStep * 1f;
            float zNear = baseZ;
            float zOff = baseZ + zStep * 3f;
            float halfWFar = HalfFrustumWidthAtZ(zFar) * (1f - viewportMargin);
            float halfWMid = HalfFrustumWidthAtZ(zMid) * (1f - viewportMargin);
            float xOff = HalfFrustumWidthAtZ(zOff) * (1f - viewportMargin) + offscreenPad;
            float xFar = halfWFar * 0.92f;
            float xMid = halfWMid * 0.62f;

            _poses[0] = MakePose(-xOff, zOff, -yawFar, scaleFar, 0f);
            _poses[1] = MakePose(-xFar, zFar, -yawFar, scaleFar, alphaFar);
            _poses[2] = MakePose(-xMid, zMid, -yawMid, scaleMid, alphaMid);
            _poses[3] = MakePose(0f, zNear, 0f, scaleNear, alphaNear);
            _poses[4] = MakePose(+xMid, zMid, +yawMid, scaleMid, alphaMid);
            _poses[5] = MakePose(+xFar, zFar, +yawFar, scaleFar, alphaFar);
            _poses[6] = MakePose(+xOff, zOff, +yawFar, scaleFar, 0f);
        }

        private void BuildAllCards()
        {
            foreach (var def in EffectRegistry.All)
            {
                var prefab = _cardOverrideMap.TryGetValue(def.id, out var customPrefab) ? customPrefab : cardPrefab;
                var card = Instantiate(prefab, rig);
                card.name = $"Card_{def.id}";
                card.SetSourceDummy(dummySource);
                card.BindEffect(def, GetSpriteOverride(def.id));
                card.ApplyPose(HiddenPos(), Quaternion.identity, 1f, 0f);
                _allCards[def.id] = card;
            }
        }

        private void BuildSpriteOverrideMap()
        {
            _spriteOverrideMap.Clear();
            foreach (var o in spriteOverrides) if (o.sprite != null) _spriteOverrideMap[o.effectId] = o.sprite;
            _cardOverrideMap.Clear();
            foreach (var o in cardOverrides) if (o.cardPrefab != null) _cardOverrideMap[o.effectId] = o.cardPrefab;
        }

        private void ShiftSlotArray(int dir)
        {
            if (dir > 0)
            {
                var tmp = _slots[0];

                for (int i = 0; i < SlotCount - 1; i++)
                    _slots[i] = _slots[i + 1];

                _slots[SlotCount - 1] = tmp;
            }
            else
            {
                var tmp = _slots[SlotCount - 1];

                for (int i = SlotCount - 1; i > 0; i--)
                    _slots[i] = _slots[i - 1];

                _slots[0] = tmp;
            }
        }

        private PoseData MakePose(float x, float z, float yawDeg, float scale, float alpha)
        {
            return new PoseData
            {
                pos = new Vector3(x, 0f, z),
                rot = Quaternion.Euler(0f, yawDeg, 0f),
                scale = scale,
                alpha = alpha,
                z = z
            };
        }

        private Vector3 HiddenPos() => new Vector3(0f, -9999f, baseZ);
        private Vector3 OffscreenStartForPose(int poseIndex, int dir)
        {
            float z = _poses[poseIndex].z;
            float halfW = HalfFrustumWidthAtZ(z) * (1f - viewportMargin);
            float halfCard = cardWidthAtScale1 * _poses[poseIndex].scale * 0.5f;
            float x = dir > 0 ? halfW + halfCard + offscreenPad : -halfW - halfCard - offscreenPad;
            return new Vector3(x, _poses[poseIndex].pos.y, _poses[poseIndex].pos.z);
        }
        private float HalfFrustumWidthAtZ(float z)
        {
            return Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * z * targetCamera.aspect;
        }

        private List<EffectDef> CurrentList()
        {
            return _catIndex >= 0 && _catIndex < _activeCatOrder.Count
                ? _listsByCat[_activeCatOrder[_catIndex]]
                : null;
        }

        private Sprite GetSpriteOverride(EffectId id)
        {
            _spriteOverrideMap.TryGetValue(id, out var s);
            return s;
        }

        private void SetTransitionAlpha(float a)
        {
            if (!transitionOverlay) return;

            var c = transitionColor;
            c.a = a;
            transitionOverlay.color = c;
        }

        private void FadeLabelAlpha(float a)
        {
            if (categoryLabel)
            {
                var c = categoryLabel.color;
                c.a = a;
                categoryLabel.color = c;
            }

            if (effectCountLabel)
            {
                var c = effectCountLabel.color;
                c.a = a;
                effectCountLabel.color = c;
            }
        }

        private static int Wrap(int v, int n) => (n <= 0) ? 0 : (v % n + n) % n;

        private void OnValidate()
        {
            if (mode == ShowcaseMode.Carousel)
                SyncCategorySettings();
        }

        private void SyncCategorySettings()
        {
            var existingCats = new HashSet<EffectCategory>();
            foreach (var s in perCategoryOrder)
                existingCats.Add(s.category);

            foreach (EffectCategory cat in Enum.GetValues(typeof(EffectCategory)))
            {
                if (!existingCats.Contains(cat))
                    perCategoryOrder.Add(new CategoryOrderSettings { category = cat });
            }

            foreach (var catSettings in perCategoryOrder)
            {
                var existingEffects = new HashSet<EffectId>(catSettings.effects);

                foreach (var def in EffectRegistry.All)
                {
                    if (def.category == catSettings.category && !existingEffects.Contains(def.id))
                        catSettings.effects.Add(def.id);
                }

                catSettings.effects.RemoveAll(id =>
                {
                    var d = EffectRegistry.FindById(id);
                    return d == null || d.category != catSettings.category;
                });
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CategoryOrderSettings))]
    public class CategoryOrderDrawer : PropertyDrawer
    {
        readonly Dictionary<string, ReorderableList> _lists = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetList(property).GetHeight() + EditorGUIUtility.singleLineHeight + 10;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetList(property);
            var catProp = property.FindPropertyRelative("category");

            Rect headerRect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(
                headerRect,
                $"Category: {catProp.enumDisplayNames[catProp.enumValueIndex]}",
                EditorStyles.boldLabel);

            Rect listRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + 2,
                position.width,
                list.GetHeight());
            list.DoList(listRect);
        }

        ReorderableList GetList(SerializedProperty property)
        {
            string key = property.propertyPath;
            if (_lists.TryGetValue(key, out var list)) return list;

            var effectsProp = property.FindPropertyRelative("effects");
            list = new ReorderableList(effectsProp.serializedObject, effectsProp, true, true, false, true);
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Effects (Drag to reorder)");
            list.drawElementCallback = (rect, index, active, focused) =>
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight),
                    effectsProp.GetArrayElementAtIndex(index), GUIContent.none);

            _lists[key] = list;
            return list;
        }
    }

    [CustomPropertyDrawer(typeof(GridEffectEntry))]
    public class GridEffectEntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var idProp = property.FindPropertyRelative("effectId");
            var spriteProp = property.FindPropertyRelative("spriteOverride");

            float half = (position.width - 8) * 0.5f;
            var idRect = new Rect(position.x, position.y, half, EditorGUIUtility.singleLineHeight);
            var spriteRect = new Rect(position.x + half + 8, position.y, half, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
            EditorGUI.PropertyField(spriteRect, spriteProp, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
#endif
}
