#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/MosaicGlitch")]
    public sealed class MosaicGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect Fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0, 1f);
        [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
        public BoolParameter stop = new BoolParameter(false);
        [Tooltip("Effect strength")]
        public NoInterpClampedFloatParameter strength = new NoInterpClampedFloatParameter(0.03f, 0, 0.5f);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(16f, 0, 100f);
        [Tooltip("Effect size")]
        public NoInterpClampedFloatParameter size = new NoInterpClampedFloatParameter(1f, 0, 10f);
        [Tooltip("RGBSplit")]
        public NoInterpClampedFloatParameter RGBSplit = new NoInterpClampedFloatParameter(1f, 0, 2f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(1f, 0, 1);
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

        const string kShaderName = "GlitchLabPro/MosaicGlitch";
        static readonly int Strength = Shader.PropertyToID("Strength");
        static readonly int Size1 = Shader.PropertyToID("Size1");
        static readonly int Speed = Shader.PropertyToID("Speed");
        static readonly int M_Fade = Shader.PropertyToID("Fade");
        static readonly int M_stop = Shader.PropertyToID("stop");
        static readonly int randAmount = Shader.PropertyToID("randAmount");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _rgb_split = Shader.PropertyToID("rgb_split");
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

            m_Material.SetFloat(Strength, strength.value);
            m_Material.SetFloat(Speed, speed.value);
            m_Material.SetFloat(Size1, size.value);
            m_Material.SetFloat(M_Fade, Fade.value);
            m_Material.SetInt(M_stop, stop.value ? 1 : 0);
            m_Material.SetFloat(randAmount, 1 - randomActivateAmount.value);
            m_Material.SetFloat(_rgb_split, RGBSplit.value);
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