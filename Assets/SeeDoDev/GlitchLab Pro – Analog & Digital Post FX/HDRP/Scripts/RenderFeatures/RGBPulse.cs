#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/RGBPulse")]
    public sealed class RGBPulse : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Range(0f, 2f), Tooltip("effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 2f);
        [Range(0f, 21f), Tooltip("Glitch periodic interval in seconds.")]
        public NoInterpClampedFloatParameter interval = new NoInterpClampedFloatParameter(1f, 0f, 21f);
        [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
        public NoInterpClampedFloatParameter rate = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 50f), Tooltip("color shift.")]
        public NoInterpClampedFloatParameter RGBSplit = new NoInterpClampedFloatParameter(1f, 0f, 50f);
        [Range(0f, 1f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip(" true - Enables ability to adjust resolution. false - screen resolution.")]
        public BoolParameter customResolution = new BoolParameter(false);
        [Tooltip("jitter resolution.")]
        public NoInterpVector2Parameter resolution = new NoInterpVector2Parameter(new Vector2(640f, 480f));
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

        const string kShaderName = "GlitchLabPro/RGBPulse";
        static readonly int _GlitchInterval = Shader.PropertyToID("_GlitchInterval");
        static readonly int _GlitchRate = Shader.PropertyToID("_GlitchRate");
        static readonly int _RGBSplit = Shader.PropertyToID("_RGBSplit");
        static readonly int _Speed = Shader.PropertyToID("_Speed");
        static readonly int _Amount = Shader.PropertyToID("_Amount");
        static readonly int _Res = Shader.PropertyToID("_Res");

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

            m_Material.SetFloat(_GlitchInterval, interval.value);
            m_Material.SetFloat(_GlitchRate, 1 - rate.value);
            m_Material.SetFloat(_RGBSplit, RGBSplit.value);
            m_Material.SetFloat(_Speed, speed.value);
            m_Material.SetFloat(_Amount, Amount.value);

            if (customResolution.value)
                m_Material.SetVector(_Res, resolution.value);
            else
                m_Material.SetVector(_Res, new Vector2(Screen.width, Screen.height));

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