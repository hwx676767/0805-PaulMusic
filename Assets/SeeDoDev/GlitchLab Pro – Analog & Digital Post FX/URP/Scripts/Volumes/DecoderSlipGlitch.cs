#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/DecoderSlipGlitch")]
    public class DecoderSlipGlitch : VolumeComponent, IPostProcessComponent
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

        [Space]

        [Tooltip("Global override switch for post-processing visibility (Scene View / camera setup). Keep OFF when using this effect in layered or selective rendering setups.")]
        public BoolParameter GlobalPostProcessingSettings = new BoolParameter(false);

        public bool IsActive() => Amount.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
#endif