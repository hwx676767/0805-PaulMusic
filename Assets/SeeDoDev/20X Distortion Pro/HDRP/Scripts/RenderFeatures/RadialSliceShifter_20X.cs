#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Radial Slice Shifter")]
    public sealed class RadialSliceShifter_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Minimum line width. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter LineWidth = new(0f, 0f, 1, true);
        public NoInterpClampedFloatParameter Speed = new(1f, 0f, 10);
        [Tooltip("Animate line width")]
        public BoolParameter AnimateLine = new(true);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter Mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter MaskChannel = new();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter MaskEdgeFineTuning = new(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && LineWidth.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/RadialSliceShifterHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int BaseSliceWidth = Shader.PropertyToID("BaseSliceWidth");
        static readonly int LineExpansionRate = Shader.PropertyToID("LineExpansionRate");
        static readonly int Phase = Shader.PropertyToID("Phase");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume RadialSliceShifter is unable to load.");
        }
        float _t;
        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (Mask.value != null)
            {
                m_Material.SetFloat(FadeMultiplier, 1);
                m_Material.SetFloat(MaskThreshold, MaskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, Mask.value);
                ParamSwitch(m_Material, MaskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(FadeMultiplier, 0);
            }

            _t += Time.deltaTime * Speed.value;
            m_Material.SetFloat(Phase, _t);
            m_Material.SetFloat(LineExpansionRate, AnimateLine.value ? 1 : 0);
            m_Material.SetFloat(BaseSliceWidth, LineWidth.value);

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
