#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/MosaicGlitch")]
    public class MosaicGlitch : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect Fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0, 1f);
        [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
        public BoolParameter stop = new BoolParameter(false);
        [Tooltip("Effect strength")]
        public NoInterpClampedFloatParameter strength = new NoInterpClampedFloatParameter(0.03f, 0, 0.5f);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(16f, 0, 100f);
        [Tooltip("Effect size")]
        public NoInterpClampedFloatParameter size = new NoInterpClampedFloatParameter(1f, 0, 10f);
        [Tooltip("RGBSplit")]
        public NoInterpClampedFloatParameter RGBSplit = new NoInterpClampedFloatParameter(1f, 0, 2f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(1f, 0, 1);
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
        public bool IsActive() => Fade.value>0;
        public bool IsTileCompatible() => false;
    }
}
#endif