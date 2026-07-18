#if GLP_HAS_URP

using System;
using UnityEngine.Rendering;

namespace GlitchLabPro
{
    public enum AxisMode
    {
        Vertical,
        Horizontal
    }
    public enum IntervalMode
    {
        Infinite,
        Periodic,
        Random
    }
    public enum maskChannelMode
    {
        alphaChannel,
        redChannel
    }
    [Serializable]
    public sealed class maskChannelModeParameter : VolumeParameter<maskChannelMode> { };
}
#endif