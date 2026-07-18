#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/DataGlitch")]
    public class DataGlitch : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 50f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("block size (higher value = smaller blocks).")]
        public ClampedFloatParameter blockSize = new ClampedFloatParameter(0f, 0f, 505f);

        [Tooltip("maximum color shift on X axis.")]
        public ClampedFloatParameter maxOffsetX = new ClampedFloatParameter(0f, 0f, 25f);
        [Tooltip("maximum color shift on Y axis.")]
        public ClampedFloatParameter maxOffsetY = new ClampedFloatParameter(0f, 0f, 25f);
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

        public bool IsActive() => Amount.value > 0;

        public bool IsTileCompatible() => false;
    }
}
#endif