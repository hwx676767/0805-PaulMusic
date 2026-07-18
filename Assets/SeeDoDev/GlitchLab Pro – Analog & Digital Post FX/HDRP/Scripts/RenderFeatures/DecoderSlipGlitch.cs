#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/DecoderSlipGlitch")]
    public sealed class DecoderSlipGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Tear trigger strength. Higher values cause more frequent horizontal line slips and stronger decoder-style color corruption. Set to 0 to disable the effect.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);

        [Tooltip("Overall intensity multiplier for tearing/hold artifacts. Increases the displacement amount and corruption when a band is triggered.")]
        public NoInterpClampedFloatParameter Offset = new NoInterpClampedFloatParameter(.35f, 0f, 1f);

        [Tooltip("Animation speed for the random band events. 0 = static. Higher values update tearing/hold patterns faster.")]
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(.5f, 0f, 1f);

        [Tooltip("Number of horizontal bands used to generate per-line events. Higher values = thinner bands / more detailed tearing.")]
        public NoInterpClampedFloatParameter Bands = new NoInterpClampedFloatParameter(260f, 0f, 560f);

        [Tooltip("Chance for a band to enter 'line hold' mode (vertical line repeat / stepped Y). Higher values = more frequent holds.")]
        public NoInterpClampedFloatParameter HoldChance = new NoInterpClampedFloatParameter(0.1f, 0f, 1f);

        [Tooltip("Resolution multiplier inside band.")]
        public NoInterpClampedFloatParameter resM = new NoInterpClampedFloatParameter(0f, 0f, 1000f);

        [Space]

        [Tooltip("Mask texture used to limit where the effect applies (white/opaque = affected, black/transparent = unaffected).")]
        public TextureParameter mask = new TextureParameter(null);

        [Tooltip("Mask sampling channel. Use Alpha for textures with transparency; use Red if your mask is stored in RGB without alpha.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

        [Tooltip("Mask cutoff threshold. Lower values expand the affected area; higher values tighten the edge.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => m_Material != null && Amount.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/DecoderSlipGlitch";
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

            m_Material.SetFloat("Offset", Offset.value);
            m_Material.SetFloat("resM", resM.value);
            m_Material.SetFloat("Speed", Speed.value);
            m_Material.SetFloat("Bands", Bands.value);
            m_Material.SetFloat("TearChance", Amount.value);
            m_Material.SetFloat("HoldChance", HoldChance.value);
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