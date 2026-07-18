#if DistortionsX_HAS_URP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Tiny Planet Warp")]
    public sealed class TinyPlanetWarp : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Overall strength.")]
        public ClampedFloatParameter _Blend = new ClampedFloatParameter(0f, 0f, 1f);


        public bool IsActive() => _Blend.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
#endif