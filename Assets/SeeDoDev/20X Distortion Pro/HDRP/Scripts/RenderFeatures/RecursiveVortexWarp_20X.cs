#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Recursive Vortex Warp")]
    public sealed class RecursiveVortexWarp_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect transparancy. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Fade = new(0f, 0f, 1f);
        [Space]
        public NoInterpClampedFloatParameter MaximumImages = new(1, 0, 5);
        public NoInterpClampedFloatParameter Speed = new(.5f, 0, 5);
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

        const string kShaderName = "DistortionsPro_20X/RecursiveVortexWarpHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int animTime = Shader.PropertyToID("animTime");
        static readonly int iterationCount = Shader.PropertyToID("iterationCount");
        static readonly int blendFactor = Shader.PropertyToID("blendFactor");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume RecursiveVortexWarp is unable to load.");
        }
        float _count;
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

            _count += Time.deltaTime * Speed.value;
            m_Material.SetFloat(animTime, _count);
            m_Material.SetFloat(blendFactor, Fade.value);
            m_Material.SetFloat(iterationCount, MaximumImages.value);

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
