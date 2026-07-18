#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/Digital Glitch")]
    public sealed class DigitalGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("Glitch lines width.")]
        public NoInterpClampedFloatParameter LinesWidth = new NoInterpClampedFloatParameter(1f, 0.1f, 20f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("Offset on X axis.")]
        public NoInterpClampedFloatParameter Offset = new NoInterpClampedFloatParameter(1f, 0f, 13f);
        [Space]
        [Tooltip("Time.unscaledTime .")]
        public BoolParameter unscaledTime = new BoolParameter(false);
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

        const string kShaderName = "GlitchLabPro/DigitalGlitch";
        static readonly int _TimeX = Shader.PropertyToID("_TimeX");
        static readonly int M_Amount = Shader.PropertyToID("Amount");
        static readonly int M_Offset = Shader.PropertyToID("Offset1");
        static readonly int resM = Shader.PropertyToID("resM");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        private float TimeX = 1.0f;



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

            if (unscaledTime.value)
                TimeX += Time.unscaledDeltaTime;
            else
                TimeX += Time.deltaTime;

            if (TimeX > 100) TimeX = 0;

            m_Material.SetFloat(_TimeX, TimeX * Speed.value);
            m_Material.SetFloat(M_Amount, 1 - Amount.value);
            m_Material.SetFloat(M_Offset, Offset.value);
            m_Material.SetFloat(resM, LinesWidth.value);
            m_Material.SetFloat("randAmount", randAmount.value);

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