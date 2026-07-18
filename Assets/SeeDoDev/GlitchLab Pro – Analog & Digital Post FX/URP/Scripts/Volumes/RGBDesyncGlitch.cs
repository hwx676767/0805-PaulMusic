#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/RGBDesyncGlitch")]
    public class RGBDesyncGlitch : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Stops effect without disabling it. Use it in your effect manipulation script")]
        public BoolParameter stop = new BoolParameter(false);
        [Tooltip("Effect fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0, 1f);
        [Tooltip("Glitch Lines width")]
        public NoInterpClampedFloatParameter linesWidth = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Glitch Lines amount")]
        public NoInterpClampedFloatParameter LinesAmount = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Glitch offset")]
        public NoInterpClampedFloatParameter offset = new NoInterpClampedFloatParameter(0.15f, 0, 2f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(.9f, 0, 1);
        [Tooltip("Effect speed")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(3f, 0, 20);
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