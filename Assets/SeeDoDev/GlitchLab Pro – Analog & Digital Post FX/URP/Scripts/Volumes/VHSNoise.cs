#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/VHSNoise")]
    public class VHSNoise : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, .02f);
        [Tooltip("Noise ")]
        public BoolParameter Noise = new BoolParameter(true);
        [Tooltip("Floating Lines Amount")]
        public NoInterpClampedFloatParameter linesAmount = new NoInterpClampedFloatParameter(1f, 0f, 10f);
        [Tooltip("Lines speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 10f);
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