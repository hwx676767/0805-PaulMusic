#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/Digital Glitch")]
    public class DigitalGlitch : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("Glitch lines width.")]
        public NoInterpClampedFloatParameter LinesWidth = new NoInterpClampedFloatParameter(1f, 0.1f, 20f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter Speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip("Offset on X axis.")]
        public NoInterpClampedFloatParameter Offset = new NoInterpClampedFloatParameter(1f, 0f, 13f);
        [Space]
        [Tooltip("Time.unscaledTime .")]
        public BoolParameter unscaledTime = new BoolParameter(false);
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