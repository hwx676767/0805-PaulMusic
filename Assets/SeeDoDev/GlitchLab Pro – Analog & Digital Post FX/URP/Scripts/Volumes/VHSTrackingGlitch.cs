#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/VHSTrackingGlitch")]
    public class VHSTrackingGlitch : VolumeComponent, IPostProcessComponent
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

        [Space]
        [Tooltip("Global override switch for post-processing visibility (Scene View / camera setup). Keep OFF when using this effect in layered or selective rendering setups.")]
        public BoolParameter GlobalPostProcessingSettings = new BoolParameter(false);

        public bool IsActive() => Intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
#endif