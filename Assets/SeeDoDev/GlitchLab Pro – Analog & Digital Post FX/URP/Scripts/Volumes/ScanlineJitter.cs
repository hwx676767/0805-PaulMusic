#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/ScanlineJitter")]
    public class ScanlineJitter : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 200f);
        [Tooltip("Infinite - 0. Periodic- 1, Random - 2")]
        public IntervalModeParameter interval = new IntervalModeParameter { };
        [Tooltip("min/max ranom interval, if Interval = 2.")]
        public NoInterpFloatRangeParameter minMax = new NoInterpFloatRangeParameter(new Vector2(0.1f, 2f), 0, 60);
        [Range(0f, 25f), Tooltip("Glitch periodic interval in seconds.")]
        public NoInterpClampedFloatParameter frequency = new NoInterpClampedFloatParameter(1f, 0f, 25f);
        [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
        public NoInterpClampedFloatParameter rate = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 15f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 15f);

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