#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/DataGlitch")]
    public sealed class DataGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Amount = new ClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 50f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("block size (higher value = smaller blocks).")]
        public ClampedFloatParameter blockSize = new ClampedFloatParameter(0f, 0f, 505f);

        [Tooltip("maximum color shift on X axis.")]
        public ClampedFloatParameter maxOffsetX = new ClampedFloatParameter(0f, 0f, 25f);
        [Tooltip("maximum color shift on Y axis.")]
        public ClampedFloatParameter maxOffsetY = new ClampedFloatParameter(0f, 0f, 25f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => m_Material != null && Amount.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/DataGlitch";
        static readonly int M_maxOffsetY = Shader.PropertyToID("maxOffsetY");
        static readonly int M_maxOffsetX = Shader.PropertyToID("maxOffsetX");
        static readonly int M_blockSize = Shader.PropertyToID("blockSize");
        static readonly int M_speed = Shader.PropertyToID("speed");
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
            m_Material.SetFloat(M_speed, speed.value);
            m_Material.SetFloat("randAmount", randAmount.value);
            m_Material.SetFloat("amount", Amount.value);
            m_Material.SetFloat(M_blockSize, blockSize.value);
            m_Material.SetFloat(M_maxOffsetX, maxOffsetX.value);
            m_Material.SetFloat(M_maxOffsetY, maxOffsetY.value);


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