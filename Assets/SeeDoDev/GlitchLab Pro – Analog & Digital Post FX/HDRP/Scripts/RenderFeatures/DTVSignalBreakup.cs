#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/DTVSignalBreakup")]
    public sealed class DTVSignalBreakup : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0, 1f);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(.7f, 0, 44f);
        [Tooltip("Dropout Intensity.")]
        public ClampedFloatParameter dropoutIntensity = new ClampedFloatParameter(0f, 0, 6f);
        public ClampedFloatParameter DisplaceIntesnsity = new ClampedFloatParameter(0f, 0, 6f);
        public ClampedFloatParameter RGBSplit = new ClampedFloatParameter(0f, 0, 6f);
        [Tooltip("Interlace Intesnsity.")]
        public ClampedFloatParameter interlaceIntesnsity = new ClampedFloatParameter(0f, 0, 6);
        public ClampedFloatParameter InterlaceScale = new ClampedFloatParameter(1f, 0, 900f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && Intensity.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/DTVSignalBreakup";
        static readonly int M_dropoutIntensity = Shader.PropertyToID("dropoutIntensity");
        static readonly int M_interlaceIntesnsity = Shader.PropertyToID("interlaceIntesnsity");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");



        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume AnalogChromaGlitch is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat(M_interlaceIntesnsity, interlaceIntesnsity.value);
            m_Material.SetFloat(M_dropoutIntensity, dropoutIntensity.value);
            m_Material.SetFloat("Intensity", Intensity.value);
            m_Material.SetFloat("_RGBSplit", RGBSplit.value);
            m_Material.SetFloat("_Speed", Speed.value);
            m_Material.SetFloat("_InterlaceScale", InterlaceScale.value);
            m_Material.SetFloat("_DisplaceIntesnsity", DisplaceIntesnsity.value);

            if (mask.value != null)
            {
                m_Material.SetFloat(_FadeMultiplier, 1);
                m_Material.SetFloat("_adj", maskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, mask.value);
                ParamSwitch(m_Material, maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(_FadeMultiplier, 0);
            }

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