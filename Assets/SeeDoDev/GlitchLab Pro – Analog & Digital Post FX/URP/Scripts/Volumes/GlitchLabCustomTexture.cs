#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/GlitchLab Custom Texture")]

    public class GlitchLabCustomTexture : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enable = new BoolParameter(false);
        public TextureParameter texture = new TextureParameter(null);
        public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0, 1);
        public ClampedFloatParameter Alpha = new ClampedFloatParameter(1f, 0, 1);

        [Space]
        [Tooltip("Use Global Post Processing Settings to enable or disable Post Processing in scene view or via camera setup. THIS SETTING SHOULD BE TURNED OFF FOR EFFECTS, IN CASE OF USING THEM FOR SEPARATE LAYERS")]
        public BoolParameter GlobalPostProcessingSettings = new BoolParameter(false);

        public bool IsActive() => (bool)enable;

        public bool IsTileCompatible() => false;

    }
}
#endif