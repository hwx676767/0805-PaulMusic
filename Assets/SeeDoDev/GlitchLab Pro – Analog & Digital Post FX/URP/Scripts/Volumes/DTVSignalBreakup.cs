#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/DTVSignalBreakup")]
    public class DTVSignalBreakup : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0, 1f);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(.7f, 0, 44f);
        [Tooltip("Dropout Intensity.")]
        public ClampedFloatParameter dropoutIntensity = new ClampedFloatParameter(0f, 0, 6f);
        public ClampedFloatParameter DisplaceIntesnsity = new ClampedFloatParameter(0f, 0, 6f);
        public ClampedFloatParameter RGBSplit = new ClampedFloatParameter(0f, 0, 6f);
        [Tooltip("Interlace Intesnsity.")]
        public ClampedFloatParameter interlaceIntesnsity = new ClampedFloatParameter(0f, 0, 6);
        public ClampedFloatParameter InterlaceScale = new ClampedFloatParameter(1f, 0, 900f);
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

        public bool IsActive() => Intensity.value >= 0;
        public bool IsTileCompatible() => false;
    }
}
#endif