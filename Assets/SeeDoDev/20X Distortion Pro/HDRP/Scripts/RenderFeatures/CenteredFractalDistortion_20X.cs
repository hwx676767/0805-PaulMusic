#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
namespace DistortionsPro_20X
{

    [Serializable, VolumeComponentMenu("20X Distortions Pro/Centered Fractal Distortion")]
    public sealed class CenteredFractalDistortion : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Intensity = new(0f, 0f, 1f, true);
        [Space]
        [Tooltip("Squeeze image to the center.")]
        public BoolParameter CenterFocus = new(false);
        public BoolParameter CubicNoise = new(false);
        [Tooltip("Distortion waves size.")]
        public NoInterpClampedFloatParameter Size = new(0.6f, 0f, 10f);
        [Tooltip("Effect speed. Use this value to animate effect.")]
        public NoInterpClampedFloatParameter Speed = new(1f, 0f, 10f);
        [Space]
        [Tooltip("Mask texture.")]
        public TextureParameter mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new DistortionsPro_20X.maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new(.15f, 0.000001f, 1f);
        [Space]
        [Tooltip("Use Global Post Processing Settings to enable or disable Post Processing in scene view or via camera setup. THIS SETTING SHOULD BE TURNED OFF FOR EFFECTS, IN CASE OF USING THEM FOR SEPARATE LAYERS")]
        public BoolParameter GlobalPostProcessingSettings = new(false);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Intensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/CenteredFractalDistortionHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int NoiseScale = Shader.PropertyToID("NoiseScale");
        static readonly int TimeFactor = Shader.PropertyToID("TimeFactor");
        static readonly int DistortStrength = Shader.PropertyToID("DistortStrength");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume CenteredFractalDistortion is unable to load.");
        }
        float T;

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (mask.value != null)
            {
                m_Material.SetFloat(FadeMultiplier, 1);
                m_Material.SetFloat(MaskThreshold, maskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, mask.value);
                ParamSwitch(m_Material, maskChannel.value == DistortionsPro_20X.maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(FadeMultiplier, 0);
            }

            T += Time.deltaTime * Speed.value;
            m_Material.SetFloat(NoiseScale, Size.value);
            m_Material.SetFloat(TimeFactor, T);
            m_Material.SetFloat(DistortStrength, Intensity.value);
            ParamSwitch(m_Material, CenterFocus.value, "FOCUS_CENTER");

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
