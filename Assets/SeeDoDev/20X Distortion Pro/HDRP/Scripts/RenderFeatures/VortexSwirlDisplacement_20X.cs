#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Vortex Swirl Displacement")]
    public sealed class VortexSwirlDisplacement_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect transparancy. Volumes 'Weight' value affect this parameter")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1, true);
        [Space]
        public NoInterpClampedFloatParameter Iterations = new NoInterpClampedFloatParameter(2f, 0.0001f, 3f);
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(0.1f, 0f, 10f);
        public NoInterpVector2Parameter CenterOffset = new NoInterpVector2Parameter(Vector2.zero);
        public NoInterpClampedFloatParameter RotationScale = new NoInterpClampedFloatParameter(0.5f, -2f, 2f);
        public NoInterpClampedFloatParameter Falloff = new NoInterpClampedFloatParameter(1f, 0f, 3f);

        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public DistortionsPro_20X.maskChannelModeParameter maskChannel = new();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        Material m_Material;

        public bool IsActive() => active && m_Material != null && Fade.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/VortexSwirlDisplacementHDRP";
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
        static readonly int FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int AnimSpeed = Shader.PropertyToID("AnimSpeed");
        static readonly int BlendFactor = Shader.PropertyToID("BlendFactor");
        static readonly int IterationCount = Shader.PropertyToID("IterationCount");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume VortexSwirlDisplacement is unable to load.");
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

            m_Material.SetFloat(IterationCount, Iterations.value);
            m_Material.SetFloat(AnimSpeed, Speed.value);
            m_Material.SetFloat(BlendFactor, Fade.value);
            m_Material.SetVector("_CenterOffset", CenterOffset.value);
            m_Material.SetFloat("_RotScale", RotationScale.value);
            m_Material.SetFloat("_FalloffExp", Falloff.value);


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
