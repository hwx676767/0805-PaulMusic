#if DistortionsX_HAS_HDRP
using UnityEngine.Rendering;
using System;

namespace DistortionsPro_20X
{
    public enum maskChannelMode
    {
        alphaChannel,
        redChannel
    }
    [Serializable]
public sealed class maskChannelModeParameter : VolumeParameter<maskChannelMode> { };
}
#endif
