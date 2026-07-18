#if GLP_HAS_URP

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [Serializable]
    public sealed class OffsetAxesModeParameter : VolumeParameter<AxisMode> { };
    [Serializable]
    public sealed class IntervalModeParameter : VolumeParameter<IntervalMode> { };
    [VolumeComponentMenu("GlitchLab Pro/LineShakeGlitch")]
    public class LineShakeGlitch : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 50f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip(" Displacement axis. ")]
        public OffsetAxesModeParameter offsetAxis = new OffsetAxesModeParameter { };
        [Tooltip("shift axis")]
        public OffsetAxesModeParameter shiftMode = new OffsetAxesModeParameter { };
        [Tooltip(" Displacement lines width.")]
        public NoInterpFloatParameter stretchResolution = new NoInterpFloatParameter(220);
        [Range(0f, 1f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
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

        public bool IsActive() => Amount.value>0;

        public bool IsTileCompatible() => false;
    }
}
#endif