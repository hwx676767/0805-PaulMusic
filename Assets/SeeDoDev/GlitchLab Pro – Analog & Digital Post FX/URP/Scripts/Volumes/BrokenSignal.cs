#if GLP_HAS_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/BrokenSignal")]
    public class BrokenSignal : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect intensity.")]
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);
        [Range(0f, 2f), Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(.1f, 0f, 1f);
        [Range(1f, 2f), Tooltip("block size (higher value = smaller blocks.")]
        public NoInterpClampedFloatParameter resolutionMultiplier = new NoInterpClampedFloatParameter(1f, 1f, 2f);
        [Range(0f, 1f), Tooltip("blocks width (max value makes effect fullscreen).")]
        public NoInterpClampedFloatParameter stretchMultiplier = new NoInterpClampedFloatParameter(0.88f, 0f, 1f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        [Space]
        [Tooltip("Use Global Post Processing Settings to enable or disable Post Processing in scene view or via camera setup. THIS SETTING SHOULD BE TURNED OFF FOR EFFECTS, IN CASE OF USING THEM FOR SEPARATE LAYERS")]
        public BoolParameter GlobalPostProcessingSettings = new BoolParameter(false);


        public bool IsActive() => Intensity.value > 0;

        public bool IsTileCompatible() => false;
    }
}
#endif