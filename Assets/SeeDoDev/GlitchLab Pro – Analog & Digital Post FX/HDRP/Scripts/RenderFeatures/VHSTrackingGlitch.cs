#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{

    [Serializable, VolumeComponentMenu("GlitchLab Pro/VHSTrackingGlitch")]
    public sealed class VHSTrackingGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Master strength of the VHS tracking effect. Scales band distortion and chroma separation together. 0 disables the effect.")]
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 2f);

        [Tooltip("Horizontal displacement amount for tracking bands. Higher values shift scanlines/bands further left/right.")]
        public NoInterpClampedFloatParameter BandShift = new NoInterpClampedFloatParameter(0f, 0f, .1f);

        [Tooltip("Band thickness / profile control. Higher values produce thicker, more visible tracking bands.")]
        public NoInterpClampedFloatParameter BandWidth = new NoInterpClampedFloatParameter(0f, 0f, 10f);

        [Tooltip("Chromatic separation strength applied inside active bands. Higher values increase R/G horizontal split when a band passes.")]
        public NoInterpClampedFloatParameter ChromaSplit = new NoInterpClampedFloatParameter(0f, 0f, 10f);

        [Tooltip("Vertical placement bias for the band cluster. Use to move the tracking region up/down (and scale its travel range).")]
        public NoInterpClampedFloatParameter BandYPos = new NoInterpClampedFloatParameter(1f, 0f, 4f);

        [Tooltip("Animation rate of the tracking bands. Higher values make band motion and randomization update faster.")]
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(1f, 0f, 20f);

        [Space]
        [Tooltip("Mask texture used to limit where the effect applies (white/opaque = affected, black/transparent = unaffected).")]
        public TextureParameter mask = new TextureParameter(null);

        [Tooltip("Mask sampling channel. Use Alpha for textures with transparency; use Red if your mask is stored in RGB without alpha.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

        [Tooltip("Mask cutoff threshold. Lower values expand the affected area; higher values tighten the edge.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && Intensity.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/VHSTrackingGlitch";
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
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

            m_Material.SetFloat("_Intensity", Intensity.value);
            m_Material.SetFloat("_BandShift", BandShift.value);
            m_Material.SetFloat("_ChromaSplit", ChromaSplit.value);
            m_Material.SetFloat("_BandWidth", BandWidth.value);
            m_Material.SetFloat("_BandYPos", BandYPos.value);
            m_Material.SetFloat("Speed", Speed.value);

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