#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Spiral Slice Kaleidoscope")]
    public sealed class SpiralSliceKaleidoscope_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect transparancy. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Fade = new(0f, 0f, 1f, true);
        [Space]
        [Tooltip("Amount of images")]
        public NoInterpClampedIntParameter Images = new(5, 1, 20);
        public NoInterpClampedFloatParameter Scale = new(1f, 0f, 2f);
        public NoInterpClampedFloatParameter Rotation = new(45f, 0f, 180f);
        public NoInterpClampedFloatParameter Offset = new(1f, 0f, 2f);
        public NoInterpClampedFloatParameter NoiseWarpStrength = new(0.5f, 0f, 1f);
        public BoolParameter Black = new(false);
        public NoInterpClampedFloatParameter Speed = new(0.5f, 0f, 2f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new DistortionsPro_20X.maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Fade.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/SpiralSliceKaleidoscopeHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int IterationCount = Shader.PropertyToID("IterationCount");
        static readonly int SpiralScale = Shader.PropertyToID("SpiralScale");
        static readonly int SpiralRotation = Shader.PropertyToID("SpiralRotation");
        static readonly int SpiralOffset = Shader.PropertyToID("SpiralOffset");
        static readonly int M_NoiseWarpStrength = Shader.PropertyToID("NoiseWarpStrength");
        static readonly int BlendFactor = Shader.PropertyToID("BlendFactor");
        static readonly int AnimationSpeed = Shader.PropertyToID("AnimationSpeed");
        static readonly int BaseDarkening = Shader.PropertyToID("BaseDarkening");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume SpiralSliceKaleidoscope is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (mask.value != null)
            {
                m_Material.SetFloat(FadeMultiplier, 1);
                m_Material.SetFloat(MaskThreshold, maskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, mask.value);
                ParamSwitch(m_Material, maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(FadeMultiplier, 0);
            }

            m_Material.SetInt(IterationCount, Images.value);
            m_Material.SetFloat(SpiralScale, Scale.value);
            m_Material.SetFloat(SpiralRotation, Rotation.value);
            m_Material.SetFloat(SpiralOffset, Offset.value);
            m_Material.SetFloat(M_NoiseWarpStrength, NoiseWarpStrength.value);
            m_Material.SetFloat(BlendFactor, Fade.value);
            m_Material.SetFloat(AnimationSpeed, Speed.value);
            m_Material.SetFloat(BaseDarkening, Black.value ? 0 : 1);

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
