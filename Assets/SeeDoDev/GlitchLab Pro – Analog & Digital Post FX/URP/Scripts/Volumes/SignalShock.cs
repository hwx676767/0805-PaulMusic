#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/Signal Shock")]

    public class SignalShock : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect fade")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("Effect amount")]
        public NoInterpClampedFloatParameter amount = new NoInterpClampedFloatParameter(0.2f, 0f, 1f);
        [Tooltip("Random effect activation for short period of time. 0  = never activate effect, 1 = endless effect")]
        public NoInterpClampedFloatParameter randomActivateAmount = new NoInterpClampedFloatParameter(.9f, 0f, 1f);
        [Tooltip("Glitch cell size")]
        public NoInterpVector2Parameter size = new NoInterpVector2Parameter(new Vector2(1f, 1f));
        [Tooltip("Enables light filter to make effect softer")]
        public BoolParameter light = new BoolParameter(false);
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