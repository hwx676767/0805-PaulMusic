#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Fluid Worley Displacement")]
    public sealed class FluidworleyDisplacement_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Intensity = new(0f, 0.00001f, 1f, true);
        [Space]
        public NoInterpClampedFloatParameter Offset = new(0.1f, 0.0001f, 2);
        public NoInterpClampedFloatParameter Speed = new(1f, 0f, 10);
        public NoInterpClampedFloatParameter Size = new(0.1f, 0f, 3);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Intensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/FluidWorleyDisplacementHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int DisplaceAmount = Shader.PropertyToID("DisplaceAmount");
        static readonly int FlowVelocity = Shader.PropertyToID("FlowVelocity");
        static readonly int NoiseScale = Shader.PropertyToID("NoiseScale");
        static readonly int BlendIntensity = Shader.PropertyToID("BlendIntensity");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume FluidWorleyDisplacement is unable to load.");
        }
        float _t;
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

            m_Material.SetFloat(BlendIntensity, Intensity.value);
            m_Material.SetFloat(FlowVelocity, Speed.value * 10);
            m_Material.SetFloat(DisplaceAmount, Offset.value);
            m_Material.SetFloat(NoiseScale, Size.value);

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
