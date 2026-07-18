#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/RGBDesyncGlitch")]
    public sealed class RGBDesyncGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
        public BoolParameter stop = new BoolParameter(false);
        [Tooltip("Effect fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0, 1f);
        [Tooltip("Glitch Lines width")]
        public NoInterpClampedFloatParameter linesWidth = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Glitch Lines amount")]
        public NoInterpClampedFloatParameter LinesAmount = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Glitch offset")]
        public NoInterpClampedFloatParameter offset = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(.9f, 0, 1);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(3f, 0, 20);
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

        const string kShaderName = "GlitchLabPro/RGBDesyncGlitch";
        static readonly int SPEED = Shader.PropertyToID("SPEED");
        static readonly int randAmount = Shader.PropertyToID("randAmount");

        static readonly int fade = Shader.PropertyToID("fade");
        static readonly int val3 = Shader.PropertyToID("val3");
        static readonly int val2 = Shader.PropertyToID("val2");
        static readonly int val1 = Shader.PropertyToID("val1");
        static readonly int M_stop = Shader.PropertyToID("stop");
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

            m_Material.SetFloat(val1, linesWidth.value);
            m_Material.SetFloat(val2, LinesAmount.value);
            m_Material.SetFloat(val3, offset.value);
            m_Material.SetFloat(fade, Fade.value);
            m_Material.SetInt(M_stop, stop.value ? 1 : 0);

            m_Material.SetFloat(SPEED, speed.value);
            m_Material.SetFloat(randAmount, 1 - randomActivateAmount.value);
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