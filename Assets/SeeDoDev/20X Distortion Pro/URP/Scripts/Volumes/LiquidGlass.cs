#if DistortionsX_HAS_URP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace DistortionsPro_20X
{

    [Serializable, VolumeComponentMenu("20X Distortions Pro/Liquid Glass")]
    public sealed class LiquidGlass : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Overall strength of ringing/halos around edges.")]
        public ClampedFloatParameter _Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _RingSize = new ClampedFloatParameter(0.7f, 0f, 2f);
        public ClampedFloatParameter _RingWidth = new ClampedFloatParameter(0f, 0f, 2f);
        public ClampedFloatParameter _RingDistortion = new ClampedFloatParameter(0f, 0f, 3f);
        public ClampedFloatParameter _RingNoise = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _Speed = new ClampedFloatParameter(1f, 0f, 10f);
        public ClampedFloatParameter _RimRing = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _Bulge = new ClampedFloatParameter(0f, 0, 1f);


        public bool IsActive() => _Amount.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
#endif