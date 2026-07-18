#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    public enum Pattern_variant
    {
        displace,
        standard,
        soft
    }
    [Serializable]
    public sealed class dist_variant_Parameter : VolumeParameter<Pattern_variant> { };

    [Serializable, VolumeComponentMenu("20X Distortions Pro/Voronoi Patterns")]
    public sealed class VoronoiPatterns_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect transparancy. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0, 0, 1, true);
        [Space]
        public dist_variant_Parameter DisplaceMode = new dist_variant_Parameter();
        [Tooltip("Effect cells size")]
        public NoInterpClampedFloatParameter Size = new NoInterpClampedFloatParameter(64f, 0f, 505f);
        [Tooltip("How thik will be cells borders. ")]
        public NoInterpClampedFloatParameter Thikness = new NoInterpClampedFloatParameter(1f, 0f, 10f);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(1f, 0f, 20f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new DistortionsPro_20X.maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Fade.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/VoronoiPatternsHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int CellScale = Shader.PropertyToID("CellSize");
        static readonly int FlowSpeed = Shader.PropertyToID("FlowRate");
        static readonly int BlendStrength = Shader.PropertyToID("BlendFactor");
        static readonly int PatternVariant = Shader.PropertyToID("DisplayMode");
        static readonly int RidgeThickness = Shader.PropertyToID("Thickness");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume VoronoiPatterns is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (mask.value != null)
            {
                m_Material.SetFloat(FadeMultiplier, 1);
                m_Material.SetFloat(MaskThreshold, maskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, mask.value);
                ParamSwitch(m_Material, maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(FadeMultiplier, 0);
            }

            m_Material.SetFloat(CellScale, Size.value);
            m_Material.SetFloat(RidgeThickness, Thikness.value);
            m_Material.SetFloat(FlowSpeed, Speed.value);
            m_Material.SetFloat(BlendStrength, Fade.value);
            m_Material.SetInt(PatternVariant, (int)DisplaceMode.value);

            cmd.Blit(source, destination, m_Material, 0);
        }
        private void ParamSwitch(Material mat, bool paramValue, string paramName)
        {
            if (paramValue) mat.EnableKeyword(paramName);
            else mat.DisableKeyword(paramName);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
#endif
