using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


namespace FlexiSprite
{
    [ExecuteAlways, DisallowMultipleComponent]
    public partial class AnimatedSpriteShader : MonoBehaviour
    {
        public enum MaterialMode { Shared, Instanced }
        public enum ShaderTimeMode { ScaledTime = 0, UnscaledTime = 1, Custom = 2 }
        public enum ShaderVariant {
            // Built-In
            Standard, Additive, Multiply, GUI, GUIAdditive,
            StandardPremultiply, AdditiveSoft, GUIPremultiply, ScreenSpaceOverlay,
            Lit3D_BuiltIn, Lit3DCutout_BuiltIn,
            // URP Unlit
            Standard_URP, Additive_URP, Premultiply_URP, GUI_URP, GUIAdditive_URP,
            // URP Lit
            Lit2D_URP, Lit2DAdditive_URP,
            Lit3D_URP, Lit3DCutout_URP, Lit3DAdditive_URP,
            // HDRP
            // GUI omitted, HDRP has no Canvas UI integration. Use Built-In/URP GUI for UI sprites.
            HDRP_Unlit, HDRP_Additive, HDRP_Lit
        }
        public static readonly string[] ShaderPaths = {
            // Built-In
            "AnimatedSprite/Standard", "AnimatedSprite/Additive", "AnimatedSprite/Multiply",
            "AnimatedSprite/GUI", "AnimatedSprite/GUI Additive",
            "AnimatedSprite/Standard Premultiply", "AnimatedSprite/Additive Soft",
            "AnimatedSprite/GUI Premultiply", "AnimatedSprite/Screen Space Overlay",
            "AnimatedSprite/3D Lit Built-In", "AnimatedSprite/3D Lit Cutout Built-In",
            // URP Unlit
            "AnimatedSprite/Standard URP", "AnimatedSprite/Additive URP",
            "AnimatedSprite/Premultiply URP", "AnimatedSprite/GUI URP", "AnimatedSprite/GUI Additive URP",
            // URP Lit
            "AnimatedSprite/2D Lit URP", "AnimatedSprite/2D Lit Additive URP",
            "AnimatedSprite/3D Lit URP", "AnimatedSprite/3D Lit Cutout URP", "AnimatedSprite/3D Lit Additive URP",
            // HDRP (GUI  omitted)
            "AnimatedSprite/HDRP Unlit", "AnimatedSprite/HDRP Additive", "AnimatedSprite/HDRP Lit"
        };

        [SerializeField] ShaderVariant _shaderVariant = ShaderVariant.Standard;
        [SerializeField] MaterialMode _materialMode = MaterialMode.Shared;
        public MaterialMode CurrentMaterialMode
        {
            get => _materialMode;
            set
            {
                if (_materialMode == value)
                    return;

                _materialMode = value;
                InvalidatePushStrategy();
                ClearRendererPropertyBlock();
                SetupMaterial(true);
                Push();

#if UNITY_EDITOR
                EditorMarkMaterialBindingDirty();
#endif
            }
        }
        public ShaderVariant CurrentVariant
        {
            get => _shaderVariant;
            set => ChangeShaderVariant(value);
        }

        [Range(0, 1)] public float globalEffectFade = 1f;
        [Range(0, 1)] public float edgeCleanup = 0f;
        public bool effectMaskEnabled;
        public Texture2D effectMaskTexture;
        public bool pixelArtSnapEnabled;
        [Range(4, 256)] public float pixelSnapResolution = 32f;
        [Range(0, 5)] public float timeScale = 1f;
        [Range(0, 60)] public float timeFPS;
        public ShaderTimeMode timeMode = ShaderTimeMode.ScaledTime;
        [HideInInspector] public bool useCustomTime; // Legacy. Kept so old serialized data/presets still load.
        public float customTime;
        [Tooltip("Animates time-based shader effects in edit mode without pushing effect values or dirtying the scene. Disabled by default.")]
        public bool editorVisualUpdatesInEditMode;
        [SerializeField, HideInInspector] bool _timeModeInitialized;
        public bool outlineEffectsOnly;
        [Range(0, 0.15f)] public float outlineEffectWidth = 0.02f;
        [Range(0f, 6f)] public float outlineFixedWidthPx = 0f;
        public bool castShadows;
        public bool receiveShadows = true;
        [Range(0.01f, 1f)] public float shadowAlphaClip = 0.5f;
        [Range(0, 0.5f)] public float vertexUVExpand = 0f;
        // Keeps the previous effect flag state when everything is disabled.
        private bool effectsActiveLastUpdate = false;
        /// <summary>
        /// Two-sided lighting: when enabled, the shader flips the normal for back-facing pixels so
        /// the "other side" of a sprite quad is lit instead of rendering black. Safe to leave on for
        /// 2D sprites since they have no meaningful front/back distinction.
        /// </summary>
        public bool twoSidedLighting = true;

        public bool masksEnabled;
        public Texture2D maskR;
        public Texture2D maskG;
        public Texture2D maskB;
        public Texture2D maskA;
        [HideInInspector] public Texture2D packedMasks;

#if UNITY_EDITOR
        [System.NonSerialized] Texture2D _editorPackedMasks;
        [System.NonSerialized] int _editorPackedMasksHash;
#endif

        // Color
        public bool strongTintEnabled;
        public Color strongTintColor = Color.red;
        [Range(0.0f, 1.0f)] public float strongTintFade = 1f;
        [Range(0, 4)] public int strongTintMaskCh;

        public bool addColorEnabled;
        [ColorUsage(true, true)] public Color addColorAmount = Color.black;
        [Range(0.0f, 1.0f)] public float addColorFade = 1f;
        [Range(0, 4)] public int addColorMaskCh;

        public bool alphaTintEnabled;
        public Color alphaTintColor = Color.white;
        [Range(0.0f, 1.0f)] public float alphaTintFade = 0f;
        [Range(0, 4)] public int alphaTintMaskCh;

        public bool hitEffectEnabled;
        public Color hitEffectColor = Color.white;
        [Range(0.0f, 1.0f)] public float hitEffectBlend = 0f;
        [Range(0, 4)] public int hitEffectMaskCh;

        public bool negativeEnabled;
        [Range(0.0f, 1.0f)] public float negativeAmount = 1f;
        [Range(0, 4)] public int negativeMaskCh;

        public bool greyscaleEnabled;
        [Range(0.0f, 1.0f)] public float greyscaleBlend = 1f;
        public Vector4 greyscaleLuminosity = new Vector4(0.299f, 0.587f, 0.114f, 0);
        [Range(0, 4)] public int greyscaleMaskCh;

        public bool bloomEnabled;
        [Range(0, 3)] public int bloomMode = 0;
        [ColorUsage(true, true)] public Color bloomColor = Color.white;
        [Range(0.0f, 1.0f)] public float bloomThreshold = 0.5f;
        [Range(0.0f, 10.0f)] public float bloomIntensity = 1f;
        [Range(0.0f, 0.02f)] public float bloomSize = 0.003f;
        [Range(1, 4)] public int bloomSamples = 2;
        [Range(0.0f, 1.0f)] public float bloomSoftKnee = 0.5f;
        public Color bloomMaskColor = Color.white;
        [Range(0.0f, 1.0f)] public float bloomMaskRange = 0.3f;
        [Range(0.0f, 1.0f)] public float bloomMaskSoft = 0.5f;
        [Range(0, 4)] public int bloomMaskCh;
        [Range(0f, 1f)] public float bloomAlphaBleed = 0f;

        public bool colorReplaceEnabled;
        public Color colorReplaceSrc1 = Color.red;
        public Color colorReplaceDst1 = Color.blue;
        public Color colorReplaceSrc2 = Color.clear;
        public Color colorReplaceDst2 = Color.clear;
        public Color colorReplaceSrc3 = Color.clear;
        public Color colorReplaceDst3 = Color.clear;
        public Color colorReplaceSrc4 = Color.clear;
        public Color colorReplaceDst4 = Color.clear;
        [Range(1, 4)] public int colorReplaceCount = 1;
        [Range(0.0f, 1.0f)] public float colorReplaceRange = 0.1f;
        [Range(0.0f, 1.0f)] public float colorReplaceFade = 1f;
        [Range(0, 4)] public int colorReplaceMaskCh;

        public bool lutReplaceEnabled;
        public Texture2D lutReplaceTexture;
        [Range(0.0f, 1.0f)] public float lutReplaceFade = 1f;
        [Range(0, 4)] public int lutReplaceMaskCh;

        public bool hueShiftEnabled;
        [Range(0.0f, 1.0f)] public float hueShiftAmount = 0f;
        [Range(-10.0f, 10.0f)] public float hueShiftSpeed = 0f;
        [Range(0.0f, 1.0f)] public float hueShiftFade = 1f;
        [Range(0, 4)] public int hueShiftMaskCh;

        public bool addHueEnabled;
        [Range(0.0f, 1.0f)] public float addHueAmount = 0f;
        [Range(-10.0f, 10.0f)] public float addHueSpeed = 1f;
        [Range(0.0f, 1.0f)] public float addHueSaturation = 1f;
        [Range(0.0f, 2.0f)] public float addHueBrightness = 1f;
        [Range(0.0f, 1.0f)] public float addHueFade = 1f;
        [Range(0, 4)] public int addHueMaskCh;

        public bool saturationEnabled;
        [Range(0.0f, 3.0f)] public float saturationAmount = 1f;
        [Range(0, 4)] public int saturationMaskCh;

        public bool brightnessEnabled;
        [Range(-1.0f, 1.0f)] public float brightnessAmount = 0f;
        [Range(0, 4)] public int brightnessMaskCh;

        public bool contrastEnabled;
        [Range(0.0f, 3.0f)] public float contrastAmount = 1f;
        [Range(0, 4)] public int contrastMaskCh;

        public bool splitToningEnabled;
        public Color splitShadowColor = new Color(0.2f, 0.2f, 0.5f, 1);
        public Color splitMidColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public Color splitHighlightColor = new Color(1, 0.9f, 0.7f, 1);
        [Range(-1.0f, 1.0f)] public float splitBalance = 0f;
        [Range(0.0f, 1.0f)] public float splitFade = 1f;
        [Range(0, 4)] public int splitToningMaskCh;

        public bool posterizeEnabled;
        [Range(2.0f, 32.0f)] public float posterizeSteps = 4f;
        [Range(0.0f, 1.0f)] public float posterizeFade = 1f;
        [Range(0, 4)] public int posterizeMaskCh;

        public bool poisonEnabled;
        public Color poisonColor = new Color(0.2f, 0.8f, 0.1f, 1);
        public Color poisonColor2 = new Color(0.1f, 0.6f, 0.0f, 1);
        public Color poisonColor3 = new Color(0.3f, 1.0f, 0.2f, 1);
        [Range(-10.0f, 10.0f)] public float poisonSpeed = 1f;
        [Range(0.0f, 30.0f)] public float poisonDensity = 5f;
        [Range(0.0f, 1.0f)] public float poisonFade = 1f;
        [Range(0, 4)] public int poisonMaskCh;

        public bool frozenEnabled;
        public Color frozenColor = new Color(0.7f, 0.85f, 1, 1);
        public Color frozenColor2 = new Color(0.4f, 0.5f, 0.9f, 1);
        public Color frozenHighlightColor = Color.white;
        [Range(0.0f, 3.0f)] public float frozenContrast = 1.5f;
        [Range(0.0f, 3.0f)] public float frozenHighlight = 1f;
        [Range(0.0f, 1.0f)] public float frozenFade = 1f;
        [Range(0, 4)] public int frozenMaskCh;

        public bool metalEnabled;
        public Color metalColor = new Color(0.8f, 0.8f, 0.85f, 1);
        public Color metalColor2 = new Color(0.4f, 0.4f, 0.45f, 1);
        [ColorUsage(true, true)] public Color metalHighlight = Color.white;
        [Range(0.0f, 3.0f)] public float metalContrast = 1.5f;
        [Range(-10.0f, 10.0f)] public float metalSpeed = 1f;
        [Range(0.0f, 1.0f)] public float metalFade = 1f;
        [Range(0, 4)] public int metalMaskCh;

        public bool camouflageEnabled;
        public Color camoColor1 = new Color(0.2f, 0.35f, 0.1f, 1);
        public Color camoColor2 = new Color(0.35f, 0.3f, 0.15f, 1);
        public Color camoColor3 = new Color(0.15f, 0.2f, 0.1f, 1);
        [Range(0.0f, 30.0f)] public float camoScale = 5f;
        [Range(-10.0f, 10.0f)] public float camoSpeed = 0f;
        [Range(0.0f, 1.0f)] public float camoFade = 1f;
        [Range(0, 4)] public int camouflageMaskCh;

        public bool rainbowEnabled;
        [Range(-10.0f, 10.0f)] public float rainbowSpeed = 1f;
        [Range(0.0f, 1.0f)] public float rainbowSaturation = 1f;
        [Range(0.0f, 2.0f)] public float rainbowBrightness = 1f;
        [Range(0.0f, 1.0f)] public float rainbowFade = 1f;
        [Range(0.0f, 10.0f)] public float rainbowDensity = 1f;
        [Range(0, 4)] public int rainbowMaskCh;

        public bool blackTintEnabled;
        public Color blackTintColor = Color.blue;
        [Range(0.0f, 1.0f)] public float blackTintThreshold = 0.3f;
        [Range(0.0f, 1.0f)] public float blackTintFade = 1f;
        [Range(0, 4)] public int blackTintMaskCh;

        public bool waterReflectEnabled;
        public Color waterReflectTint = new Color(0.2f, 0.4f, 0.8f, 1);
        [Range(0f, 1f)] public float waterReflectYPoint = 0.5f;
        [Range(0f, 1f)] public float waterReflectDepth = 0.5f;
        [Range(0f, 1f)] public float waterReflectOpacity = 0.6f;
        [Range(0f, 1f)] public float waterReflectTintAmount = 0.3f;
        [Range(0f, 50f)] public float waterReflectWaveFreq = 10f;
        [Range(-10f, 10f)] public float waterReflectWaveSpeed = 2f;
        [Range(0f, 0.1f)] public float waterReflectWaveAmp = 0.01f;
        [Range(0f, 1f)] public float waterReflectFade = 1f;
        [Range(0, 4)] public int waterReflectMaskCh;

        public bool chromaticEnabled;
        [Range(0.0f, 0.1f)] public float chromaticAmount = 0.01f;
        [Range(-3.15f, 3.15f)] public float chromaticAngle = 0f;
        [Range(0, 4)] public int chromaticMaskCh;

        public bool paletteSwapEnabled;
        public Texture2D paletteSwapTexture;
        [Range(2, 32)] public int paletteSwapColors = 8;
        [Range(0.0f, 1.0f)] public float paletteSwapFade = 1f;
        [Range(0, 4)] public int paletteSwapMaskCh;

        public bool ditheringEnabled;
        [Range(1.0f, 16.0f)] public float ditheringSize = 4f;
        [Range(2.0f, 16.0f)] public float ditheringLevels = 4f;
        [Range(0.0f, 1.0f)] public float ditheringFade = 1f;
        [Range(0, 1)] public int ditheringAlphaMode = 0;
        [Range(0, 1)] public float ditheringAlphaThreshold = 1f;
        public bool ditheringScreenSpace = true;
        [Range(0, 4)] public int ditheringMaskCh;

        public bool crtScanlinesEnabled;
        [Range(10.0f, 500.0f)] public float crtDensity = 100f;
        [Range(0.0f, 1.0f)] public float crtIntensity = 0.3f;
        [Range(-10.0f, 10.0f)] public float crtSpeed = 0f;
        [Range(0.0f, 2.0f)] public float crtCurvature = 0f;
        [Range(0, 4)] public int crtScanlinesMaskCh;

        public bool vignetteEnabled;
        public Color vignetteColor = Color.black;
        [Range(0.0f, 3.0f)] public float vignetteIntensity = 1f;
        [Range(0.0f, 1.0f)] public float vignetteSmoothness = 0.5f;
        [Range(0f, 1f)] public float vignetteAlpha = 0f;
        public Vector4 vignetteCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0, 4)] public int vignetteMaskCh;
        public Vector2 vignetteOffset = Vector2.zero;

        public bool colorGradingEnabled;
        [Range(-3.0f, 3.0f)] public float cgExposure = 0f;
        [Range(0.1f, 3.0f)] public float cgGamma = 1f;
        public Vector4 cgLift = Vector4.zero;
        [Range(0, 4)] public int colorGradingMaskCh;

        public bool sepiaEnabled;
        [Range(0.0f, 1.0f)] public float sepiaIntensity = 1f;
        public Color sepiaColor = new Color(0.44f, 0.26f, 0.08f, 1);
        [Range(0, 4)] public int sepiaMaskCh;

        public bool thermalVisionEnabled;
        [Range(0.0f, 1.0f)] public float thermalIntensity = 1f;
        [Range(0f, 5f)] public float thermalHDRGlow;
        [Range(0, 4)] public int thermalVisionMaskCh;

        public bool recolorRGBEnabled;
        public Color recolorR = Color.red;
        public Color recolorG = Color.green;
        public Color recolorB = Color.blue;
        public Color recolorCyan = Color.cyan;
        public Color recolorMagenta = Color.magenta;
        public Color recolorYellow = Color.yellow;
        [Range(0.0f, 1.0f)] public float recolorFade = 1f;
        [Range(0, 4)] public int recolorRGBMaskCh;

        public bool pingPongGlowEnabled;
        [ColorUsage(true, true)] public Color pingPongGlowColor = Color.white;
        [Range(0.0f, 10.0f)] public float pingPongGlowSpeed = 2f;
        [Range(0.0f, 3.0f)] public float pingPongGlowMin = 0f;
        [Range(0.0f, 3.0f)] public float pingPongGlowMax = 1f;
        [Range(0, 4)] public int pingPongGlowMaskCh;

        public bool colorCycleEnabled;
        public Color colorCycleA = Color.red;
        public Color colorCycleB = Color.green;
        public Color colorCycleC = Color.blue;
        public Color colorCycleD = Color.yellow;
        [Range(0.0f, 10.0f)] public float colorCycleSpeed = 1f;
        [Range(0.0f, 1.0f)] public float colorCycleFade = 1f;
        [Range(0, 4)] public int colorCycleMaskCh;
        // Edge
        public bool outlineEnabled;
        [ColorUsage(true, true)] public Color outlineColor = Color.white;
        [Range(0.0f, 0.1f)] public float outlineSize = 0.01f;
        [Range(0.0f, 1.0f)] public float outlineAlpha = 1f;
        [Range(0, 4)] public int outlineMaskCh;

        public bool innerOutlineEnabled;
        public Color innerOutlineColor = Color.black;
        [Range(0.0f, 0.05f)] public float innerOutlineThickness = 0.005f;
        [Range(0.0f, 1.0f)] public float innerOutlineAlpha = 1f;
        [Range(0, 4)] public int innerOutlineMaskCh;

        public bool texOutlineEnabled;
        public Texture2D texOutlineTexture;
        [ColorUsage(true, true)] public Color texOutlineColor = Color.white;
        [Range(0.0f, 0.1f)] public float texOutlineSize = 0.005f;
        public Vector4 texOutlineSpeed = Vector4.zero;
        public Vector4 texOutlineTiling = new Vector4(1, 1, 0, 0);
        [Range(0, 4)] public int texOutlineMaskCh;

        public bool rainbowOutlineEnabled;
        [ColorUsage(true, true)] public Color rainbowOutlineColor = Color.white;
        [Range(0.0f, 0.1f)] public float rainbowOutlineSize = 0.005f;
        [Range(-10.0f, 10.0f)] public float rainbowOutlineSpeed = 1f;
        [Range(0.0f, 10.0f)] public float rainbowOutlineDensity = 1f;
        [Range(0, 4)] public int rainbowOutlineMaskCh;

        public bool distortOutlineEnabled;
        [ColorUsage(true, true)] public Color distortOutlineColor = Color.white;
        [Range(0.0f, 1.0f)] public float distortOutlineSize = 0.33f;
        [Range(0.0f, 0.5f)] public float distortOutlineAmount = 0.1f;
        [Range(-10.0f, 10.0f)] public float distortOutlineSpeed = 2f;
        [Range(0, 4)] public int distortOutlineMaskCh;

        public bool glowEnabled;
        [ColorUsage(true, true)] public Color glowColor = Color.cyan;
        [Range(0.0f, 0.15f)] public float glowSize = 0.02f;
        [Range(0.0f, 10.0f)] public float glowIntensity = 1f;
        [Range(0, 4)] public int glowMaskCh;

        public bool shadowEnabled;
        public Color shadowColor = new Color(0, 0, 0, 0.5f);
        public Vector4 shadowOffset = new Vector4(0.02f, -0.02f, 0, 0);
        [Range(0, 4)] public int shadowMaskCh;

        public bool softShadowEnabled;
        public Color softShadowColor = new Color(0, 0, 0, 0.4f);
        public Vector4 softShadowOffset = new Vector4(0.03f, -0.03f, 0, 0);
        [Range(0.0f, 0.05f)] public float softShadowSoftness = 0.01f;
        [Range(2, 6)] public int softShadowSamples = 3;
        [Range(0, 4)] public int softShadowMaskCh;

        public bool rimLightEnabled;
        [ColorUsage(true, true)] public Color rimColor = Color.white;
        [Range(0.0f, 0.15f)] public float rimWidth = 0.03f;
        [Range(0.0f, 10.0f)] public float rimIntensity = 1f;
        [Range(0, 4)] public int rimLightMaskCh;

        public bool outlineOnlyEnabled;
        [ColorUsage(true, true)] public Color outlineOnlyColor = Color.white;
        [Range(0.0f, 0.1f)] public float outlineOnlyWidth = 0.01f;
        [Range(0, 4)] public int outlineOnlyMaskCh;

        public bool gradOutlineEnabled;
        [ColorUsage(true, true)] public Color gradOutlineColorA = Color.white;
        [ColorUsage(true, true)] public Color gradOutlineColorB = Color.cyan;
        [Range(0.0f, 0.1f)] public float gradOutlineSize = 0.01f;
        [Range(-3.15f, 3.15f)] public float gradOutlineAngle = 1.5708f;
        [Range(0, 4)] public int gradOutlineMaskCh;

        public bool animOutlineEnabled;
        [ColorUsage(true, true)] public Color animOutlineColor = Color.white;
        [Range(0.0f, 0.05f)] public float animOutlineMin = 0.005f;
        [Range(0.0f, 0.1f)] public float animOutlineMax = 0.02f;
        [Range(0.0f, 10.0f)] public float animOutlineSpeed = 2f;
        [Range(0, 4)] public int animOutlineMaskCh;

        public bool selectHighlightEnabled;
        [ColorUsage(true, true)] public Color selectHighlightColor = Color.yellow;
        [Range(0.0f, 0.1f)] public float selectHighlightSize = 0.01f;
        [Range(2.0f, 50.0f)] public float selectHighlightDash = 10f;
        [Range(-10.0f, 10.0f)] public float selectHighlightSpeed = 3f;
        [Range(0, 4)] public int selectHighlightMaskCh;
        // Uv
        public bool pixelateEnabled;
        [Range(4.0f, 512.0f)] public float pixelateSize = 64f;
        [Range(0, 4)] public int pixelateMaskCh;

        public bool waveEnabled;
        [Range(-10.0f, 10.0f)] public float waveSpeed = 3f;
        [Range(0.0f, 0.5f)] public float waveAmplitude = 0.05f;
        [Range(0.0f, 30.0f)] public float waveFrequency = 10f;
        [Range(0, 4)] public int waveMaskCh;

        public bool twistEnabled;
        [Range(-10.0f, 10.0f)] public float twistAmount = 0f;
        [Range(-10.0f, 10.0f)] public float twistSpeed = 1f;
        [Range(0.0f, 2.0f)] public float twistRadius = 0.5f;
        [Range(0, 4)] public int twistMaskCh;
        public Vector2 twistOffset = Vector2.zero;

        public bool uvDistortEnabled;
        public Texture2D distortTexture;
        public Vector4 distortTiling = new Vector4(1, 1, 0, 0);
        [Range(0.0f, 0.5f)] public float distortAmount = 0.05f;
        [Range(-10.0f, 10.0f)] public float distortSpeed = 1f;
        [Range(0, 1)] public int distortIsNormalMap = 0;
        [Range(0, 4)] public int uvDistortMaskCh;

        public bool fullDistortEnabled;
        [Range(0.0f, 1.0f)] public float fullDistortAmount = 0.1f;
        [Range(-10.0f, 10.0f)] public float fullDistortSpeed = 1f;
        [Range(1.0f, 30.0f)] public float fullDistortDensity = 10f;
        [Range(0, 4)] public int fullDistortMaskCh;

        public bool dirDistortEnabled;
        [Range(-3.15f, 3.15f)] public float dirDistortAngle = 0f;
        [Range(0.0f, 0.5f)] public float dirDistortAmount = 0.1f;
        [Range(-10.0f, 10.0f)] public float dirDistortSpeed = 1f;
        [Range(0, 4)] public int dirDistortMaskCh;

        public bool uvScrollEnabled;
        [Range(-5.0f, 5.0f)] public float uvScrollSpeedX = 0.1f;
        [Range(-5.0f, 5.0f)] public float uvScrollSpeedY = 0f;
        [Range(0, 4)] public int uvScrollMaskCh;

        public bool rotateEnabled;
        [Range(-10.0f, 10.0f)] public float rotateSpeed = 1f;
        [Range(-3.15f, 3.15f)] public float rotateAmount = 0f;
        [Range(0, 4)] public int rotateMaskCh;

        public bool squeezeEnabled;
        public Vector4 squeezeAmount = new Vector4(0.1f, 0.1f, 0, 0);
        [Range(-10.0f, 10.0f)] public float squeezeSpeed = 2f;
        [Range(0.0f, 1.0f)] public float squeezePower = 0.5f;
        [Range(0, 4)] public int squeezeMaskCh;

        public bool squishEnabled;
        public Vector4 squishAmount = new Vector4(0.1f, 0.05f, 0, 0);
        [Range(-10.0f, 10.0f)] public float squishSpeed = 2f;
        [Range(0.0f, 1.0f)] public float squishPower = 0.5f;
        [Range(0, 4)] public int squishMaskCh;

        public bool wiggleEnabled;
        [Range(-20.0f, 20.0f)] public float wiggleSpeed = 5f;
        [Range(0.0f, 0.3f)] public float wiggleAmount = 0.05f;
        [Range(0.0f, 30.0f)] public float wiggleFrequency = 10f;
        [Range(0, 4)] public int wiggleMaskCh;

        public bool windEnabled;
        [Range(-10.0f, 10.0f)] public float windSpeed = 2f;
        [Range(0.0f, 0.5f)] public float windAmount = 0.1f;
        [Range(0.0f, 10.0f)] public float windFrequency = 3f;
        [Range(0, 1)] public float windBottomY = 0f;
        [Range(0, 4)] public int windMaskCh;

        public bool sharpenEnabled;
        [Range(0.0f, 5.0f)] public float sharpenAmount = 1f;
        [Range(0.0f, 0.01f)] public float sharpenSize = 0.002f;
        [Range(0, 4)] public int sharpenMaskCh;

        public bool gaussBlurEnabled;
        [Range(0.0f, 0.05f)] public float gaussBlurSize = 0.005f;
        [Range(0.0f, 1.0f)] public float gaussBlurSamples = 0f;
        [Range(0, 4)] public int gaussBlurMaskCh;

        public bool dirBlurEnabled;
        [Range(-3.15f, 3.15f)] public float dirBlurAngle = 0f;
        [Range(0.0f, 0.1f)] public float dirBlurSize = 0.01f;
        [Range(2.0f, 16.0f)] public float dirBlurSamples = 6f;
        [Range(0, 4)] public int dirBlurMaskCh;

        public bool worldTilingEnabled;
        [Range(0.01f, 10.0f)] public float worldTileScale = 1f;
        [Range(0, 4)] public int worldTilingMaskCh;
        public Vector2 worldTileOffset = Vector2.zero;
        [Range(-6.283f, 6.283f)] public float worldTileRotation = 0f;

        public bool shockwaveEnabled;
        public Vector4 shockwaveCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0.0f, 2.0f)] public float shockwaveRadius = 0f;
        [Range(0.01f, 0.3f)] public float shockwaveWidth = 0.1f;
        [Range(0.0f, 0.3f)] public float shockwaveAmplitude = 0.05f;
        [Range(0, 4)] public int shockwaveMaskCh;
        [Range(0f, 10f)] public float shockwaveSpeed = 0f;

        public bool sineScaleEnabled;
        [Range(0.0f, 0.5f)] public float sineScaleAmountX = 0.1f;
        [Range(0.0f, 0.5f)] public float sineScaleAmountY = 0.1f;
        [Range(0.0f, 10.0f)] public float sineScaleSpeed = 2f;
        [Range(0, 4)] public int sineScaleMaskCh;

        public bool heartbeatEnabled;
        [Range(0.0f, 10.0f)] public float heartbeatSpeed = 3f;
        [Range(0.0f, 0.3f)] public float heartbeatAmount = 0.1f;
        [Range(0, 4)] public int heartbeatMaskCh;

        public bool breathingEnabled;
        [Range(0.0f, 5.0f)] public float breathingSpeed = 1f;
        [Range(0.0f, 0.3f)] public float breathingAmount = 0.05f;
        [Range(0, 4)] public int breathingMaskCh;

        public bool liquidWobbleEnabled;
        [Range(0.0f, 10.0f)] public float liquidWobbleSpeed = 3f;
        [Range(0.0f, 0.3f)] public float liquidWobbleAmount = 0.05f;
        [Range(1.0f, 30.0f)] public float liquidWobbleFreq = 8f;
        [Range(0, 4)] public int liquidWobbleMaskCh;

        public bool flipBookBlendEnabled;
        public Texture2D flipBookNextFrame;
        [Range(0.0f, 1.0f)] public float flipBookLerp = 0f;
        [Range(0, 4)] public int flipBookBlendMaskCh;

        public bool smoothPixelArtEnabled;
        [Range(0.001f, 0.05f)] public float smoothPixelArtTexelSize = 0.01f;
        [Range(0, 4)] public int smoothPixelArtMaskCh;

        public bool fbmNoiseEnabled;
        [Range(0, 0.5f)] public float fbmNoiseAmount = 0.05f;
        [Range(-10, 10)] public float fbmNoiseSpeed = 1f;
        [Range(1, 30)] public float fbmNoiseDensity = 5f;
        [Range(1, 6)] public int fbmNoiseOctaves = 3;
        [Range(0, 4)] public int fbmNoiseMaskCh;

        public bool curlNoiseEnabled;
        [Range(0, 0.5f)] public float curlNoiseAmount = 0.05f;
        [Range(-10, 10)] public float curlNoiseSpeed = 1f;
        [Range(1, 30)] public float curlNoiseDensity = 5f;
        [Range(0, 4)] public int curlNoiseMaskCh;

        public bool uvScaleOffsetEnabled;
        public Vector4 uvScaleOffset = new Vector4(1, 1, 0, 0);
        [Range(0, 4)] public int uvScaleOffsetMaskCh;
        // Dissolve
        public bool fadeEnabled;

        public bool burnEnabled;

        public bool dissolveEnabled;
        [Range(0, 2)] public int dissolveNoiseType = 1;
        public Texture2D dissolveTexture;
        [Range(1.0f, 30.0f)] public float dissolveNoiseScale = 8f;
        [Range(0.0f, 1.0f)] public float dissolveAmount = 0f;
        [Range(0.0f, 0.3f)] public float dissolveBorderWidth = 0.05f;
        [ColorUsage(true, true)] public Color dissolveBorderColor = Color.yellow;
        [Range(0f, 0.3f)] public float dissolveBurnWidth = 0f;
        [ColorUsage(true, true)] public Color dissolveBurnColor = new Color(1, 0.3f, 0, 1);
        public Vector4 dissolveTiling = new Vector4(1, 1, 0, 0);
        [Range(0, 4)] public int dissolveMaskCh;

        public bool glowDissolveEnabled;
        [Range(0, 2)] public int glowDissolveNoiseType = 1;
        public Texture2D glowDissolveTexture;
        [Range(1.0f, 30.0f)] public float glowDissolveNoiseScale = 8f;
        [Range(0.0f, 1.0f)] public float glowDissolveAmount = 0f;
        [ColorUsage(true, true)] public Color glowDissolveColor = Color.cyan;
        [Range(0.0f, 0.5f)] public float glowDissolveWidth = 0.1f;
        [Range(0, 4)] public int glowDissolveMaskCh;

        public bool srcDissolveEnabled;
        [Range(0.0f, 1.0f)] public float srcDissolveAmount = 0f;
        public Vector4 srcDissolvePosition = new Vector4(0.5f, 0.5f, 0, 0);
        [ColorUsage(true, true)] public Color srcDissolveBorderColor = Color.white;
        [Range(0.0f, 0.3f)] public float srcDissolveBorderWidth = 0.05f;
        [Range(0, 4)] public int srcDissolveMaskCh;

        public bool dirFadeEnabled;
        [Range(-3.15f, 3.15f)] public float dirFadeAngle = 0f;
        [Range(0.0f, 2.0f)] public float dirFadeAmount = 0f;
        [Range(0.0f, 0.5f)] public float dirFadeSmoothness = 0.1f;
        [Range(0, 4)] public int dirFadeMaskCh;

        public bool dirGlowFadeEnabled;
        [Range(-3.15f, 3.15f)] public float dirGlowFadeAngle = 0f;
        [Range(0.0f, 1.0f)] public float dirGlowFadeAmount = 0f;
        [ColorUsage(true, true)] public Color dirGlowFadeColor = Color.cyan;
        [Range(0.0f, 0.5f)] public float dirGlowFadeWidth = 0.1f;
        [Range(0, 4)] public int dirGlowFadeMaskCh;

        public bool fullAlphaDissolveEnabled;
        [Range(0.0f, 1.0f)] public float fullAlphaDissolveAmount = 0f;
        [Range(0, 4)] public int fullAlphaDissolveMaskCh;

        public bool halftoneEnabled;
        [Range(1.0f, 100.0f)] public float halftoneSize = 30f;
        [Range(0.0f, 1.0f)] public float halftoneAmount = 0f;
        [Range(0.0f, 0.5f)] public float halftoneSmoothness = 0.1f;
        [Range(0, 4)] public int halftoneMaskCh;

        public bool alphaCutoffEnabled;
        [Range(0.0f, 1.0f)] public float alphaCutoff = 0.5f;
        [Range(0, 4)] public int alphaCutoffMaskCh;

        public bool radialWipeEnabled;
        [Range(0.0f, 1.0f)] public float radialWipeAmount = 0f;
        [Range(0.0f, 0.1f)] public float radialWipeSmoothness = 0.02f;
        [Range(-3.15f, 3.15f)] public float radialWipeAngle = 0f;
        [Range(0, 4)] public int radialWipeMaskCh;

        public bool irisWipeEnabled;
        [Range(0.0f, 1.0f)] public float irisWipeAmount = 0f;
        [Range(0.0f, 0.2f)] public float irisWipeSmoothness = 0.05f;
        public Vector4 irisWipeCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0, 4)] public int irisWipeMaskCh;

        public bool blindsEnabled;
        [Range(0.0f, 1.0f)] public float blindsAmount = 0f;
        [Range(2.0f, 30.0f)] public float blindsCount = 8f;
        [Range(0, 1)] public int blindsDirection = 0;
        [Range(0, 4)] public int blindsMaskCh;

        public bool diamondDissolveEnabled;
        [Range(0.0f, 1.0f)] public float diamondDissolveAmount = 0f;
        [Range(1.0f, 30.0f)] public float diamondDissolveScale = 8f;
        [Range(0, 4)] public int diamondDissolveMaskCh;

        public bool scanlineDissolveEnabled;
        [Range(0.0f, 1.0f)] public float scanlineDissolveAmount = 0f;
        [Range(10.0f, 200.0f)] public float scanlineDissolveDensity = 50f;
        [Range(0, 4)] public int scanlineDissolveMaskCh;

        public bool spreadFadeEnabled;
        [Range(0.0f, 1.0f)] public float spreadFadeAmount = 0f;
        public Vector4 spreadFadeOrigin = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0.0f, 0.3f)] public float spreadFadeSmoothness = 0.1f;
        [Range(0, 4)] public int spreadFadeMaskCh;

        public bool pixelDissolveEnabled;
        [Range(0, 1)] public float pixelDissolveAmount = 0f;
        [Range(4, 128)] public float pixelDissolveGrid = 32f;
        [Range(0, 4)] public int pixelDissolveMaskCh;

        public bool spiralWipeEnabled;
        [Range(0, 1)] public float spiralWipeAmount = 0f;
        [Range(0, 0.2f)] public float spiralWipeSmoothness = 0.05f;
        [Range(0, 10)] public float spiralWipeArms = 3f;
        [Range(0, 4)] public int spiralWipeMaskCh;

        public bool hexDissolveEnabled;
        [Range(0, 1)] public float hexDissolveAmount = 0f;
        [Range(2, 50)] public float hexDissolveScale = 10f;
        [Range(0, 4)] public int hexDissolveMaskCh;
        // Special
        public bool hologramEnabled;
        [Range(1.0f, 200.0f)] public float hologramStripesAmount = 50f;
        [Range(0.0f, 1.0f)] public float hologramMinAlpha = 0.3f;
        [Range(-10.0f, 10.0f)] public float hologramStripesSpeed = 1f;
        public Color hologramColor = Color.cyan;
        [Range(0.0f, 1.0f)] public float hologramBlend = 0.3f;
        [Range(0.0f, 0.1f)] public float hologramDistortion = 0.01f;
        [Range(0, 4)] public int hologramMaskCh;

        public bool glitchEnabled;
        [Range(0.0f, 1.0f)] public float glitchAmount = 0.5f;
        [Range(0.0f, 20.0f)] public float glitchSpeed = 5f;
        [Range(1.0f, 100.0f)] public float glitchSize = 20f;
        [Range(0.0f, 0.1f)] public float glitchColorShift = 0.01f;
        [Range(0, 4)] public int glitchMaskCh;

        public bool flickerEnabled;
        [Range(0.0f, 30.0f)] public float flickerFreq = 10f;
        [Range(0.0f, 1.0f)] public float flickerPercent = 0.3f;
        [Range(0, 4)] public int flickerMaskCh;

        public bool shineEnabled;
        [ColorUsage(true, true)] public Color shineColor = new Color(2, 2, 2, 1);
        [Range(0.01f, 1.0f)] public float shineWidth = 0.1f;
        [Range(-10.0f, 10.0f)] public float shineSpeed = 1f;
        [Range(0.0f, 10.0f)] public float shineDelay = 0f;
        [Range(-1.0f, 1.0f)] public float shineRotation = 0.3f;
        [Range(0.0f, 3.0f)] public float shineGlow = 1f;
        [Range(0, 4)] public int shineMaskCh;

        public bool smokeEnabled;
        public Color smokeColor = new Color(0.3f, 0.3f, 0.3f, 1);
        [Range(0.0f, 20.0f)] public float smokeDensity = 5f;
        [Range(-10.0f, 10.0f)] public float smokeSpeed = 1f;
        [Range(0.0f, 1.0f)] public float smokeFade = 0.5f;
        [Range(0f, 1f)] public float smokeTurbulence = 0.3f;
        public Vector4 smokeDirection = new Vector4(1, 0.5f, 0, 0);
        [Range(0f, 2f)] public float smokeColorStrength = 1f;
        [Range(0, 4)] public int smokeMaskCh;

        public bool flameEnabled;
        public Color flameColor1 = new Color(1, 0.9f, 0.3f, 1);
        public Color flameColor2 = new Color(1, 0.3f, 0, 1);
        [Range(-10.0f, 10.0f)] public float flameSpeed = 3f;
        [Range(0.1f, 10.0f)] public float flameDensity = 1f;
        [Range(0.0f, 3.0f)] public float flameIntensity = 1f;
        [Range(0, 1)] public int flameOnlyMode = 0;
        [Range(0f, 2f)] public float flameExtendSides = 0f;
        [Range(0f, 1f)] public float flameSideFalloff = 0.5f;
        [Range(0, 4)] public int flameMaskCh;

        public bool enchantedEnabled;
        [ColorUsage(true, true)] public Color enchantedColor = new Color(1, 0.8f, 0.3f, 1);
        [Range(-10.0f, 10.0f)] public float enchantedSpeed = 1f;
        [Range(0.0f, 10.0f)] public float enchantedDensity = 3f;
        [Range(0.0f, 3.0f)] public float enchantedBrightness = 1f;
        [Range(0.0f, 1.0f)] public float enchantedFade = 1f;
        [Range(0, 4)] public int enchantedMaskCh;

        public bool shiftingEnabled;
        public Color shiftingColor1 = Color.red;
        public Color shiftingColor2 = Color.blue;
        [Range(-10.0f, 10.0f)] public float shiftingSpeed = 2f;
        [Range(0.0f, 1.0f)] public float shiftingFade = 1f;
        [Range(0, 4)] public int shiftingMaskCh;

        public bool shiftRainbowEnabled;
        [Range(-10.0f, 10.0f)] public float shiftRainbowSpeed = 1f;
        [Range(0.0f, 10.0f)] public float shiftRainbowDensity = 1f;
        [Range(0.0f, 1.0f)] public float shiftRainbowFade = 1f;
        [Range(0, 4)] public int shiftRainbowMaskCh;

        public bool overlayEnabled;
        public Texture2D overlayTexture;
        public Color overlayColor = Color.white;
        [Range(0.0f, 1.0f)] public float overlayBlend = 0.5f;
        public Vector4 overlaySpeed = new Vector4(0.1f, 0, 0, 0);
        public Vector4 overlayTiling = new Vector4(1, 1, 0, 0);
        [Range(0, 4)] public int overlayMaskCh;

        public bool checkerboardEnabled;
        [Range(0, 2)] public int checkerCoordSpace = 0;
        [Range(1.0f, 50.0f)] public float checkerScale = 10f;
        public Color checkerColor1 = Color.white;
        public Color checkerColor2 = Color.gray;
        [Range(0, 4)] public int checkerboardMaskCh;

        public bool uvVisualizeEnabled;
        [Range(0.0f, 1.0f)] public float uvVisualizeBlend = 1f;
        [Range(0, 4)] public int uvVisualizeMaskCh;

        public bool uvCheckerEnabled;
        [Range(1.0f, 50.0f)] public float uvCheckerScale = 10f;
        [Range(0.0f, 1.0f)] public float uvCheckerBlend = 1f;
        [Range(0, 4)] public int uvCheckerMaskCh;

        public bool uvNumericEnabled;
        [Range(1, 50)] public float uvNumericScale = 10f;
        [Range(0, 1)] public float uvNumericBlend = 1f;
        [Range(0, 4)] public int uvNumericMaskCh;

        public bool voronoiCrackleEnabled;
        [Range(1, 50)] public float crackleScale = 10f;
        [Range(-5, 5)] public float crackleSpeed = 1f;
        [Range(0, 0.3f)] public float crackleEdgeWidth = 0.05f;
        [ColorUsage(true, true)] public Color crackleEdgeColor = Color.yellow;
        [Range(0, 1)] public float crackleCellTint = 0.3f;
        [Range(0, 1)] public float crackleFade = 1f;
        [Range(0, 4)] public int crackleMaskCh;

        public bool inkBleedEnabled;
        [Range(0, 0.5f)] public float inkBleedAmount = 0.1f;
        [Range(0, 0.5f)] public float inkBleedWidth = 0.1f;
        [Range(1, 50)] public float inkBleedScale = 10f;
        public Texture2D paperTex;
        [Range(0.1f, 20)] public float paperScale = 5f;
        [Range(0, 1)] public float paperAmount = 0.3f;
        [Range(0, 1)] public float inkBleedFade = 1f;
        [Range(0, 4)] public int inkBleedMaskCh;

        public bool energyAuraEnabled;
        [ColorUsage(true, true)] public Color auraColor = Color.cyan;
        [Range(0, 0.5f)] public float auraRadius = 0.1f;
        [Range(1, 50)] public float auraFrequency = 10f;
        [Range(0, 10)] public float auraSpeed = 3f;
        [Range(0, 3)] public float auraIntensity = 1f;
        [Range(0, 1)] public float auraFade = 1f;
        [Range(0, 4)] public int auraMaskCh;

        public bool retroPaletteEnabled;
        [Range(4, 256)] public float retroPixelSize = 64f;
        [Range(2, 64)] public float retroPaletteSize = 4f;
        public Texture2D retroPaletteTex;
        [Range(0, 1)] public float retroDitherStrength = 0.5f;
        [Range(0, 1)] public float retroFade = 1f;
        [Range(0, 4)] public int retroMaskCh;

        public bool motionSmearEnabled;
        public Vector4 smearDirection = new Vector4(1, 0, 0, 0);
        [Range(0, 0.5f)] public float smearAmount = 0.1f;
        [Range(2, 8)] public float smearSamples = 4f;
        [Range(0, 1)] public float smearFade = 1f;
        [Range(0, 4)] public int smearMaskCh;
        [Range(0, 1)] public int chromaMode;
        public Vector4 chromaRadialCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0, 2)] public int outlineMode;
        [Range(1, 4)] public float outlineRings = 2f;
        [Range(0, 0.1f)] public float outlineAA = 0f;
        public Texture2D outlineSDFTex;
        [Range(0, 0.1f)] public float outlineSDFSoftness = 0.01f;
        [Range(0.001f, 0.999f)] public float outlineSDFAlphaThreshold = 0.01f;
        [HideInInspector] public float outlineSDFSpread = 16f;

        public bool uvScrollOverlayEnabled;
        public Texture2D uvScrollOverlayTex;
        [Range(-5, 5)] public float uvScrollOverlaySpeedX = 0.1f;
        [Range(-5, 5)] public float uvScrollOverlaySpeedY = 0f;
        [Range(0, 1)] public float uvScrollOverlayBlend = 0.5f;
        [ColorUsage(true, true)] public Color uvScrollOverlayTint = Color.white;
        public Vector4 uvScrollOverlayTiling = new Vector4(1, 1, 0, 0);
        [Range(0, 4)] public int uvScrollOverlayMaskCh;
        public bool premulAlpha;
        [Range(0, 2)] public float dissolveNoiseSpeed;
        public bool spriteFlipX;
        public bool spriteFlipY;
        [Range(2, 64)] public float lutSize = 16f;

        public bool depthParallaxEnabled;
        public Texture2D parallaxHeightTex;
        public Vector4 parallaxDirection = new Vector4(0.02f, 0, 0, 0);
        [Range(0, 0.2f)] public float parallaxAmount = 0.05f;
        [Range(1, 8)] public float parallaxLayers = 3f;
        [Range(0, 1)] public float parallaxFade = 1f;
        [Range(0, 4)] public int parallaxMaskCh;

        public bool healthTintEnabled;
        public Color healthTintLowColor = Color.red;
        public Color healthTintHighColor = Color.green;
        [Range(0.0f, 1.0f)] public float healthTintAmount = 1f;
        [Range(0f, 1f)] public float healthDamageNoise = 0f;
        [Range(0f, 1f)] public float healthEdgeGrain = 0f;
        [Range(0f, 20f)] public float healthPulseSpeed = 0f;
        [Range(0f, 1f)] public float healthVignetteStr = 0f;
        [Range(0f, 1f)] public float healthDesaturation = 0.5f;
        [Range(0, 4)] public int healthTintMaskCh;

        public bool xraySilhouetteEnabled;
        public Color xraySilhouetteColor = new Color(0.2f, 0.5f, 1, 0.5f);
        [Range(0.0f, 1.0f)] public float xraySilhouetteBlend = 0.5f;
        [Range(0, 4)] public int xraySilhouetteMaskCh;

        public bool minimapPingEnabled;
        [ColorUsage(true, true)] public Color minimapPingColor = Color.white;
        [Range(0.0f, 10.0f)] public float minimapPingSpeed = 2f;
        [Range(0.01f, 0.2f)] public float minimapPingWidth = 0.05f;
        [Range(0, 4)] public int minimapPingMaskCh;
        public Vector2 minimapPingOffset = Vector2.zero;

        public bool ghostingEnabled;
        [Range(1, 5)] public int ghostingCount = 3;
        public Vector4 ghostingOffset = new Vector4(0.02f, 0, 0, 0);
        [Range(0.1f, 1.0f)] public float ghostingFade = 0.5f;
        [Range(0, 4)] public int ghostingMaskCh;

        public bool dialogueEmphasisEnabled;
        [Range(0.0f, 10.0f)] public float dialogueBobSpeed = 3f;
        [Range(0.0f, 0.1f)] public float dialogueBobAmount = 0.02f;
        [Range(0.0f, 1.0f)] public float dialogueBrightness = 0.15f;
        [Range(0, 4)] public int dialogueEmphasisMaskCh;


        // Stencil
        public bool stencilEnabled;
        [Range(0, 255)] public int stencilRef = 1;
        public enum StencilCompare { Disabled = 0, Never = 1, Less = 2, Equal = 3, LessEqual = 4, Greater = 5, NotEqual = 6, GreaterEqual = 7, Always = 8 }
        public StencilCompare stencilComp = StencilCompare.Always;
        public enum StencilOp { Keep = 0, Zero = 1, Replace = 2, IncrSat = 3, DecrSat = 4, Invert = 5, IncrWrap = 6, DecrWrap = 7 }
        public StencilOp stencilPass = StencilOp.Replace;
        [Range(0, 255)] public int stencilReadMask = 255;
        [Range(0, 255)] public int stencilWriteMask = 255;

        // Internal
        Renderer _rend;
        MaterialPropertyBlock _mpb;

        Renderer Rend { get { if (!_rend) _rend = GetComponent<Renderer>(); return _rend; } }

        static readonly Dictionary<int, Material> _sharedMaterials = new();
        static readonly HashSet<AnimatedSpriteShader> _activeInstances = new();

        // Material pool cleanup — prevents memory leaks in long editor sessions
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void CleanupOnDomainReload()
        {
            _sharedMaterials.Clear();
            _activeInstances.Clear();
        }

    #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void RegisterSceneCleanup()
        {
            // Use named handlers and unregister first so domain reloads / script recompiles do not
            // accumulate duplicate editor callbacks. Scene saves can clear non-asset material refs or
            // renderer property blocks in some Unity versions, so repaint the edit-mode material state
            // immediately after the save completes.
            UnityEditor.SceneManagement.EditorSceneManager.sceneClosing -= HandleEditorSceneClosing;
            UnityEditor.SceneManagement.EditorSceneManager.sceneClosing += HandleEditorSceneClosing;
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaved -= HandleEditorSceneSaved;
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += HandleEditorSceneSaved;
        }

        static void HandleEditorSceneClosing(UnityEngine.SceneManagement.Scene scene, bool removing)
        {
            CleanupUnusedMaterials();
        }

        static void HandleEditorSceneSaved(UnityEngine.SceneManagement.Scene scene)
        {
            RefreshSceneVisualBindingsAfterEditorSave(scene);
        }

        static void RefreshSceneVisualBindingsAfterEditorSave(UnityEngine.SceneManagement.Scene scene)
        {
            if (Application.isPlaying)
                return;

            // Run after Unity finishes its own save-time renderer/material serialization pass.
            // Some versions clear non-saved material refs / MPBs after sceneSaved fires, so doing
            // the rebind on delayCall is more reliable than doing it inside the event itself.
            UnityEditor.EditorApplication.delayCall += () => RefreshSceneVisualBindingsNow(scene);
        }

        static void RefreshSceneVisualBindingsNow(UnityEngine.SceneManagement.Scene scene)
        {
            if (Application.isPlaying)
                return;

            bool refreshedAny = false;

            void RefreshInstance(AnimatedSpriteShader inst)
            {
                if (!inst || !inst.isActiveAndEnabled || inst.gameObject.scene != scene)
                    return;

                inst.RefreshEditorMaterialBinding();
                refreshedAny = true;
            }

            foreach (var inst in _activeInstances)
                RefreshInstance(inst);

            // Static sets can be stale after recompiles/imports or if Unity reloads a scene while
            // editor callbacks are already registered. Fall back to the actual scene objects so the
            // save refresh never silently misses sprites.
            var all = Resources.FindObjectsOfTypeAll<AnimatedSpriteShader>();
            for (int i = 0; i < all.Length; i++)
                RefreshInstance(all[i]);

            if (!refreshedAny)
                return;

            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
    #endif

        /// <summary>Removes shared materials that are no longer referenced by any active instance.</summary>
        public static void CleanupUnusedMaterials()
        {
            var usedVariants = new HashSet<int>();
            foreach (var inst in _activeInstances)
                if (inst) usedVariants.Add((int)inst._shaderVariant);

            var toRemove = new List<int>();
            foreach (var kvp in _sharedMaterials)
                if (!usedVariants.Contains(kvp.Key) || !kvp.Value)
                    toRemove.Add(kvp.Key);

            foreach (var key in toRemove)
            {
                var mat = _sharedMaterials[key];
                if (mat)
                {
#if UNITY_EDITOR
                    if (!UnityEditor.EditorUtility.IsPersistent(mat))
                    {
                        if (Application.isPlaying) Object.Destroy(mat);
                        else Object.DestroyImmediate(mat);
                    }
#else
                    Object.Destroy(mat);
#endif
                }
                _sharedMaterials.Remove(key);
            }
        }

        // Effect-level dirty mask (128 bits = up to 128 effects)
        // Dirty mask moved to AnimatedSpriteShader.DirtyMask.cs (192-bit)
        bool _globalsDirty;

        // O(1) EffectId → index + De Bruijn bit-scan table
        static readonly Dictionary<EffectId, int> _effectIndexMap;

        static AnimatedSpriteShader()
        {
            var all = EffectRegistry.All;
            _effectIndexMap = new Dictionary<EffectId, int>(all.Length);
            for (int i = 0; i < all.Length; i++)
                _effectIndexMap[all[i].id] = i;

            InitDebruijn();
        }



        public void MarkEffectDirty(EffectId id)
        {
            if (!_effectIndexMap.TryGetValue(id, out int idx)) return;
            SetEffectDirtyBit(idx);
        }

        // SetEffectDirtyBit moved to DirtyMask.cs

        public bool MarkGlobalsDirty() => _globalsDirty = true;

        /// <summary>
        /// Marks all live AnimatedSpriteShader instances as globals-dirty. Used by editor tools
        /// (Master Setup project-wide sliders) so changes propagate to every sprite on the next
        /// frame without needing the user to select/nudge each one individually.
        /// Returns the number of sprites notified.
        /// </summary>
        public static int BroadcastGlobalsDirty(bool applyImmediately = false)
        {
            int n = 0;
            foreach (var inst in _activeInstances)
            {
                if (inst == null) continue;
                inst._globalsDirty = true;
                n++;

#if UNITY_EDITOR
                if (applyImmediately && !Application.isPlaying)
                    inst.ApplyGlobalsDirtyImmediatelyInEditor();
#endif
            }
            return n;
        }

#if UNITY_EDITOR
        void ApplyGlobalsDirtyImmediatelyInEditor()
        {
            // Project-wide lighting is stored outside the scene, so changing it should be a
            // visual-only refresh. Do not call SetupMaterial/MarkDirty here; that path can dirty
            // scenes just by opening them. Update the current MPB/material values in place.
            if (IsGUIMode)
            {
                PushGUI();
                return;
            }

            SyncRendererShadowSettings();
            IncrementalPush();
        }
#endif
        // AnyEffectDirty moved to DirtyMask.cs
        // ClearEffectDirtyMask moved to DirtyMask.cs

        // Called by Unity Animator after it writes any animated property — immediate same-frame response
        void OnDidApplyAnimationProperties()
        {
            MarkEnabledEffectsDirty(); // generated — direct bool checks, no reflection
            _globalsDirty = true;
        }

        // Global Snapshot
        // Prev values for global fields. Compared each frame to detect any external change.
        float _pg_globalEffectFade;
        float _pg_edgeCleanup;
        bool _pg_effectMaskEnabled;
        Texture2D _pg_effectMaskTexture;
        bool _pg_pixelArtSnapEnabled;
        float _pg_pixelSnapResolution;
        float _pg_timeScale;
        float _pg_timeFPS;
        ShaderTimeMode _pg_timeMode;
        bool _pg_useCustomTime;
        float _pg_customTime;
        bool _pg_outlineEffectsOnly;
        float _pg_outlineEffectWidth;
        float _pg_outlineFixedWidthPx;
        bool _pg_castShadows;
        bool _pg_receiveShadows;
        float _pg_shadowAlphaClip;
        bool _pg_masksEnabled;
        Texture2D _pg_packedMasks;
        bool _pg_premulAlpha;
        float _pg_dissolveNoiseSpeed;
        bool _pg_spriteFlipX;
        bool _pg_spriteFlipY;
        float _pg_lutSize;
        int _pg_chromaMode;
        Vector4 _pg_chromaRadialCenter;
        int _pg_outlineMode;
        float _pg_outlineRings;
        float _pg_outlineAA;
        Texture2D _pg_outlineSDFTex;
        float _pg_outlineSDFSoftness;
        float _pg_outlineSDFSpread;

        // Returns true if any global changed, updates snapshot for changed fields
        bool CheckGlobalsForChanges()
        {
            bool changed = false;
            if (globalEffectFade != _pg_globalEffectFade) { _pg_globalEffectFade = globalEffectFade; changed = true; }
            if (edgeCleanup != _pg_edgeCleanup) { _pg_edgeCleanup = edgeCleanup; changed = true; }
            if (effectMaskEnabled != _pg_effectMaskEnabled) { _pg_effectMaskEnabled = effectMaskEnabled; changed = true; }
            if (effectMaskTexture != _pg_effectMaskTexture) { _pg_effectMaskTexture = effectMaskTexture; changed = true; }
            if (pixelArtSnapEnabled != _pg_pixelArtSnapEnabled) { _pg_pixelArtSnapEnabled = pixelArtSnapEnabled; changed = true; }
            if (pixelSnapResolution != _pg_pixelSnapResolution) { _pg_pixelSnapResolution = pixelSnapResolution; changed = true; }
            if (timeScale != _pg_timeScale) { _pg_timeScale = timeScale; changed = true; }
            if (timeFPS != _pg_timeFPS) { _pg_timeFPS = timeFPS; changed = true; }
            if (timeMode != _pg_timeMode) { _pg_timeMode = timeMode; changed = true; }
            if (useCustomTime != _pg_useCustomTime) { _pg_useCustomTime = useCustomTime; changed = true; }
            if (customTime != _pg_customTime) { _pg_customTime = customTime; changed = true; }
            if (outlineEffectsOnly != _pg_outlineEffectsOnly) { _pg_outlineEffectsOnly = outlineEffectsOnly; changed = true; }
            if (outlineEffectWidth != _pg_outlineEffectWidth) { _pg_outlineEffectWidth = outlineEffectWidth; changed = true; }
            if (outlineFixedWidthPx != _pg_outlineFixedWidthPx) { _pg_outlineFixedWidthPx = outlineFixedWidthPx; changed = true; }
            if (castShadows != _pg_castShadows) { _pg_castShadows = castShadows; changed = true; }
            if (receiveShadows != _pg_receiveShadows) { _pg_receiveShadows = receiveShadows; changed = true; }
            if (shadowAlphaClip != _pg_shadowAlphaClip) { _pg_shadowAlphaClip = shadowAlphaClip; changed = true; }
            if (masksEnabled != _pg_masksEnabled) { _pg_masksEnabled = masksEnabled; changed = true; }
            if (packedMasks != _pg_packedMasks) { _pg_packedMasks = packedMasks; changed = true; }
            if (premulAlpha != _pg_premulAlpha) { _pg_premulAlpha = premulAlpha; changed = true; }
            if (dissolveNoiseSpeed != _pg_dissolveNoiseSpeed) { _pg_dissolveNoiseSpeed = dissolveNoiseSpeed; changed = true; }
            if (spriteFlipX != _pg_spriteFlipX) { _pg_spriteFlipX = spriteFlipX; changed = true; }
            if (spriteFlipY != _pg_spriteFlipY) { _pg_spriteFlipY = spriteFlipY; changed = true; }
            if (lutSize != _pg_lutSize) { _pg_lutSize = lutSize; changed = true; }
            if (chromaMode != _pg_chromaMode) { _pg_chromaMode = chromaMode; changed = true; }
            if (chromaRadialCenter != _pg_chromaRadialCenter) { _pg_chromaRadialCenter = chromaRadialCenter; changed = true; }
            if (outlineMode != _pg_outlineMode) { _pg_outlineMode = outlineMode; changed = true; }
            if (outlineRings != _pg_outlineRings) { _pg_outlineRings = outlineRings; changed = true; }
            if (outlineAA != _pg_outlineAA) { _pg_outlineAA = outlineAA; changed = true; }
            if (outlineSDFTex != _pg_outlineSDFTex) { _pg_outlineSDFTex = outlineSDFTex; changed = true; }
            if (outlineSDFSoftness != _pg_outlineSDFSoftness) { _pg_outlineSDFSoftness = outlineSDFSoftness; changed = true; }
            if (outlineSDFSpread != _pg_outlineSDFSpread) { _pg_outlineSDFSpread = outlineSDFSpread; changed = true; }
            return changed;
        }

        void InitGlobalSnapshot()
        {
            _pg_globalEffectFade = globalEffectFade;
            _pg_edgeCleanup = edgeCleanup;
            _pg_effectMaskEnabled = effectMaskEnabled;
            _pg_effectMaskTexture = effectMaskTexture;
            _pg_pixelArtSnapEnabled = pixelArtSnapEnabled;
            _pg_pixelSnapResolution = pixelSnapResolution;
            _pg_timeScale = timeScale;
            _pg_timeFPS = timeFPS;
            _pg_timeMode = timeMode;
            _pg_useCustomTime = useCustomTime;
            _pg_customTime = customTime;
            _pg_outlineEffectsOnly = outlineEffectsOnly;
            _pg_outlineEffectWidth = outlineEffectWidth;
            _pg_outlineFixedWidthPx = outlineFixedWidthPx;
            _pg_castShadows = castShadows;
            _pg_receiveShadows = receiveShadows;
            _pg_shadowAlphaClip = shadowAlphaClip;
            _pg_masksEnabled = masksEnabled;
            _pg_packedMasks = packedMasks;
            _pg_premulAlpha = premulAlpha;
            _pg_dissolveNoiseSpeed = dissolveNoiseSpeed;
            _pg_spriteFlipX = spriteFlipX;
            _pg_spriteFlipY = spriteFlipY;
            _pg_lutSize = lutSize;
            _pg_chromaMode = chromaMode;
            _pg_chromaRadialCenter = chromaRadialCenter;
            _pg_outlineMode = outlineMode;
            _pg_outlineRings = outlineRings;
            _pg_outlineAA = outlineAA;
            _pg_outlineSDFTex = outlineSDFTex;
            _pg_outlineSDFSoftness = outlineSDFSoftness;
            _pg_outlineSDFSpread = outlineSDFSpread;
        }

        // Lifecycle

        static Material GetOrCreateSharedMaterial(int variantIndex)
        {
            // Bounds check guards against deprecated serialized variant values (e.g. the old HDRP_GUI
            // enum entry that was index 24 before removal). MigrateDeprecatedVariant in OnEnable
            // catches these too, but this is a second line of defence for code paths that can run
            // before OnEnable (e.g. custom inspector previewing a freshly-created scriptable object).
            if (variantIndex < 0 || variantIndex >= ShaderPaths.Length) return null;
            if (_sharedMaterials.TryGetValue(variantIndex, out var existing) && existing)
                return existing;
            var shader = Shader.Find(ShaderPaths[variantIndex]);
            if (!shader) return null;
    #if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Material AnimSprite_" + (ShaderVariant)variantIndex);
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
                if (asset && asset.shader == shader) { _sharedMaterials[variantIndex] = asset; return asset; }
            }
    #endif
            var mat = new Material(shader) { name = "AnimSprite_" + (ShaderVariant)variantIndex };
            ConfigureGeneratedMaterial(mat);
            _sharedMaterials[variantIndex] = mat;
            return mat;
        }

        static void ConfigureGeneratedMaterial(Material mat)
        {
            if (!mat) return;
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(mat)) return;

            // In edit mode the material must survive scene saves long enough for Scene/Game view to
            // keep rendering. HideAndDontSave can be cleared by Unity during save, which makes sprites
            // fall back to the default material until Play Mode rebuilds them. We still keep generated
            // materials out of builds, and every component overwrites values from its own serialized
            // fields on enable/validate so duplicate/copy values do not leak.
            if (!Application.isPlaying)
            {
                mat.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInBuild;
                return;
            }
#endif
            mat.hideFlags = HideFlags.HideAndDontSave;
        }

        static Material CreateGeneratedMaterial(Material source, string suffix)
        {
            if (!source) return null;
            var mat = new Material(source) { name = source.name + suffix };
            ConfigureGeneratedMaterial(mat);
            return mat;
        }

        static ShaderVariant GetDefaultVariantForCurrentPipeline(bool wantsGUI)
        {
            switch (AnimSpriteProjectSettings.DetectPipeline())
            {
                case RenderPipelineTarget.URP:
                    return wantsGUI ? ShaderVariant.GUI_URP : ShaderVariant.Standard_URP;
                case RenderPipelineTarget.HDRP:
                    return ShaderVariant.HDRP_Unlit;
                default:
                    return wantsGUI ? ShaderVariant.GUI : ShaderVariant.Standard;
            }
        }

        static bool IsURPVariant(ShaderVariant v) => v switch
        {
            ShaderVariant.Standard_URP or ShaderVariant.Additive_URP or ShaderVariant.Premultiply_URP or
            ShaderVariant.GUI_URP or ShaderVariant.GUIAdditive_URP or ShaderVariant.Lit2D_URP or
            ShaderVariant.Lit2DAdditive_URP or ShaderVariant.Lit3D_URP or ShaderVariant.Lit3DCutout_URP or
            ShaderVariant.Lit3DAdditive_URP => true,
            _ => false,
        };

        static bool IsHDRPVariant(ShaderVariant v) => v switch
        {
            ShaderVariant.HDRP_Unlit or ShaderVariant.HDRP_Additive or ShaderVariant.HDRP_Lit => true,
            _ => false,
        };

        static bool IsBuiltInVariant(ShaderVariant v) => !IsURPVariant(v) && !IsHDRPVariant(v);

        static ShaderVariant ConvertVariantToPipeline(ShaderVariant v, RenderPipelineTarget pipeline, bool wantsGUI)
        {
            if (pipeline == RenderPipelineTarget.URP)
            {
                if (wantsGUI)
                {
                    return v switch
                    {
                        ShaderVariant.Additive or ShaderVariant.GUIAdditive or ShaderVariant.Additive_URP or ShaderVariant.GUIAdditive_URP => ShaderVariant.GUIAdditive_URP,
                        _ => ShaderVariant.GUI_URP,
                    };
                }

                return v switch
                {
                    ShaderVariant.Additive or ShaderVariant.GUIAdditive or ShaderVariant.AdditiveSoft or ShaderVariant.Additive_URP or ShaderVariant.GUIAdditive_URP => ShaderVariant.Additive_URP,
                    ShaderVariant.StandardPremultiply or ShaderVariant.GUIPremultiply or ShaderVariant.Premultiply_URP => ShaderVariant.Premultiply_URP,
                    ShaderVariant.Lit2D_URP => ShaderVariant.Lit2D_URP,
                    ShaderVariant.Lit2DAdditive_URP => ShaderVariant.Lit2DAdditive_URP,
                    ShaderVariant.Lit3D_BuiltIn or ShaderVariant.Lit3D_URP or ShaderVariant.HDRP_Lit => ShaderVariant.Lit3D_URP,
                    ShaderVariant.Lit3DCutout_BuiltIn or ShaderVariant.Lit3DCutout_URP => ShaderVariant.Lit3DCutout_URP,
                    ShaderVariant.Lit3DAdditive_URP or ShaderVariant.HDRP_Additive => ShaderVariant.Lit3DAdditive_URP,
                    _ => ShaderVariant.Standard_URP,
                };
            }

            if (pipeline == RenderPipelineTarget.HDRP)
            {
                return v switch
                {
                    ShaderVariant.Additive or ShaderVariant.GUIAdditive or ShaderVariant.Additive_URP or ShaderVariant.GUIAdditive_URP or ShaderVariant.HDRP_Additive => ShaderVariant.HDRP_Additive,
                    ShaderVariant.Lit3D_BuiltIn or ShaderVariant.Lit3D_URP or ShaderVariant.Lit3DCutout_BuiltIn or ShaderVariant.Lit3DCutout_URP or ShaderVariant.HDRP_Lit => ShaderVariant.HDRP_Lit,
                    _ => ShaderVariant.HDRP_Unlit,
                };
            }

            if (wantsGUI)
            {
                return v switch
                {
                    ShaderVariant.Additive_URP or ShaderVariant.GUIAdditive_URP or ShaderVariant.HDRP_Additive => ShaderVariant.GUIAdditive,
                    ShaderVariant.Premultiply_URP => ShaderVariant.GUIPremultiply,
                    _ => ShaderVariant.GUI,
                };
            }

            return v switch
            {
                ShaderVariant.Additive_URP or ShaderVariant.GUIAdditive_URP or ShaderVariant.HDRP_Additive => ShaderVariant.Additive,
                ShaderVariant.Premultiply_URP => ShaderVariant.StandardPremultiply,
                ShaderVariant.Lit3D_URP or ShaderVariant.HDRP_Lit => ShaderVariant.Lit3D_BuiltIn,
                ShaderVariant.Lit3DCutout_URP => ShaderVariant.Lit3DCutout_BuiltIn,
                _ => ShaderVariant.Standard,
            };
        }

        bool AutoRepairVariantForCurrentPipeline(bool markEditorDirty)
        {
            var pipeline = AnimSpriteProjectSettings.DetectPipeline();
            bool wantsGUI = IsGUIMode || (GetUIImage() != null && GetComponent<Renderer>() == null);

            bool compatible = pipeline switch
            {
                RenderPipelineTarget.URP => IsURPVariant(_shaderVariant),
                RenderPipelineTarget.HDRP => IsHDRPVariant(_shaderVariant),
                _ => IsBuiltInVariant(_shaderVariant),
            };

            if (compatible)
                return false;

            _shaderVariant = ConvertVariantToPipeline(_shaderVariant, pipeline, wantsGUI);
            InvalidatePushStrategy();
#if UNITY_EDITOR
            if (markEditorDirty && !Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            return true;
        }

        void Reset()
        {
            bool hasRenderer = GetComponent<Renderer>() != null;
            bool hasImage = GetUIImage() != null;

            // On first add, pick the variant from the actual project render pipeline and the
            // component type already on this GameObject. SpriteRenderers get sprite variants;
            // UGUI Images get GUI variants where the active pipeline supports them.
            _shaderVariant = GetDefaultVariantForCurrentPipeline(hasImage && !hasRenderer);
            SyncRendererForVariant(_shaderVariant);
        }

        // Old scenes can still deserialize the removed HDRP GUI enum value.
        void MigrateDeprecatedVariant()
        {
            int idx = (int)_shaderVariant;
            if (idx < 0 || idx >= ShaderPaths.Length)
            {
                var fallback = ShaderVariant.HDRP_Unlit;
                Debug.LogWarning($"[AnimSprite] '{name}' had a deprecated or invalid shader variant (index {idx}). " +
                                 $"Falling back to {fallback}. HDRP has no Canvas/UGUI shader — use Built-In or URP for UI sprites.", this);
                _shaderVariant = fallback;
    #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
    #endif
            }
        }

   
        void Awake()
        {
            Application.targetFrameRate = 1000;
            QualitySettings.vSyncCount = 0;
            AnimatedSpriteUpdater.NotifyCreated();
        }

        void OnEnable()
        {
            MigrateDeprecatedVariant();
            AutoRepairVariantForCurrentPipeline(false);
            EnsureTimeModeInitialized();
            AnimatedSpriteUpdater.Register(this);
            _activeInstances.Add(this);
            SetupMaterial(false);
            EnsurePackedMasksForCurrentMode(false);
            // Re-bake gradient textures if they were lost. The editor path avoids serialized writes on scene load.
            RebakeGradientTexturesIfNeeded();
            Push();
#if UNITY_EDITOR
            AnimSpriteShaderFeatureVariantKeeper.QueueRefreshIfShaderFeature();
#endif
        }

        /// <summary>
        /// Re-creates baked gradient textures if they were lost due to domain reload or scene reload.
        /// Runtime Texture2D objects are not serialized, so they vanish on recompile.
        /// </summary>
        public void RebakeGradientTexturesIfNeeded()
        {
            bool rebaked = false;
            // Color Ramp: always ensure texture exists when effect is on
            if (colorRampEnabled && colorRampGradient != null && colorRampGradient.colorKeys.Length > 0)
            {
                if (colorRampBakedGradient == null)
                { colorRampBakedGradient = BakeGradientToTexture(colorRampGradient, null); rebaked = true; }
                if (Application.isPlaying && colorRampTexture == null)
                { colorRampTexture = colorRampBakedGradient; rebaked = true; }
            }

            if (gradientEnabled && gradientUseGradientEditor)
            {
                if (gradientBakedTexture == null && gradientGradient != null && gradientGradient.colorKeys.Length > 0)
                { gradientBakedTexture = BakeGradientToTexture(gradientGradient, null); rebaked = true; }
            }

            if (radialGradientEnabled && radialGradUseGradientEditor)
            {
                if (radialGradBakedTexture == null && radialGradGradient != null && radialGradGradient.colorKeys.Length > 0)
                { radialGradBakedTexture = BakeGradientToTexture(radialGradGradient, null); rebaked = true; }
            }
            if (rebaked) _dirty = true;
        }

        void OnDisable()
        {
            AnimatedSpriteUpdater.Unregister(this);
            _activeInstances.Remove(this);
#if UNITY_EDITOR
            ReleaseEditorPackedMasks();
#endif
            if (_materialMode == MaterialMode.Shared && !PipelineRouting.PushesToMaterial(ResolvePushStrategy()))
                RebuildSharedKeywords((int)_shaderVariant);
        }

        /// <summary>
        /// Destroys the per-instance cloned materials we may have created (Instanced mode material
        /// clone, and the per-Image material clone used by GUI variants). Without this, every time
        /// a sprite is swapped in/out of a scene these clones accumulate in memory until Unity's GC
        /// collects the scene or the user manually calls <c>Resources.UnloadUnusedAssets</c>.
        /// </summary>
        void OnDestroy()
        {
#if UNITY_EDITOR
            ReleaseEditorPackedMasks();
#endif
            ReleaseInstancedMaterial();
            ReleaseImageMaterial();

            // Decrement the updater's live count. When this was the last sprite, the updater tears
            // down its manager GameObject — no extra cleanup needed here.
            AnimatedSpriteUpdater.NotifyDestroyed();
        }

        // SetupMaterial moved to AnimatedSpriteShader.Groups.cs

        /// <summary>Deprecated — use SyncMainTexture() instead. Kept for backward compatibility.</summary>
        public void SetupTextureQuickFix() { SyncMainTexture(); }

        public void ProcessUpdate()
        {
            // Velocity tracking for trails + motion smear (must run before push)
            UpdateVelocityTracking();

            // Ensure gradient textures exist (cheap null check, rebakes only if needed)
            RebakeGradientTexturesIfNeeded();

            // Full-push path: structural changes
            if (_dirty) { Push(); return; }

            // Snapshot comparison
            CheckEnabledEffectsForChanges();
            if (_dirty) { Push(); return; }

            if (RequiresPerFrameTimePush()) _globalsDirty = true;
            if (CheckGlobalsForChanges()) _globalsDirty = true;

            if (AnyEffectDirty() || _globalsDirty) IncrementalPush();

            // Always sync main texture — sprite frame may have changed (animation)
            SyncMainTexture();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Edit-mode visual-only refresh. This updates only volatile shader time/texture state so
        /// animated effects keep moving in Scene/Game view outside Play Mode. It does not read
        /// serialized fields for effect values, does not mark the component dirty, and does not run
        /// the normal full/incremental push path.
        /// </summary>
        public bool EditorVisualRefreshInEditMode(float editorTime)
        {
            if (Application.isPlaying || !isActiveAndEnabled)
                return false;

            // Even when animated edit-mode updates are disabled, keep the material binding alive.
            // Unity can drop HideAndDontSave materials during save/domain reload; without this the
            // sprite looks like the default shader until entering Play Mode.
            if (EditorMaterialBindingLooksInvalid())
            {
                RefreshEditorMaterialBinding();
                return true;
            }

            if (!editorVisualUpdatesInEditMode)
                return false;

            SyncMainTexture();

            float timeValue = GetEffectiveTimeMode() == ShaderTimeMode.Custom ? customTime : editorTime;
            float useCustom = 1f; // force editor-time in edit mode so previews animate without Play Mode.

            if (IsGUIMode)
            {
                var img = GetUIImage();
                var mat = img != null ? GetImageMaterial(img) : null;
                if (!mat)
                    return false;

                PushEditorVisualTimeToMaterial(mat, timeValue, useCustom);
                return true;
            }

            if (!Rend)
                return false;

            if (PipelineRouting.PushesToMaterial(ResolvePushStrategy()))
            {
                var mat = Rend.sharedMaterial;
                if (!mat)
                    return false;

                PushEditorVisualTimeToMaterial(mat, timeValue, useCustom);
                return true;
            }

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            Rend.GetPropertyBlock(_mpb);
            PushEditorVisualTimeToMPB(_mpb, timeValue, useCustom);
            Rend.SetPropertyBlock(_mpb);
            return true;
        }

        void PushEditorVisualTimeToMaterial(Material mat, float timeValue, float useCustom)
        {
            mat.SetFloat("_TimeScale", timeScale);
            mat.SetFloat("_TimeFPS", timeFPS);
            mat.SetFloat("_UseCustomTime", useCustom);
            mat.SetFloat("_CustomTime", timeValue);
        }

        void PushEditorVisualTimeToMPB(MaterialPropertyBlock block, float timeValue, float useCustom)
        {
            block.SetFloat("_TimeScale", timeScale);
            block.SetFloat("_TimeFPS", timeFPS);
            block.SetFloat("_UseCustomTime", useCustom);
            block.SetFloat("_CustomTime", timeValue);
        }

#endif

        /// <summary>Lightweight per-frame texture sync for animated sprites.</summary>
        void SyncMainTexture()
        {
            var sprite = GetActiveSprite();
            if (!sprite || !sprite.texture) return;
            var tex = sprite.texture;

            if (IsGUIMode)
            {
                var img = GetUIImage();
                if (img == null) return;
                var mat = GetImageMaterial(img);
                if (mat && mat.mainTexture != tex) mat.SetTexture("_MainTex", tex);
                return;
            }

            if (!Rend) return;

            if (_materialMode == MaterialMode.Instanced || PipelineRouting.PushesToMaterial(ResolvePushStrategy()))
            {
                var mat = Rend.sharedMaterial;
                if (mat && mat.mainTexture != tex) mat.SetTexture("_MainTex", tex);
            }
            else
            {
                if (_mpb == null) _mpb = new MaterialPropertyBlock();
                Rend.GetPropertyBlock(_mpb);
                _mpb.SetTexture("_MainTex", tex);
                Rend.SetPropertyBlock(_mpb);
            }
        }

        /// <summary>
        /// Returns the currently bound Sprite regardless of whether the object uses a SpriteRenderer
        /// or a UGUI Image. GUI variants read from Image; everything else reads from SpriteRenderer.
        /// </summary>
        public Sprite GetActiveSprite()
        {
            if (IsGUIMode)
            {
                var img = GetUIImage();
                if (img != null) return GetImageSprite(img);
            }
            var sr = GetComponent<SpriteRenderer>();
            return sr ? sr.sprite : null;
        }

        bool _dirty = true;

#if UNITY_EDITOR
        private bool _editorMaterialRebuildQueued;
#endif

        public void MarkDirty() { _dirty = true; }
        public void ForcePush() { Push(); _dirty = false; }

        public void RefreshAfterPresetApply()
        {
            MigrateDeprecatedVariant();
            InvalidatePushStrategy();
            SetupMaterial(true);
            EnsurePackedMasksForCurrentMode(!Application.isPlaying);
            RebakeGradientTexturesIfNeeded();
            ForcePush();
        }
        public void EnsureInstancedMode()
        {
            if (_materialMode != MaterialMode.Instanced)
            {
                _materialMode = MaterialMode.Instanced;
                InvalidatePushStrategy();
                SetupMaterial(true);
                _dirty = true;
            }
        }

        void OnValidate()
        {
            MigrateDeprecatedVariant();
            AutoRepairVariantForCurrentPipeline(true);
            EnsureTimeModeInitialized();
            _dirty = true;
#if UNITY_EDITOR
            QueueEditorMaterialRebuild();
            AnimSpriteShaderFeatureVariantKeeper.QueueRefreshIfShaderFeature();
#endif
        }

#if UNITY_EDITOR
        void QueueEditorMaterialRebuild()
        {
            if (Application.isPlaying || _editorMaterialRebuildQueued)
                return;

            _editorMaterialRebuildQueued = true;
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (!this)
                    return;

                _editorMaterialRebuildQueued = false;
                RefreshEditorMaterialBinding();
            };
        }

        void RefreshEditorMaterialBinding()
        {
            // OnValidate also runs while Unity is loading scenes and scripts. Keep this path visual-only:
            // do not add/remove render components and do not mark the scene dirty just because it opened.
            MigrateDeprecatedVariant();
            AutoRepairVariantForCurrentPipeline(false);
            InvalidatePushStrategy();
            ClearRendererPropertyBlock();
            SetupMaterial(true);
            EnsurePackedMasksForCurrentMode(true);
            PushEditorVisualState();
        }

        bool EditorMaterialBindingLooksInvalid()
        {
            int currentVariant = (int)_shaderVariant;
            if (currentVariant < 0 || currentVariant >= ShaderPaths.Length)
                return true;

            var expectedShader = Shader.Find(ShaderPaths[currentVariant]);
            if (!expectedShader)
                return false;

            if (IsGUIMode)
            {
                var img = GetUIImage();
                var mat = img != null ? GetImageMaterial(img) : null;
                if (!mat || mat.shader != expectedShader)
                    return true;
                return (mat.hideFlags & HideFlags.DontSaveInEditor) != 0;
            }

            var rend = Rend;
            var current = rend ? rend.sharedMaterial : null;
            if (!current || current.shader != expectedShader)
                return true;

            // Older patch versions created edit-mode materials with HideAndDontSave. Unity may clear
            // those on scene save, so upgrade/rebind them to the editor-safe flags above.
            return (current.hideFlags & HideFlags.DontSaveInEditor) != 0;
        }

        void PushEditorVisualState()
        {
            // ClearRendererPropertyBlock removes _MainTex from the renderer. Refill the block here so
            // simply opening a scene does not leave SpriteRenderers sampling Unity's white texture.
            if (IsGUIMode)
            {
                PushGUI();
                return;
            }

            var rend = Rend;
            if (!rend)
                return;

            var strategy = ResolvePushStrategy();

            if (PipelineRouting.PushesToMaterial(strategy))
            {
                var mat = EnsureInstancedMaterial(true);
                if (!mat)
                    return;

                ClearRendererPropertyBlock();
                PushMaterialDirect(mat);
                FinishPushState();
                return;
            }

            if (_materialMode == MaterialMode.Instanced)
            {
                var mat = EnsureInstancedMaterial(false);
                if (mat)
                    PushKeywordsToMaterial(mat);
            }
            else
            {
                RebuildSharedKeywords((int)_shaderVariant);
            }

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            rend.GetPropertyBlock(_mpb);
            PushStencilToMPB(_mpb);
            PushGlobal(_mpb);
            PushEffects(_mpb);
            rend.SetPropertyBlock(_mpb);

            FinishPushState();
        }
#endif

        void AssignSharedMaterial(bool forceRebind = true)
        {
            var mat = GetOrCreateSharedMaterial((int)_shaderVariant);
            var rend = Rend;

            if (mat && rend)
            {
                var current = rend.sharedMaterial;
                if (forceRebind || current == null || current.shader != mat.shader)
                {
                    if (current != mat)
                        rend.sharedMaterial = mat;
                }
            }

            ReleaseInstancedMaterial();
        }

        Material _instancedMat;
        Material _imageMat;

        Material EnsureInstancedMaterial(bool forceRebind = true)
        {
            var sourceMat = GetOrCreateSharedMaterial((int)_shaderVariant);
            var rend = Rend;

            if (!sourceMat || !rend)
                return null;

            if (_instancedMat && _instancedMat.shader == sourceMat.shader)
            {
                if (rend.sharedMaterial != _instancedMat)
                    rend.sharedMaterial = _instancedMat;

                return _instancedMat;
            }

            // Do not adopt the renderer's current material here. Duplicating/copy-pasting a
            // GameObject can leave another generated instance material on the renderer; adopting it
            // makes two sprites share stale material values. Always create this component's own
            // transient clone from the clean shared source instead.
            ReleaseInstancedMaterial();

            _instancedMat = CreateGeneratedMaterial(sourceMat, " (Instance)");

            if (rend.sharedMaterial != _instancedMat)
                rend.sharedMaterial = _instancedMat;

            return _instancedMat;
        }

        void ReleaseInstancedMaterial()
        {
            if (!_instancedMat)
                return;

            var rend = Rend;
            if (rend && rend.sharedMaterial == _instancedMat)
                rend.sharedMaterial = null;

            if (Application.isPlaying)
                Destroy(_instancedMat);
            else
                DestroyImmediate(_instancedMat);

            _instancedMat = null;
        }

        void ReleaseImageMaterial()
        {
            if (!_imageMat)
                return;

            var img = GetUIImage();
            if (img != null && GetImageMaterial(img) == _imageMat)
                SetImageMaterial(img, null);

            if (Application.isPlaying)
                Destroy(_imageMat);
            else
                DestroyImmediate(_imageMat);

            _imageMat = null;
        }

        void ClearRendererPropertyBlock()
        {
            if (IsGUIMode || !Rend)
                return;

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            _mpb.Clear();
            Rend.SetPropertyBlock(_mpb);
        }

#if UNITY_EDITOR
        void EditorMarkMaterialBindingDirty()
        {
            if (Application.isPlaying)
                return;

            UnityEditor.EditorUtility.SetDirty(this);

            var rend = GetComponent<Renderer>();
            if (rend)
                UnityEditor.EditorUtility.SetDirty(rend);

            var img = GetUIImage();
            if (img)
                UnityEditor.EditorUtility.SetDirty(img);

            if (gameObject.scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

        static void KW(Material m, string k, bool on) { if (on) m.EnableKeyword(k); else m.DisableKeyword(k); }
        public void RebuildCache() { _dirty = true; }

        void EnsureTimeModeInitialized()
        {
            if (useCustomTime)
                timeMode = ShaderTimeMode.Custom;

            if (_timeModeInitialized)
                return;

            if (!useCustomTime)
                timeMode = IsGUIMode ? ShaderTimeMode.UnscaledTime : ShaderTimeMode.ScaledTime;

            _timeModeInitialized = true;
        }

        ShaderTimeMode GetEffectiveTimeMode()
        {
            EnsureTimeModeInitialized();
            return useCustomTime ? ShaderTimeMode.Custom : timeMode;
        }

        bool RequiresPerFrameTimePush()
        {
            return GetEffectiveTimeMode() == ShaderTimeMode.UnscaledTime;
        }

        float GetShaderTimeValue()
        {
            return GetEffectiveTimeMode() == ShaderTimeMode.UnscaledTime ? Time.unscaledTime : customTime;
        }

        float GetUseCustomShaderTimeFlag()
        {
            return GetEffectiveTimeMode() == ShaderTimeMode.ScaledTime ? 0f : 1f;
        }

        // Push moved to AnimatedSpriteShader.Groups.cs (pipeline-aware)

        // IncrementalPush moved to AnimatedSpriteShader.Groups.cs (pipeline-aware)

        void PushStencilToMaterial(Material mat)
        {
            // Stencil block in the generated shader references the Unity-standard property names
            // (_Stencil, _StencilOp, _StencilComp, _StencilReadMask, _StencilWriteMask). The legacy
            // _StencilRef/_StencilPass aliases are also set for any old material that still uses them.
            float refVal = stencilEnabled ? stencilRef : 0;
            float compVal = stencilEnabled ? (float)stencilComp : 8;
            float passVal = stencilEnabled ? (float)stencilPass : 0;
            float readMask = stencilEnabled ? stencilReadMask : 255;
            float writeMask = stencilEnabled ? stencilWriteMask : 255;
            mat.SetFloat("_Stencil", refVal);
            mat.SetFloat("_StencilOp", passVal);
            mat.SetFloat("_StencilComp", compVal);
            mat.SetFloat("_StencilReadMask", readMask);
            mat.SetFloat("_StencilWriteMask", writeMask);
            // Legacy aliases (harmless if the shader doesn't declare them).
            mat.SetFloat("_StencilRef", refVal);
            mat.SetFloat("_StencilPass", passVal);
        }

        void PushStencilToMPB(MaterialPropertyBlock b)
        {
            float refVal = stencilEnabled ? stencilRef : 0;
            float compVal = stencilEnabled ? (float)stencilComp : 8;
            float passVal = stencilEnabled ? (float)stencilPass : 0;
            float readMask = stencilEnabled ? stencilReadMask : 255;
            float writeMask = stencilEnabled ? stencilWriteMask : 255;
            b.SetFloat("_Stencil", refVal);
            b.SetFloat("_StencilOp", passVal);
            b.SetFloat("_StencilComp", compVal);
            b.SetFloat("_StencilReadMask", readMask);
            b.SetFloat("_StencilWriteMask", writeMask);
            b.SetFloat("_StencilRef", refVal);
            b.SetFloat("_StencilPass", passVal);
        }

        void PushGlobal(MaterialPropertyBlock b)
        {
            // Safe sprite lookup — without this, a component on a GameObject whose SpriteRenderer
            // has no assigned sprite (common during level loading / asset refactors) throws NRE
            // every frame from PushGlobal → ProcessUpdate → LateUpdate. Now we tolerate missing
            // sprites and fall back to defaults so the component stays silent until a sprite appears.
            var sr = GetComponent<SpriteRenderer>();
            var sprite = sr ? sr.sprite : null;
            if (sprite && sprite.texture) b.SetTexture("_MainTex", sprite.texture);

            b.SetFloat("_GlobalEffectFade", globalEffectFade);
            b.SetFloat("_EdgeCleanup", edgeCleanup);
            b.SetFloat("_PixelSnapResolution", pixelSnapResolution);
            b.SetFloat("_TimeScale", timeScale);
            b.SetFloat("_TimeFPS", timeFPS);
            b.SetFloat("_UseCustomTime", GetUseCustomShaderTimeFlag());
            b.SetFloat("_CustomTime", GetShaderTimeValue());
            b.SetFloat("_OutlineEffectWidth", outlineEffectWidth);
            b.SetFloat("_OutlineEffectsOnly_En", outlineEffectsOnly ? 1f : 0f);
            b.SetFloat("_OutlineFixedWidthPx", outlineFixedWidthPx);
            b.SetFloat("_CastShadows", castShadows ? 1f : 0f);
            b.SetFloat("_ReceiveShadows", receiveShadows ? 1f : 0f);
            b.SetFloat("_ShadowAlphaClip", shadowAlphaClip);
            b.SetFloat("_FlipNormals", twoSidedLighting ? 1f : 0f);
            b.SetFloat("_EffectMask_En", effectMaskEnabled ? 1f : 0f);
            b.SetFloat("_PixelArtSnap_En", pixelArtSnapEnabled ? 1f : 0f);
            var activeMasks = EnsurePackedMasksForCurrentMode(false);
            b.SetFloat("_MaskTex_En", (masksEnabled && activeMasks) ? 1f : 0f);
            if (effectMaskTexture) b.SetTexture("_GlobalEffectMask", effectMaskTexture);
            if (activeMasks) b.SetTexture("_EffectMasks", activeMasks);
            b.SetFloat("_ChromaMode", (float)chromaMode);
            b.SetVector("_ChromaRadialCenter", chromaRadialCenter);
            b.SetFloat("_OutlineMode", (float)outlineMode);
            b.SetFloat("_OutlineRings", outlineRings);
            b.SetFloat("_OutlineAA", outlineAA);
            if (outlineSDFTex) b.SetTexture("_OutlineSDFTex", outlineSDFTex);
            b.SetFloat("_OutlineSDFSoftness", outlineSDFSoftness);
            b.SetFloat("_OutlineSDFPadding", 16f);
            b.SetFloat("_OutlineSDFSpread", outlineSDFSpread);
            b.SetFloat("_PremulAlpha", premulAlpha ? 1f : 0f);
            b.SetFloat("_DissolveNoiseSpeed", dissolveNoiseSpeed);
            b.SetFloat("_SpriteFlipX", spriteFlipX ? 1f : 0f);
            b.SetFloat("_SpriteFlipY", spriteFlipY ? 1f : 0f);
            // Safe aspect computation — falls back to 1.0 when no sprite is assigned.
            float aspect = 1f;
            if (sprite)
            {
                float h = sprite.rect.height;
                if (h > 1e-5f) aspect = sprite.rect.width / h;
            }
            b.SetFloat("_SpriteAspect", aspect);
            b.SetFloat("_LUTSize", lutSize);
            b.SetFloat("_VertexUVExpand", vertexUVExpand);
            b.SetFloat("_AlphaCutoff_En", alphaCutoffEnabled ? 1f : 0f);
            b.SetFloat("_AlphaCutoff", alphaCutoff);
            PushLightingGlobals(b);
            PushAtlasUV(b);
            PushOutlineSDFRect(b);
        }

        /// <summary>
        /// Pushes project-wide lighting multipliers (global light intensity, ambient intensity,
        /// HDRP env controls) from <see cref="AnimSpriteProjectSettings"/> to a MaterialPropertyBlock.
        /// If the settings asset cannot be loaded, safe defaults (1.0, 1.0, 0.0) are used.
        /// </summary>
        void PushLightingGlobals(MaterialPropertyBlock b)
        {
            float gli = 1f, ali = 1f, exp = 0f, hAmb = 1f, hInd = 1f;
            var s = AnimSpriteProjectSettings.Instance;
            if (s != null)
            {
                gli = s.globalLightIntensity;
                ali = s.ambientLightIntensity;
                exp = s.hdrpExposureCompensation;
                hAmb = s.hdrpAmbientMultiplier;
                hInd = s.hdrpIndirectMultiplier;
            }
            b.SetFloat("_GlobalLightIntensity", gli);
            b.SetFloat("_AmbientLightIntensity", ali);
            b.SetFloat("_HDRPExposureComp", exp);
            b.SetFloat("_HDRPAmbientMul", hAmb);
            b.SetFloat("_HDRPIndirectMul", hInd);
        }

        /// <summary>Material-direct variant of <see cref="PushLightingGlobals(MaterialPropertyBlock)"/>.</summary>
        void PushLightingGlobalsToMat(Material mat)
        {
            float gli = 1f, ali = 1f, exp = 0f, hAmb = 1f, hInd = 1f;
            var s = AnimSpriteProjectSettings.Instance;
            if (s != null)
            {
                gli = s.globalLightIntensity;
                ali = s.ambientLightIntensity;
                exp = s.hdrpExposureCompensation;
                hAmb = s.hdrpAmbientMultiplier;
                hInd = s.hdrpIndirectMultiplier;
            }
            mat.SetFloat("_GlobalLightIntensity", gli);
            mat.SetFloat("_AmbientLightIntensity", ali);
            mat.SetFloat("_HDRPExposureComp", exp);
            mat.SetFloat("_HDRPAmbientMul", hAmb);
            mat.SetFloat("_HDRPIndirectMul", hInd);
        }

        void PushAtlasUV(MaterialPropertyBlock b)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite || !sr.sprite.texture)
            {
                b.SetFloat("_MinXUV", 0);
                b.SetFloat("_MaxXUV", 1);
                b.SetFloat("_MinYUV", 0);
                b.SetFloat("_MaxYUV", 1);
                b.SetVector("_MainTex_TexelSize", new Vector4(1f, 1f, 1f, 1f));
                return;
            }

            var sprite = sr.sprite;
            var tex = sprite.texture;
            var r = sprite.textureRect;

            b.SetFloat("_MinXUV", r.x / tex.width);
            b.SetFloat("_MaxXUV", (r.x + r.width) / tex.width);
            b.SetFloat("_MinYUV", r.y / tex.height);
            b.SetFloat("_MaxYUV", (r.y + r.height) / tex.height);

            // Explicit _MainTex_TexelSize push. Unity auto-populates this on materials that have
            // _MainTex set directly, but when _MainTex is pushed through MaterialPropertyBlock
            // (as we do here) it is NOT reliably propagated on mobile drivers (GLES/Metal/Vulkan).
            // All edge effects derive sample offsets from this; if it reads zero the offset samples
            // collapse to a single pixel and outlines render as solid black. Format: (1/w, 1/h, w, h).
            b.SetVector("_MainTex_TexelSize", new Vector4(1f / tex.width, 1f / tex.height, tex.width, tex.height));
        }

        void PushOutlineSDFRect(MaterialPropertyBlock b)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return;

            var rect = sr.sprite.textureRect;
            float texW = sr.sprite.texture.width;
            float texH = sr.sprite.texture.height;
            b.SetVector("_OutlineSDFRect", new Vector4(rect.x / texW, rect.y / texH, rect.width / texW, rect.height / texH));

            // Same MPB-texel-size reliability issue as _MainTex — explicit push for SDF outlines.
            if (outlineSDFTex != null)
            {
                b.SetVector("_OutlineSDFTex_TexelSize",
                    new Vector4(1f / outlineSDFTex.width, 1f / outlineSDFTex.height, outlineSDFTex.width, outlineSDFTex.height));
            }
        }

        // Mask Packing

        public Texture2D BakePackedMasks(int size = 256)
        {
            packedMasks = CreatePackedMasksTexture(size);
            return packedMasks;
        }

        Texture2D EnsurePackedMasksForCurrentMode(bool forceEditorRebake)
        {
            if (!masksEnabled)
                return packedMasks;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return EnsureEditorPackedMasks(forceEditorRebake);
#endif

            if (packedMasks == null && HasMaskSources())
                BakePackedMasks();

            return packedMasks;
        }

        Texture2D CreatePackedMasksTexture(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var rCh = ReadChannel(maskR, size);
            var gCh = ReadChannel(maskG, size);
            var bCh = ReadChannel(maskB, size);
            var aCh = ReadChannel(maskA, size);
            var pixels = new Color32[size * size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(rCh[i], gCh[i], bCh[i], aCh[i]);
            tex.SetPixels32(pixels);
            tex.Apply();
            return tex;
        }

        bool HasMaskSources()
        {
            return maskR || maskG || maskB || maskA;
        }

        int ComputeMaskSourceHash()
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + (masksEnabled ? 1 : 0);
                h = h * 31 + (maskR ? maskR.GetInstanceID() : 0);
                h = h * 31 + (maskG ? maskG.GetInstanceID() : 0);
                h = h * 31 + (maskB ? maskB.GetInstanceID() : 0);
                h = h * 31 + (maskA ? maskA.GetInstanceID() : 0);
                return h;
            }
        }

#if UNITY_EDITOR
        Texture2D EnsureEditorPackedMasks(bool forceRebake)
        {
            if (!masksEnabled)
            {
                ReleaseEditorPackedMasks();
                return packedMasks;
            }

            if (!HasMaskSources())
            {
                ReleaseEditorPackedMasks();
                return packedMasks;
            }

            int hash = ComputeMaskSourceHash();
            if (!forceRebake && _editorPackedMasks && _editorPackedMasksHash == hash)
                return _editorPackedMasks;

            ReleaseEditorPackedMasks();

            _editorPackedMasks = CreatePackedMasksTexture(256);
            _editorPackedMasks.name = name + "_EffectMasks_Preview";
            _editorPackedMasks.hideFlags = HideFlags.HideAndDontSave;
            _editorPackedMasksHash = hash;
            return _editorPackedMasks;
        }

        void ReleaseEditorPackedMasks()
        {
            if (!_editorPackedMasks)
                return;

            DestroyImmediate(_editorPackedMasks);
            _editorPackedMasks = null;
            _editorPackedMasksHash = 0;
        }
#endif

        byte[] ReadChannel(Texture2D src, int size)
        {
            var result = new byte[size * size];
            if (!src) { for (int i = 0; i < result.Length; i++) result[i] = 255; return result; }
            var rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
            RenderTexture.active = rt;
            Graphics.Blit(src, rt);
            var tmp = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tmp.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tmp.Apply();
            var pix = tmp.GetPixels32();
            for (int i = 0; i < result.Length; i++) result[i] = pix[i].r;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            DestroyImmediate(tmp);
            return result;
        }

        // Public Api

        public void SetEffectEnabled(EffectId id, bool on)
        {
            var def = EffectRegistry.FindById(id);
            if (def == null) return;
            var fi = GetType().GetField(def.enableField, BindingFlags.Public | BindingFlags.Instance);
            if (fi != null) fi.SetValue(this, on);
            _dirty = true;
        }

        public void DeactivateAllEffects()
        {
            foreach (var def in EffectRegistry.All)
            {
                var fi = GetType().GetField(def.enableField, BindingFlags.Public | BindingFlags.Instance);
                if (fi != null) fi.SetValue(this, false);
            }
            _dirty = true;
        }

        public void ChangeShaderVariant(ShaderVariant v)
        {
            ChangeShaderVariant(v, true);
        }

        public void ChangeShaderVariant(ShaderVariant v, bool markEditorDirty)
        {
            bool wasGUI = IsGUIMode;
            _shaderVariant = v;
            MigrateDeprecatedVariant();
            bool isNowGUI = IsGUIMode;
            if (wasGUI && !isNowGUI)
                ReleaseImageMaterial();
            if (!wasGUI && isNowGUI && timeMode == ShaderTimeMode.ScaledTime && !useCustomTime)
                timeMode = ShaderTimeMode.UnscaledTime;
            SyncRendererForVariant(_shaderVariant);
            InvalidatePushStrategy();
            ClearRendererPropertyBlock();
            SetupMaterial(true);
            Push();

#if UNITY_EDITOR
            if (markEditorDirty)
                EditorMarkMaterialBindingDirty();
            AnimSpriteShaderFeatureVariantKeeper.QueueRefreshIfShaderFeature();
#endif
        }

        // PushKeywordsToMaterial moved to Groups.cs (group-based)

        // RebuildSharedKeywords moved to Groups.cs (group-based)

        public void ApplyToChildren()
        {
            var type = GetType();
            var children = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var childRend in children)
            {
                if (childRend.gameObject == gameObject) continue;
                var child = childRend.GetComponent<AnimatedSpriteShader>();
                if (!child) child = childRend.gameObject.AddComponent<AnimatedSpriteShader>();
    #if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(child, "Apply Settings To Children");
    #endif
                foreach (var def in EffectRegistry.All)
                {
                    CopyField(type, def.enableField, child);
                    if (def.hasMask) CopyField(type, def.maskField, child);
                    foreach (var p in def.parms) CopyField(type, p.field, child);
                }
                child.globalEffectFade = globalEffectFade;
                child.edgeCleanup = edgeCleanup;
                child.effectMaskEnabled = effectMaskEnabled;
                child.effectMaskTexture = effectMaskTexture;
                child.pixelArtSnapEnabled = pixelArtSnapEnabled;
                child.pixelSnapResolution = pixelSnapResolution;
                child.timeScale = timeScale;
                child.timeFPS = timeFPS;
                child.timeMode = timeMode;
                child.useCustomTime = useCustomTime;
                child.customTime = customTime;
                child._timeModeInitialized = _timeModeInitialized;
                child.outlineEffectsOnly = outlineEffectsOnly;
                child.outlineEffectWidth = outlineEffectWidth;
                child.outlineFixedWidthPx = outlineFixedWidthPx;
                child.castShadows = castShadows;
                child.receiveShadows = receiveShadows;
                child.shadowAlphaClip = shadowAlphaClip;
                child.masksEnabled = masksEnabled;
                child.maskR = maskR;
                child.maskG = maskG;
                child.maskB = maskB;
                child.maskA = maskA;
                child.packedMasks = packedMasks;
                child.outlineMode = outlineMode;
                child.outlineRings = outlineRings;
                child.outlineAA = outlineAA;
                child.MarkDirty();
            }
        }

        void CopyField(System.Type type, string fieldName, AnimatedSpriteShader target)
        {
            if (string.IsNullOrEmpty(fieldName)) return;
            var fi = type.GetField(fieldName);
            if (fi != null) fi.SetValue(target, fi.GetValue(this));
        }

        public void SetupSpriteAtlas() { MarkDirty(); }

        public void SetupPixelPerfect()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite || !sr.sprite.texture) return;
            if (!Rend) return; // GUI mode has no Renderer — pixel-perfect setup doesn't apply.
            float ppu = sr.sprite.pixelsPerUnit;
            float texelX = 1f / sr.sprite.texture.width;
            float texelY = 1f / sr.sprite.texture.height;
            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            Rend.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_SmoothPixelArtTexelSize", texelX);
            _mpb.SetFloat("_PixelSnapResolution", ppu);
            // _MainTex_TexelSize is a Vector4 (1/w, 1/h, w, h) — the shader reads .x and .y for the
            // per-axis inverse-size, not separate scalar props. Previous _MainTex_TexelSize_X / _Y
            // pushes never landed because the shader doesn't declare them.
            _mpb.SetVector("_MainTex_TexelSize",
                new Vector4(texelX, texelY, sr.sprite.texture.width, sr.sprite.texture.height));
            Rend.SetPropertyBlock(_mpb);
        }

        public void AutoDetectSpriteFlip()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr) return;
            spriteFlipX = sr.flipX;
            spriteFlipY = sr.flipY;
        }

        public float DetectWindBottomY()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return 0f;
            var tex = sr.sprite.texture;
            if (!tex.isReadable) return 0f;
            var rect = sr.sprite.textureRect;
            int startY = (int)rect.y, endY = (int)(rect.y + rect.height);
            int startX = (int)rect.x, endX = (int)(rect.x + rect.width);
            for (int y = startY; y < endY; y++)
                for (int x = startX; x < endX; x++)
                    if (tex.GetPixel(x, y).a > 0.01f)
                        return (y - rect.y) / rect.height;
            return 0f;
        }

    #if UNITY_EDITOR
        public void SetupSubPixelFiltering()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return;
            var tex = sr.sprite.texture;
            var path = UnityEditor.AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path)) return;
            var importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            if (importer == null) return;
            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
                Debug.Log($"[AnimSprite] Set {tex.name} filter to Bilinear for SmoothPixelArt.");
            }
            SetupPixelPerfect();
        }

        public Texture2D BakeOutlineSDF(int padding = 16) => BakeOutlineSDF(padding, outlineSDFAlphaThreshold);

        public Texture2D BakeOutlineSDF(int padding, float alphaThreshold)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return null;
            var tex = sr.sprite.texture;
            bool wasReadable = tex.isReadable;
            if (!wasReadable)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(tex);
                if (!string.IsNullOrEmpty(path))
                {
                    var imp = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
                    if (imp != null) { imp.isReadable = true; imp.SaveAndReimport(); }
                    else { Debug.LogWarning("[AnimSprite] Cannot auto-enable Read/Write"); return null; }
                }
                else { Debug.LogWarning("[AnimSprite] Texture has no asset path"); return null; }
            }

            var rect = sr.sprite.textureRect;
            int w = (int)rect.width + padding * 2;
            int h = (int)rect.height + padding * 2;
            bool[,] solid = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int sx = x - padding + (int)rect.x;
                    int sy = y - padding + (int)rect.y;
                    solid[x, y] = sx >= (int)rect.x && sx < (int)(rect.x + rect.width) &&
                                  sy >= (int)rect.y && sy < (int)(rect.y + rect.height) &&
                                  tex.GetPixel(sx, sy).a > alphaThreshold;
                }

            float[,] outerDist = ComputeDistanceField(w, h, solid, false);
            float[,] innerDist = ComputeDistanceField(w, h, solid, true);
            float maxOuter = 0, maxInner = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (!solid[x, y]) maxOuter = Mathf.Max(maxOuter, outerDist[x, y]);
                    else maxInner = Mathf.Max(maxInner, innerDist[x, y]);
                }

            float spread = Mathf.Max(maxOuter, maxInner, 1f);
            var sdfTex = new Texture2D(w, h, TextureFormat.RFloat, false);
            sdfTex.filterMode = FilterMode.Bilinear;
            sdfTex.wrapMode = TextureWrapMode.Clamp;
  
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float sd = solid[x, y] ? -innerDist[x, y] : outerDist[x, y];
                    float norm = 0.5f - sd / (2f * spread);
                    sdfTex.SetPixel(x, y, new Color(Mathf.Clamp01(norm), 0, 0, 1));
                }
            sdfTex.Apply();
            outlineSDFSpread = spread;

            if (!wasReadable)
            {
                var rPath = UnityEditor.AssetDatabase.GetAssetPath(tex);
                var rImp = UnityEditor.AssetImporter.GetAtPath(rPath) as UnityEditor.TextureImporter;
                if (rImp != null) { rImp.isReadable = false; rImp.SaveAndReimport(); }
            }
            Debug.Log("SDF Texture Created, If you're building for mobile devices, please check override format Android/IOS with RFloat choosen on the texture!");
            return sdfTex;
        }

        static float[,] ComputeDistanceField(int w, int h, bool[,] solid, bool invert)
        {
            float[,] dist = new float[w, h];
            int[,] nearX = new int[w, h];
            int[,] nearY = new int[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool target = invert ? !solid[x, y] : solid[x, y];
                    if (target) { dist[x, y] = 0; nearX[x, y] = x; nearY[x, y] = y; }
                    else { dist[x, y] = w * w + h * h; nearX[x, y] = -1; nearY[x, y] = -1; }
                }
            int maxStep = Mathf.Max(w, h);
            for (int step = maxStep / 2; step >= 1; step /= 2)
            {
                int[,] offsets = { { -step, 0 }, { step, 0 }, { 0, -step }, { 0, step }, { -step, -step }, { step, -step }, { -step, step }, { step, step } };
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        for (int d = 0; d < 8; d++)
                        {
                            int nx = x + offsets[d, 0], ny = y + offsets[d, 1];
                            if (nx < 0 || nx >= w || ny < 0 || ny >= h || nearX[nx, ny] < 0) continue;
                            float dx = x - nearX[nx, ny], dy = y - nearY[nx, ny];
                            float nd = dx * dx + dy * dy;
                            if (nd < dist[x, y]) { dist[x, y] = nd; nearX[x, y] = nearX[nx, ny]; nearY[x, y] = nearY[nx, ny]; }
                        }
            }
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    dist[x, y] = Mathf.Sqrt(dist[x, y]);
            return dist;
        }

        public Texture2D BakeMaterialToTexture(int w = 0, int h = 0)
        {
            // Baking requires a Renderer with a material. GUI variants route through an Image
            // component (no Renderer), so baking isn't meaningful there — fail fast.
            if (IsGUIMode || Rend == null)
            {
                Debug.LogWarning("[AnimSprite] BakeMaterialToTexture is only supported on non-GUI variants. " +
                                 "Switch to a sprite-type variant before baking.", this);
                return null;
            }
            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite || !Rend.sharedMaterial) return null;
            var tex = sr.sprite.texture;
            var sprite = sr.sprite;

            // Get atlas UV rect for this sprite
            Rect texRect = sprite.textureRect;
            float texW = tex.width;
            float texH = tex.height;
            float minU = texRect.x / texW;
            float maxU = (texRect.x + texRect.width) / texW;
            float minV = texRect.y / texH;
            float maxV = (texRect.y + texRect.height) / texH;

            if (w <= 0) w = (int)texRect.width;
            if (h <= 0) h = (int)texRect.height;

            // Create temp material copy with correct atlas UVs
            var bakeMat = new Material(Rend.sharedMaterial);
            bakeMat.SetFloat("_MinXUV", minU);
            bakeMat.SetFloat("_MaxXUV", maxU);
            bakeMat.SetFloat("_MinYUV", minV);
            bakeMat.SetFloat("_MaxYUV", maxV);
            bakeMat.SetFloat("_VertexUVExpand", vertexUVExpand);

            // Push all current effect values onto the temp material.
            // Blit ignores renderer MPBs, so baking must use the material-direct path.
            PushGlobalToMat(bakeMat);
            bakeMat.SetFloat("_AIO_GenPushSentinel", 0f);
            PushEffectsToMaterial(bakeMat);
            if (bakeMat.GetFloat("_AIO_GenPushSentinel") < 0.5f)
                PushEffectsToMat(bakeMat);

            var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);

            // Blit using atlas UV rect as source rect
            // Graphics.Blit with scale/offset maps source UVs to the atlas region
            bakeMat.SetTextureOffset("_MainTex", new Vector2(minU, minV));
            bakeMat.SetTextureScale("_MainTex", new Vector2(maxU - minU, maxV - minV));
            Graphics.Blit(tex, rt, bakeMat);

            var r = new Texture2D(w, h, TextureFormat.RGBA32, false);
            r.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            r.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            DestroyImmediate(bakeMat);
            return r;
        }
    #endif

        // Material-Direct Push
        // Used by the baking path (editor-only) AND by the GUI push path at runtime.
        // UGUI Image cannot consume MaterialPropertyBlocks, so GUI variants need these.

        void PushGlobalToMat(Material mat)
        {
            mat.SetFloat("_GlobalEffectFade", globalEffectFade);
            mat.SetFloat("_EdgeCleanup", edgeCleanup);
            mat.SetFloat("_PremulAlpha", premulAlpha ? 1f : 0f);
            mat.SetFloat("_SpriteFlipX", spriteFlipX ? 1f : 0f);
            mat.SetFloat("_SpriteFlipY", spriteFlipY ? 1f : 0f);
            mat.SetFloat("_TimeScale", timeScale);
            mat.SetFloat("_TimeFPS", timeFPS);
            mat.SetFloat("_UseCustomTime", GetUseCustomShaderTimeFlag());
            mat.SetFloat("_CustomTime", GetShaderTimeValue());
            mat.SetFloat("_OutlineEffectWidth", outlineEffectWidth);
            mat.SetFloat("_OutlineEffectsOnly_En", outlineEffectsOnly ? 1f : 0f);
            mat.SetFloat("_OutlineFixedWidthPx", outlineFixedWidthPx);
            mat.SetFloat("_CastShadows", castShadows ? 1f : 0f);
            mat.SetFloat("_ReceiveShadows", receiveShadows ? 1f : 0f);
            mat.SetFloat("_ShadowAlphaClip", shadowAlphaClip);
            mat.SetFloat("_FlipNormals", twoSidedLighting ? 1f : 0f);
            mat.SetFloat("_PixelSnapResolution", pixelSnapResolution);
            mat.SetFloat("_PixelArtSnap_En", pixelArtSnapEnabled ? 1f : 0f);
            mat.SetFloat("_VertexUVExpand", vertexUVExpand);
            mat.SetFloat("_AlphaCutoff_En", alphaCutoffEnabled ? 1f : 0f);
            mat.SetFloat("_AlphaCutoff", alphaCutoff);
            PushLightingGlobalsToMat(mat);
            mat.SetFloat("_ChromaMode", (float)chromaMode);
            mat.SetVector("_ChromaRadialCenter", chromaRadialCenter);
            mat.SetFloat("_OutlineMode", (float)outlineMode);
            mat.SetFloat("_OutlineRings", outlineRings);
            mat.SetFloat("_OutlineAA", outlineAA);
            if (outlineSDFTex) mat.SetTexture("_OutlineSDFTex", outlineSDFTex);
            mat.SetFloat("_OutlineSDFSoftness", outlineSDFSoftness);
            mat.SetFloat("_OutlineSDFPadding", 16f);
            mat.SetFloat("_OutlineSDFSpread", outlineSDFSpread);
            mat.SetFloat("_DissolveNoiseSpeed", dissolveNoiseSpeed);
            mat.SetFloat("_LUTSize", lutSize);
            if (effectMaskEnabled && effectMaskTexture) mat.SetTexture("_GlobalEffectMask", effectMaskTexture);
            mat.SetFloat("_EffectMask_En", effectMaskEnabled ? 1f : 0f);
            var activeMasks = EnsurePackedMasksForCurrentMode(false);
            if (masksEnabled && activeMasks) mat.SetTexture("_EffectMasks", activeMasks);
            mat.SetFloat("_MaskTex_En", (masksEnabled && activeMasks) ? 1f : 0f);

            // Atlas UV rect (sprite sub-region of its atlas texture).
            var sprite = GetActiveSprite();
            if (sprite && sprite.texture)
            {
                var r = sprite.textureRect;
                float texW = sprite.texture.width;
                float texH = sprite.texture.height;
                mat.SetFloat("_MinXUV", r.x / texW);
                mat.SetFloat("_MaxXUV", (r.x + r.width) / texW);
                mat.SetFloat("_MinYUV", r.y / texH);
                mat.SetFloat("_MaxYUV", (r.y + r.height) / texH);
                mat.SetFloat("_SpriteAspect", sprite.rect.width / Mathf.Max(sprite.rect.height, 1e-5f));
                mat.SetVector("_OutlineSDFRect", new Vector4(r.x / texW, r.y / texH, r.width / texW, r.height / texH));
                if (mat.GetTexture("_MainTex") != sprite.texture) mat.SetTexture("_MainTex", sprite.texture);
                // Explicit texel size — see PushAtlasUV for rationale. Unity's auto-population is
                // unreliable on mobile when the texture is pushed through MPB or reassigned at runtime.
                mat.SetVector("_MainTex_TexelSize", new Vector4(1f / texW, 1f / texH, texW, texH));
            }
            else
            {
                mat.SetFloat("_MinXUV", 0);
                mat.SetFloat("_MaxXUV", 1);
                mat.SetFloat("_MinYUV", 0);
                mat.SetFloat("_MaxYUV", 1);
                mat.SetVector("_MainTex_TexelSize", new Vector4(1f, 1f, 1f, 1f));
            }

            if (outlineSDFTex != null)
            {
                mat.SetVector("_OutlineSDFTex_TexelSize",
                    new Vector4(1f / outlineSDFTex.width, 1f / outlineSDFTex.height, outlineSDFTex.width, outlineSDFTex.height));
            }
        }

        void PushEffectsToMat(Material mat)
        {
            var all = EffectRegistry.All;
            for (int i = 0; i < all.Length; i++)
                PushEffectToMat(mat, all[i]);
        }

        void PushEffectToMat(Material mat, EffectDef def)
        {
            var type = GetType();

            var fi = type.GetField(def.enableField, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (fi == null || fi.FieldType != typeof(bool))
                return;

            bool enabled = (bool)fi.GetValue(this);
            if (def.enableShader != null)
                mat.SetFloat(def.enableShader, enabled ? 1f : 0f);

            if (!string.IsNullOrEmpty(def.maskField) && !string.IsNullOrEmpty(def.maskShader))
            {
                var mfi = type.GetField(def.maskField, System.Reflection.BindingFlags.Public | BindingFlags.Instance);
                if (mfi != null)
                {
                    var mv = mfi.GetValue(this);
                    if (mfi.FieldType == typeof(int))
                        mat.SetFloat(def.maskShader, (float)(int)mv);
                }
            }

            foreach (var p in def.parms)
            {
                var pfi = type.GetField(p.field, System.Reflection.BindingFlags.Public | BindingFlags.Instance);
                if (pfi == null)
                    continue;

                var val = pfi.GetValue(this);
                switch (p.type)
                {
                    case ParamType.Float:
                        if (val is bool fb) mat.SetFloat(p.shader, fb ? 1f : 0f);
                        else if (val != null) mat.SetFloat(p.shader, System.Convert.ToSingle(val));
                        break;
                    case ParamType.Int:
                        if (val is bool ib) mat.SetFloat(p.shader, ib ? 1f : 0f);
                        else if (val != null) mat.SetFloat(p.shader, System.Convert.ToSingle(val));
                        break;
                    case ParamType.Color:
                    case ParamType.HDRColor:
                        if (val is Color c) mat.SetColor(p.shader, c);
                        else if (val is Vector4 cv) mat.SetColor(p.shader, new Color(cv.x, cv.y, cv.z, cv.w));
                        break;
                    case ParamType.Vector4:
                        if (val is Vector4 v4) mat.SetVector(p.shader, v4);
                        else if (val is Vector3 v43) mat.SetVector(p.shader, new Vector4(v43.x, v43.y, v43.z, 0f));
                        else if (val is Vector2 v42) mat.SetVector(p.shader, new Vector4(v42.x, v42.y, 0f, 0f));
                        break;
                    case ParamType.Vector3:
                        if (val is Vector3 v3) mat.SetVector(p.shader, new Vector4(v3.x, v3.y, v3.z, 0f));
                        else if (val is Vector4 v34) mat.SetVector(p.shader, new Vector4(v34.x, v34.y, v34.z, 0f));
                        else if (val is Vector2 v32) mat.SetVector(p.shader, new Vector4(v32.x, v32.y, 0f, 0f));
                        break;
                    case ParamType.Vector2:
                        if (val is Vector2 v2) mat.SetVector(p.shader, new Vector4(v2.x, v2.y, 0f, 0f));
                        else if (val is Vector4 v24) mat.SetVector(p.shader, new Vector4(v24.x, v24.y, 0f, 0f));
                        else if (val is Vector3 v23) mat.SetVector(p.shader, new Vector4(v23.x, v23.y, 0f, 0f));
                        break;
                    case ParamType.Texture:
                        if (val is Texture tex) mat.SetTexture(p.shader, tex);
                        break;
                }
            }
        }

        void PushDirtyEffectToMaterial(Material mat, EffectDef def)
        {
            mat.SetFloat("_AIO_GenPushSentinel", 0f);
            PushEffectToMaterial(mat, def);

            if (mat.GetFloat("_AIO_GenPushSentinel") < 0.5f)
                PushEffectToMat(mat, def);
        }

        void IncrementalPushMaterialDirect(Material mat)
        {
            if (!mat)
                return;

            if (_globalsDirty)
            {
                PushStencilToMaterial(mat);
                PushGlobalToMat(mat);
                SyncRendererShadowSettings();
                _globalsDirty = false;
            }

            var all = EffectRegistry.All;
            ulong m0 = _dirtyMask0, m1 = _dirtyMask1, m2 = _dirtyMask2;

            while (m0 != 0) { int idx = TrailingZeros(m0); if (idx < all.Length) PushDirtyEffectToMaterial(mat, all[idx]); m0 &= m0 - 1; }
            while (m1 != 0) { int idx = 64 + TrailingZeros(m1); if (idx < all.Length) PushDirtyEffectToMaterial(mat, all[idx]); m1 &= m1 - 1; }
            while (m2 != 0) { int idx = 128 + TrailingZeros(m2); if (idx < all.Length) PushDirtyEffectToMaterial(mat, all[idx]); m2 &= m2 - 1; }

            ClearEffectDirtyMask();
        }

        // Pipeline-Aware Rendering

        void Push()
        {
        
            // GUI variants render through a UGUI Image component that doesn't support MPBs.
            // Delegate to the material-direct push path.
            if (IsGUIMode) { PushGUI(); return; }

            if (!Rend) return;
            bool hasActive = false;
            var all = EffectRegistry.All;
            var type = GetType();
            for (int i = 0; i < all.Length; i++)
            {
                var fi = type.GetField(all[i].enableField, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (fi != null && fi.FieldType == typeof(bool) && (bool)fi.GetValue(this)) { hasActive = true; break; }
            }
      
            var strategy = ResolvePushStrategy();

            if (PipelineRouting.PushesToMaterial(strategy))
            {
                var mat = EnsureInstancedMaterial(true);
                if (mat == null) return;

                ClearRendererPropertyBlock();
                PushMaterialDirect(mat);
                effectsActiveLastUpdate = hasActive;
                SyncRendererShadowSettings();
                FinishPushState();
                return;
            }

            if (_materialMode == MaterialMode.Instanced)
            {
                var mat = EnsureInstancedMaterial();
                if (mat == null) return;
                PushKeywordsToMaterial(mat);
                PushStencilToMaterial(mat);
                if (hasActive || effectsActiveLastUpdate)
                {
                    if (_mpb == null) _mpb = new MaterialPropertyBlock();
                    Rend.GetPropertyBlock(_mpb);
                    PushGlobal(_mpb);
                    PushEffects(_mpb);
                    Rend.SetPropertyBlock(_mpb);
                }
                else
                {
                    if (_mpb == null) _mpb = new MaterialPropertyBlock();
                    Rend.GetPropertyBlock(_mpb);
                    PushGlobal(_mpb);
                    Rend.SetPropertyBlock(_mpb);
                }
            }
            else
            {
                var mat = Rend.sharedMaterial;
                if (!mat) { AssignSharedMaterial(); mat = Rend.sharedMaterial; }
                if (!mat) return;
                RebuildSharedKeywords((int)_shaderVariant);
                if (_mpb == null) _mpb = new MaterialPropertyBlock();
                // GetPropertyBlock OVERWRITES _mpb contents with the renderer's current block, so
                // it must come first; any push calls that came before it were silently discarded.
                // (Previously, the "hasActive=false" branch called PushStencilToMPB before Get,
                // wiping stencil values on every disable-all-effects Push.)
                Rend.GetPropertyBlock(_mpb);
                PushStencilToMPB(_mpb);
                PushGlobal(_mpb);
                if (hasActive || effectsActiveLastUpdate) PushEffects(_mpb);
                Rend.SetPropertyBlock(_mpb);
            }
            effectsActiveLastUpdate = hasActive;
            SyncRendererShadowSettings();
            FinishPushState();

        }

        void FinishPushState()
        {
            _dirty = false;
            _globalsDirty = false;
            ClearEffectDirtyMask();
            InitEffectSnapshot();
            InitGlobalSnapshot();
        }

        void PushMaterialDirect(Material mat)
        {
            if (!mat)
                return;

            PushKeywordsToMaterial(mat);
            PushStencilToMaterial(mat);
            PushGlobalToMat(mat);

            mat.SetFloat("_AIO_GenPushSentinel", 0f);
            PushEffectsToMaterial(mat);
            if (mat.GetFloat("_AIO_GenPushSentinel") < 0.5f)
            {
                PushEffectsToMat(mat);
            }
        }

        void SetupMaterial(bool forceRebind = true)
        {
            // GUI mode: the Image component owns its material. We still need a shared source material
            // per shader-variant for cloning, but we don't assign to any Renderer.
            if (IsGUIMode)
            {
                ReleaseInstancedMaterial();

                var shared = GetOrCreateSharedMaterial((int)_shaderVariant);
                if (!shared) return;

                var img = GetUIImage();
                if (img == null) return;

                if (!_imageMat || _imageMat.shader != shared.shader)
                {
                    ReleaseImageMaterial();
                    _imageMat = CreateGeneratedMaterial(shared, " (Image)");
                }

                if (GetImageMaterial(img) != _imageMat)
                    SetImageMaterial(img, _imageMat);

                return;
            }

            ReleaseImageMaterial();

            if (PipelineRouting.NeedsInstancedMaterial(ResolvePushStrategy()))
            {
                EnsureInstancedMaterial(forceRebind);
            }
            else
            {
                AssignSharedMaterial(forceRebind);
            }
        }

        /// <summary>
        /// Material-direct push for UGUI Image-backed variants.
        /// Each Image instance gets its own cloned material so effect parameters don't leak between
        /// GUI elements. Global + per-effect values + stencil + keywords all go straight to the
        /// material since MPBs are unsupported by UGUI.
        ///
        /// Push path ordering:
        ///   1. PushEffectsToMaterial (generated partial) — runs only if the generator has emitted
        ///      AnimatedSpriteShader.Generated.cs. This is the fast path and matches the quality of
        ///      the MPB push.
        ///   2. If the partial body didn't run (generator not present), we detect by checking a
        ///      sentinel effect and fall back to the reflection-based PushEffectsToMat path.
        ///
        /// In practice "Full Rebuild" in Master Setup regenerates the partials so path 1 is used.
        /// Path 2 only runs if someone imports the asset without running the generator — slower but
        /// still correct.
        /// </summary>
        void PushGUI()
        {
            var img = GetUIImage();
            if (img == null) return;

            var shared = GetOrCreateSharedMaterial((int)_shaderVariant);
            if (!shared) return;

            // Ensure we're on this component's own per-Image clone so parameter changes don't leak
            // and duplicate/paste operations do not keep stale material values from the source object.
            if (!_imageMat || _imageMat.shader != shared.shader)
            {
                ReleaseImageMaterial();
                _imageMat = CreateGeneratedMaterial(shared, " (Image)");
            }

            if (GetImageMaterial(img) != _imageMat)
                SetImageMaterial(img, _imageMat);

            PushMaterialDirect(_imageMat);
            FinishPushState();
        }

        void SyncRendererShadowSettings()
        {
            var rend = Rend;
            if (!rend)
                return;

            var targetShadowMode = castShadows
                ? UnityEngine.Rendering.ShadowCastingMode.On
                : UnityEngine.Rendering.ShadowCastingMode.Off;

            if (rend.shadowCastingMode != targetShadowMode)
                rend.shadowCastingMode = targetShadowMode;

            if (rend.receiveShadows != receiveShadows)
                rend.receiveShadows = receiveShadows;
        }

        public void IncrementalPush()
        {
        
            // GUI mode: no MPB path — fall back to a full material-direct push.
            if (IsGUIMode) { PushGUI(); return; }

            if (!Rend) return;

            if (PipelineRouting.PushesToMaterial(ResolvePushStrategy()))
            {
                var mat = EnsureInstancedMaterial(false);
                if (!mat)
                    mat = EnsureInstancedMaterial(true);

                if (mat)
                {
                    ClearRendererPropertyBlock();
                    IncrementalPushMaterialDirect(mat);
                }
                return;
            }

            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            Rend.GetPropertyBlock(_mpb);
            if (_globalsDirty)
            {
                SyncRendererShadowSettings();
                PushGlobal(_mpb);
                _globalsDirty = false;
            }
            var all = EffectRegistry.All;
            ulong m0 = _dirtyMask0, m1 = _dirtyMask1, m2 = _dirtyMask2;
            while (m0 != 0) { int idx = TrailingZeros(m0); if (idx < all.Length) PushEffect(_mpb, all[idx]); m0 &= m0 - 1; }
            while (m1 != 0) { int idx = 64 + TrailingZeros(m1); if (idx < all.Length) PushEffect(_mpb, all[idx]); m1 &= m1 - 1; }
            while (m2 != 0) { int idx = 128 + TrailingZeros(m2); if (idx < all.Length) PushEffect(_mpb, all[idx]); m2 &= m2 - 1; }
            ClearEffectDirtyMask();
            Rend.SetPropertyBlock(_mpb);
        }

        // Generated partial methods (filled by AnimatedSpriteShaderGenerator)
        partial void PushGlobalToMaterial(Material mat);
        partial void PushEffectsToMaterial(Material mat);
        partial void PushEffectToMaterial(Material mat, EffectDef def);

        public static void MigratePipelineAll()
        {
            foreach (var inst in _activeInstances) inst._dirty = true;
        }

        // VELOCITY TRACKING (Trails + Motion Smear)

        public enum VelocitySource { Manual, Rigidbody2D, Transform }
        [Header("Trail / Smear Velocity")]
        public VelocitySource trailVelocitySource = VelocitySource.Manual;
        public VelocitySource smearVelocitySource = VelocitySource.Manual;

        /// <summary>Decay multiplier per second. 0.95 means velocity decays to 5% per second. Higher = longer trail persistence.</summary>
        [Range(0.8f, 0.999f)] public float velocityDecay = 0.95f;

        // Internal state
        Vector3 _lastPosition;
        Rigidbody2D _rb2d;
        bool _rb2dChecked;
        Vector4 _smoothedTrailVel;
        Vector4 _smoothedSmearVel;

        void UpdateVelocityTracking()
        {
            bool anyTrail = (trailVelocitySource != VelocitySource.Manual) &&
                            (colorfulTrailAEnabled || colorfulTrailBEnabled || colorfulTrailCEnabled);
            bool anySmear = (smearVelocitySource != VelocitySource.Manual) && motionSmearEnabled;

            if (!anyTrail && !anySmear) return;

            float dt = Time.deltaTime;
            // Convert the decay value into a smooth interpolation factor
            float smoothFactor = 1f - Mathf.Pow(velocityDecay, dt * 60f);

            if (anyTrail)
            {
                Vector4 rawVel = ComputeVelocityFromSource(trailVelocitySource);

                // Smoothly interpolate both acceleration and deceleration
                _smoothedTrailVel = Vector4.Lerp(_smoothedTrailVel, rawVel, smoothFactor);

                // FIX: Cut off the long tail. If raw input is basically 0 and smoothed is low, snap to 0.
                if (rawVel.sqrMagnitude < 0.001f && _smoothedTrailVel.sqrMagnitude < 0.01f)
                {
                    _smoothedTrailVel = Vector4.zero;
                }

                if (colorfulTrailAEnabled) trailAVelocity = _smoothedTrailVel;
                if (colorfulTrailBEnabled) trailBVelocity = _smoothedTrailVel;
                if (colorfulTrailCEnabled) trailCVelocity = _smoothedTrailVel;
            }

            if (anySmear)
            {
                Vector4 rawVel = ComputeVelocityFromSource(smearVelocitySource);

                _smoothedSmearVel = Vector4.Lerp(_smoothedSmearVel, rawVel, smoothFactor);

                if (rawVel.sqrMagnitude < 0.001f && _smoothedSmearVel.sqrMagnitude < 0.01f)
                {
                    _smoothedSmearVel = Vector4.zero;
                }

                smearDirection = _smoothedSmearVel;
            }
        }

        Vector4 ComputeVelocityFromSource(VelocitySource source)
        {
            if (source == VelocitySource.Rigidbody2D)
            {
                if (!_rb2dChecked) { _rb2d = GetComponent<Rigidbody2D>(); _rb2dChecked = true; }
                if (_rb2d) return new Vector4(_rb2d.linearVelocity.x, _rb2d.linearVelocity.y, 0, 0);
            }
            else if (source == VelocitySource.Transform)
            {
                Vector3 pos = transform.position;
                Vector3 delta = (pos - _lastPosition) / Mathf.Max(Time.deltaTime, 0.001f);
                _lastPosition = pos;
                return new Vector4(delta.x, delta.y, delta.z, 0);
            }
            return Vector4.zero;
        }

    }
    }
