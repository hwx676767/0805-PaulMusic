#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Sector Kaleido Distort")]
    public sealed class SectorKaleidoDistort_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect transparancy. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Fade = new(0f, 0f, 1f, true);
        public NoInterpClampedFloatParameter Intensity = new(1f, 0f, 1f);
        [Space]
        public NoInterpClampedFloatParameter InnerRadius = new(0.1f, 0f, 1f);
        public NoInterpClampedFloatParameter OuterRadius = new(0.49f, 0f, 1f);
        public NoInterpClampedFloatParameter Fill = new(0.9f, -1f, 1f);
        [Tooltip("Fill areas out of circle with black color.")]
        public BoolParameter Black = new(true);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new DistortionsPro_20X.maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Intensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/SectorKaleidoDistortHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int M_InnerRadius = Shader.PropertyToID("InnerRadius");
        static readonly int M_OuterRadius = Shader.PropertyToID("OuterRadius");
        static readonly int BlendFactor = Shader.PropertyToID("BlendFactor");
        static readonly int SectorIntensity = Shader.PropertyToID("SectorIntensity");
        static readonly int FillRatio = Shader.PropertyToID("FillRatio");
        static readonly int AddBlack = Shader.PropertyToID("AddBlack");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume SectorKaleidoDistort is unable to load.");
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

            m_Material.SetFloat(M_InnerRadius, InnerRadius.value);
            m_Material.SetFloat(M_OuterRadius, OuterRadius.value);
            m_Material.SetFloat(BlendFactor, Fade.value);
            m_Material.SetFloat(SectorIntensity, Intensity.value);
            m_Material.SetFloat(FillRatio, Fill.value);
            m_Material.SetInt(AddBlack, Black.value ? 0 : 1);

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
