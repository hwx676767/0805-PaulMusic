#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/SoftRGBGlitch")]
    public sealed class SoftRGBGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect Fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0, 1f);
        [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
        public BoolParameter stop = new BoolParameter(false);
        [Tooltip("Higher values make RGB shifts rarer and more “spiky”; lower values make the effect more frequent and even.")]
        public NoInterpClampedFloatParameter SpikeExponent = new NoInterpClampedFloatParameter(16f, 0, 16f);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(0.11f, 0, 1f);
        [Tooltip("Effect amplitude")]
        public NoInterpClampedFloatParameter amplitude = new NoInterpClampedFloatParameter(0.1f, 0, 1f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(.9f, 0, 1);
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

        const string kShaderName = "GlitchLabPro/SoftRGBGlitch";
        static readonly int AMPLITUDE = Shader.PropertyToID("AMPLITUDE");
        static readonly int ShiftSeed = Shader.PropertyToID("SpikeExponent");
        static readonly int Speed = Shader.PropertyToID("SPEED");
        static readonly int M_Fade = Shader.PropertyToID("fade");
        static readonly int M_stop = Shader.PropertyToID("stop");
        static readonly int randAmount = Shader.PropertyToID("randAmount");


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

            m_Material.SetFloat(ShiftSeed, SpikeExponent.value);
            m_Material.SetFloat(Speed, speed.value);
            m_Material.SetFloat(AMPLITUDE, amplitude.value);
            m_Material.SetFloat(M_Fade, Fade.value);
            m_Material.SetInt(M_stop, stop.value ? 1 : 0);
            m_Material.SetFloat(randAmount, 1 - randomActivateAmount.value);

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