using UnityEngine;



namespace FlexiSprite
{
    public partial class AnimatedSpriteShader
    {
        // Gradient (ColorExt)
        public bool gradientEnabled;
        public Color gradientColorA = Color.white;
        public Color gradientColorB = Color.black;
        [Range(-3.15f, 3.15f)] public float gradientAngle = 1.5708f;
        [Range(0, 1)] public float gradientFade = 1f;
        [Range(0, 4)] public int gradientMaskCh;
        public Gradient gradientGradient = new Gradient();
        public bool gradientUseGradientEditor;
        [System.NonSerialized, HideInInspector] public Texture2D gradientBakedTexture;
        [Range(0, 1)] public int gradientBlendMode; // 0=additive, 1=multiply

        // Radial Gradient (ColorExt)
        public bool radialGradientEnabled;
        public Color radialGradCenterColor = Color.white;
        public Color radialGradEdgeColor = Color.black;
        public Vector4 radialGradCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0.01f, 2f)] public float radialGradRadius = 0.5f;
        [Range(0, 1)] public float radialGradFade = 1f;
        [Range(0, 4)] public int radialGradMaskCh;
        public Gradient radialGradGradient = new Gradient();
        public bool radialGradUseGradientEditor;
        [System.NonSerialized, HideInInspector] public Texture2D radialGradBakedTexture;
        [Range(0, 1)] public int radialGradBlendMode; // 0=additive, 1=multiply

        // Color Ramp (ColorExt)
        public bool colorRampEnabled;
        public Texture2D colorRampTexture;
        [Range(0, 2)] public int colorRampMode;
        [Range(0, 3)] public float colorRampIntensity = 1f;
        public Color colorRampColorA = Color.black;
        public Color colorRampColorB = Color.white;
        [Range(0, 1)] public float colorRampFade = 1f;
        [Range(0, 4)] public int colorRampMaskCh;
        public Gradient colorRampGradient = new Gradient();
        [System.NonSerialized, HideInInspector] public Texture2D colorRampBakedGradient;

        // Burn Direction (Dissolve)
        public bool burnDirectionEnabled;
        [Range(-3.15f, 3.15f)] public float burnDirAngle;
        [Range(0, 1)] public float burnDirAmount;
        [Range(0, 0.3f)] public float burnDirEdgeWidth = 0.05f;
        [ColorUsage(true, true)] public Color burnDirFireColor = new Color(1f, 0.5f, 0f, 1f);
        public Color burnDirCharcoalColor = new Color(0.1f, 0.08f, 0.05f, 1f);
        [Range(0, 0.3f)] public float burnDirCharcoalWidth = 0.08f;
        [Range(0, 30f)] public float burnDirNoiseScale = 8f;
        [Range(0, 4)] public int burnDirMaskCh;

        // Improved Enchanted extras (VFX — extends existing enchanted fields)
        [Range(0, 1)] public float enchantedColorVariation = 0.3f;
        [Range(0, 1)] public float enchantedSizeMix = 0.5f;
        [Range(0, 1)] public float enchantedShimmer = 0.4f;

        // Global Smoke (VFX — screen space)
        public bool globalSmokeEnabled;
        public Color globalSmokeColor = new Color(0.3f, 0.3f, 0.35f, 1);
        [Range(0, 20f)] public float globalSmokeDensity = 3f;
        [Range(-10, 10)] public float globalSmokeSpeed = 0.5f;
        [Range(0, 1)] public float globalSmokeOpacity = 0.3f;
        public Vector4 globalSmokeDirection = new Vector4(1, 0.3f, 0, 0);
        [Range(0, 4)] public int globalSmokeMaskCh;

        // Holographic Card (VFX)
        public bool holographicCardEnabled;
        [Range(0, 10)] public float holoCardDensity = 3f;
        [Range(-5, 5)] public float holoCardSpeed = 0.5f;
        [Range(0, 1)] public float holoCardSaturation = 0.8f;
        [Range(0, 2)] public float holoCardBrightness = 1f;
        [Range(0, 5)] public float holoCardFresnelPower = 2f;
        [Range(0, 1)] public float holoCardFresnelShift = 0.5f;
        [Range(0, 200)] public float holoCardScanDensity = 50f;
        [Range(-5, 5)] public float holoCardScanSpeed = 1f;
        [Range(0, 1)] public float holoCardScanIntensity = 0.2f;
        [Range(0, 1)] public float holoCardFade = 1f;
        [Range(0, 4)] public int holoCardMaskCh;

        // Wind Motion Vector (UV — flag on Wind)
        public bool windMotionVectorEnabled;

        // Cloud Shadows (VFX — world space)
        public bool cloudShadowsEnabled;
        [Range(0.01f, 5f)] public float cloudShadowScale = 0.5f;
        public Vector4 cloudShadowSpeed = new Vector4(0.1f, 0.05f, 0, 0);
        [Range(0, 1)] public float cloudShadowDarkness = 0.3f;
        [Range(0, 1)] public float cloudShadowThreshold = 0.4f;
        [Range(0, 1)] public float cloudShadowSoftness = 0.3f;
        [Range(0, 1)] public float cloudShadowFade = 1f;
        [Range(0, 4)] public int cloudShadowMaskCh;

        // Trail A: Speed Color (VFX)
        public bool colorfulTrailAEnabled;
        public Vector4 trailAVelocity;
        public Color trailASlowColor = Color.cyan;
        [ColorUsage(true, true)] public Color trailAFastColor = new Color(1, 0.3f, 0.1f, 1);
        [Range(0, 10)] public float trailAMaxSpeed = 5f;
        [Range(0.1f, 5)] public float trailACurvePower = 1f;
        [Range(0, 3)] public float trailAIntensity = 1f;
        [Range(0, 4)] public int trailAMaskCh;

        // Trail B: Afterimage (VFX)
        public bool colorfulTrailBEnabled;
        public Vector4 trailBVelocity;
        [ColorUsage(true, true)] public Color trailBColor = Color.white;
        [Range(0, 10)] public float trailBMaxSpeed = 5f;
        [Range(0.1f, 5)] public float trailBCurvePower = 1f;
        [Range(0, 3)] public float trailBIntensity = 1f;
        [Range(2, 16)] public int trailBSegments = 5;
        [Range(0, 1)] public float trailBFadeOut = 0.7f;
        [Range(0, 4)] public int trailBMaskCh;

        // Trail C: Rainbow (VFX)
        public bool colorfulTrailCEnabled;
        public Vector4 trailCVelocity;
        [ColorUsage(true, true)] public Color trailCColorA = Color.cyan;
        [ColorUsage(true, true)] public Color trailCColorB = new Color(1, 0, 1, 1);
        [Range(0, 10)] public float trailCMaxSpeed = 5f;
        [Range(0.1f, 5)] public float trailCCurvePower = 1f;
        [Range(0, 3)] public float trailCIntensity = 1f;
        [Range(0, 10)] public float trailCRainbowSpeed = 1f;
        [Range(0, 4)] public int trailCMaskCh;

        // Fake Lighting (Edge)
        public bool fakeLightingEnabled;
        public Vector4 fakeLightDirection = new Vector4(0.5f, 0.7f, 1f, 0);
        [ColorUsage(true, true)] public Color fakeLightColor = Color.white;
        [Range(0, 3)] public float fakeLightIntensity = 1f;
        [Range(0, 1)] public float fakeLightAmbient = 0.3f;
        [Range(0.001f, 0.05f)] public float fakeLightNormalDist = 0.005f;
        [Range(0.1f, 2)] public float fakeLightNormalZ = 1f;
        [Range(0, 5)] public float fakeLightSpecPower = 2f;
        [Range(0, 3)] public float fakeLightSpecIntensity = 0.5f;
        [Range(0, 1)] public float fakeLightFade = 1f;
        [Range(0, 4)] public int fakeLightMaskCh;

        // Dash Shader (Dissolve)
        public bool dashShaderEnabled;
        [Range(0, 1)] public int dashMode;
        [Range(0, 1)] public float dashAmount;
        [Range(0, 1)] public float dashY0;
        [Range(0, 1)] public float dashY1 = 1f;
        [Range(0, 0.3f)] public float dashSmoothness = 0.1f;
        public Color dashSmokeColor = new Color(0.4f, 0.4f, 0.5f, 1);
        [Range(0, 0.3f)] public float dashSmokeWidth = 0.15f;
        [Range(1, 20)] public float dashSmokeDensity = 5f;
        [Range(-10, 10)] public float dashSmokeSpeed = 3f;
        [Range(0, 4)] public int dashMaskCh;

        // Charging Ability (VFX)
        public bool chargingAbilityEnabled;
        [ColorUsage(true, true)] public Color chargeColor = new Color(0.3f, 0.5f, 1f, 1);
        public Vector4 chargeCenter = new Vector4(0.5f, 0.5f, 0, 0);
        [Range(0, 2)] public float chargeRadius = 0.4f;
        [Range(0, 0.3f)] public float chargeWidth = 0.1f;
        [Range(0, 20)] public float chargeSpeed = 5f;
        [Range(1, 50)] public float chargeFrequency = 10f;
        [Range(0, 5)] public float chargeDensity = 5f;
        [Range(0, 3)] public float chargeIntensity = 1f;
        [Range(1, 12)] public float chargeLines = 6f;
        [Range(0, 1)] public float chargeFade = 1f;
        [Range(0, 4)] public int chargeMaskCh;

        // Blueprint (Game)
        public bool blueprintEnabled;
        public Color blueprintBgColor = new Color(0.05f, 0.1f, 0.3f, 1);
        public Color blueprintLineColor = new Color(0.6f, 0.8f, 1f, 1);
        public Color blueprintGridColor = new Color(0.15f, 0.25f, 0.5f, 1);
        [Range(0.001f, 0.05f)] public float blueprintEdgeSize = 0.005f;
        [Range(0.5f, 5f)] public float blueprintLineWidth = 1.2f;
        [Range(0, 3)] public float blueprintLineIntensity = 1f;
        [Range(1, 50)] public float blueprintGridDensity = 10f;
        [Range(0, 1)] public float blueprintGridOpacity = 0.3f;
        [Range(0, 1)] public float blueprintFade = 1f;
        [Range(0, 4)] public int blueprintMaskCh;

        // Directional Outline (Edge) — replaces Rim Light
        public bool dirOutlineEnabled;
        [ColorUsage(true, true)] public Color dirOutlineColor = Color.white;
        [Range(0, 0.15f)] public float dirOutlineWidth = 0.03f;
        [Range(0, 10)] public float dirOutlineIntensity = 1f;
        public Vector2 dirOutlineDirection = new Vector2(1, 0);
        [Range(0, 1)] public float dirOutlineSoftness = 0.5f;
        [Range(0, 4)] public int dirOutlineMaskCh;

        // UV Bend Point (UV)
        public bool uvBendPointEnabled;
        public Vector4 bendLocalPoint = new Vector4(0.5f, 0.5f, 0, 0);
        public Vector4 bendWorldPoint = Vector4.zero;
        public bool bendUseWorldSpace;
        [Range(0.01f, 5f)] public float bendWorldScale = 1f;
        [Range(0.01f, 2f)] public float bendRadius = 0.5f;
        [Range(-1f, 1f)] public float bendAmount = 0.1f;
        [Range(0.1f, 5f)] public float bendFalloff = 2f;
        public bool bendInvert;
        [Range(0, 4)] public int bendMaskCh;

        // Cloud Shadows overlay-only mode
        public bool cloudShadowOverlayOnly;

        // Global Smoke overlay-only mode
        public bool globalSmokeOverlayOnly;


        /// <summary>
        /// Bakes a Unity Gradient to a 256x1 Texture2D for use as a color ramp.
        /// </summary>
        public static Texture2D BakeGradientToTexture(Gradient gradient, Texture2D existing = null)
        {
            const int width = 256;
            if (existing == null || existing.width != width || existing.height != 1)
            {
                existing = new Texture2D(width, 1, TextureFormat.RGBA32, false)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    hideFlags = HideFlags.HideAndDontSave
                };
            }
            var pixels = new Color[width];
            for (int i = 0; i < width; i++)
                pixels[i] = gradient.Evaluate((float)i / (width - 1));
            existing.SetPixels(pixels);
            existing.Apply();
            return existing;
        }

    }
}
