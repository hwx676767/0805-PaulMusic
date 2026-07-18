#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Speed Warp")]
    public sealed class SpeedWarp_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Intensity = new(0f, 0, 2);
        [Space]
        public NoInterpClampedFloatParameter Frequency = new(64, 0, 200);
        public NoInterpClampedFloatParameter Speed = new(.5f, 0, 1);
        public Vector2Parameter Center = new(Vector2.zero);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new DistortionsPro_20X.maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => active && m_Material != null && Intensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/SpeedWarpHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int NoiseFrequency = Shader.PropertyToID("NoiseFrequency");
        static readonly int WarpStrength = Shader.PropertyToID("WarpStrength");
        static readonly int FlowRate = Shader.PropertyToID("FlowRate");
        static readonly int WarpCenter = Shader.PropertyToID("WarpCenter");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume SpeedWarp is unable to load.");
        }
        float t = 1;

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

            t += Time.deltaTime;
            m_Material.SetFloat(FlowRate, Speed.value);
            m_Material.SetFloat(NoiseFrequency, Frequency.value);
            m_Material.SetFloat(WarpStrength, Intensity.value);
            m_Material.SetVector(WarpCenter, Center.value);
            m_Material.SetFloat("T", t);
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
