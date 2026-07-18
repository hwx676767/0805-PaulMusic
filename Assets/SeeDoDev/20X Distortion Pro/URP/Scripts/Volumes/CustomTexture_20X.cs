#if DistortionsX_HAS_URP
using UnityEngine;
using UnityEngine.Rendering;

namespace DistortionsPro_20X
{
    [VolumeComponentMenu("20X Distortions Pro/CustomTexture_20X")]

    public class CustomTexture_20X : VolumeComponent, IPostProcessComponent
    {

        public TextureParameter texture = new TextureParameter(null);
        [Range(0f, 1f), Tooltip("fade parameter.")]
        public ClampedFloatParameter fade = new ClampedFloatParameter(0f, 0f, 1f);
        [Range(0f, 1f), Tooltip("Passthrough custom texture alpha chanel.")]
        public NoInterpClampedFloatParameter alpha = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Space]
        [Tooltip("Use Global Post Processing Settings to enable or disable Post Processing in scene view or via camera setup. THIS SETTING SHOULD BE TURNED OFF FOR EFFECTS, IN CASE OF USING THEM FOR SEPARATE LAYERS")]
        public BoolParameter GlobalPostProcessingSettings = new BoolParameter(false);

        public bool IsActive() => fade.value>0;

        public bool IsTileCompatible() => false;

    }
}
#endif
