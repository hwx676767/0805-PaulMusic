#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/SignalShock")]
    public sealed class SignalShock : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("Effect amount")]
        public NoInterpClampedFloatParameter amount = new NoInterpClampedFloatParameter(0.2f, 0f, 1f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(.9f, 0f, 1f);
        [Tooltip("Glitch cell size")]
        public NoInterpVector2Parameter size = new NoInterpVector2Parameter(new Vector2(1f, 1f));
        [Tooltip("Enables light filter to make effect softer")]
        public BoolParameter light = new BoolParameter(false);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && Fade.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/SignalShock";
        static readonly int fade = Shader.PropertyToID("fade");
        static readonly int randAmount = Shader.PropertyToID("randAmount");
        static readonly int M_amount = Shader.PropertyToID("amount");
        static readonly int M_size = Shader.PropertyToID("size");
        static readonly int M_light = Shader.PropertyToID("light");
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

            m_Material.SetFloat(randAmount, 1 - randomActivateAmount.value);
            m_Material.SetFloat(fade, Fade.value);
            m_Material.SetFloat(M_amount, amount.value);
            m_Material.SetVector(M_size, size.value);
            m_Material.SetFloat(M_light, light.value ? 1 : 0);
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