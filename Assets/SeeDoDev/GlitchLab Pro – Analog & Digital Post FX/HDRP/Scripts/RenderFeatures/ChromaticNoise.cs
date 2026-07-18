#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/Chromatic Noise")]
    public sealed class ChromaticNoise : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Header("Random Seed")]
        [Tooltip("seed x")]
        public NoInterpClampedFloatParameter x = new NoInterpClampedFloatParameter(127.1f, -2f, 200f);
        [Tooltip("seed y")]
        public NoInterpClampedFloatParameter y = new NoInterpClampedFloatParameter(43758.5453123f, -2f, 10002f);
        [Tooltip("seed z")]
        public NoInterpClampedFloatParameter z = new NoInterpClampedFloatParameter(311.7f, -2f, 200f);
        [Space]
        [Tooltip("Effect fade.")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("Effect amount")]
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 2f);
        [Tooltip("Stretch on X axes")]
        public ClampedFloatParameter stretch = new ClampedFloatParameter(0f, 0f, 4f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(0.5f, 0f, 1f);
        [Space]
        [Tooltip("Red.")]
        public NoInterpClampedFloatParameter rMultiplier = new NoInterpClampedFloatParameter(1f, -1f, 2f);
        [Tooltip("Green.")]
        public NoInterpClampedFloatParameter gMultiplier = new NoInterpClampedFloatParameter(1f, -1f, 2f);
        [Tooltip("Blue.")]
        public NoInterpClampedFloatParameter bMultiplier = new NoInterpClampedFloatParameter(0f, -1f, 2f);
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

        const string kShaderName = "GlitchLabPro/ChromaticNoise";
        static readonly int Strength = Shader.PropertyToID("Strength");
        static readonly int M_x = Shader.PropertyToID("x");
        static readonly int M_y = Shader.PropertyToID("y");
        static readonly int angleY = Shader.PropertyToID("angleY");
        static readonly int Stretch = Shader.PropertyToID("Stretch");
        static readonly int Speed = Shader.PropertyToID("Speed");
        static readonly int mR = Shader.PropertyToID("mR");
        static readonly int mG = Shader.PropertyToID("mG");
        static readonly int mB = Shader.PropertyToID("mB");
        static readonly int M_Fade = Shader.PropertyToID("Fade");
        static readonly int m_T = Shader.PropertyToID("T");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");

        private float T;


        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume AnalogChromaGlitch is unable to load.");
        }
        float _time;

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            T += Time.deltaTime;


            m_Material.SetFloat(Strength, amount.value);

            m_Material.SetFloat(M_x, x.value);
            m_Material.SetFloat(M_y, y.value);
            m_Material.SetFloat(angleY, z.value);
            m_Material.SetFloat(Stretch, stretch.value);
            m_Material.SetFloat(Speed, speed.value);

            m_Material.SetFloat(mR, rMultiplier.value);
            m_Material.SetFloat(mG, gMultiplier.value);
            m_Material.SetFloat(mB, bMultiplier.value);
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
            m_Material.SetFloat(M_Fade, Fade.value);
            m_Material.SetFloat(m_T, T);

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