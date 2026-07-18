#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/Chromatic Noise")]
    public class ChromaticNoise : VolumeComponent, IPostProcessComponent
    {
        [Header("Random Seed")]
        [Tooltip("seed x")]
        public NoInterpClampedFloatParameter x = new NoInterpClampedFloatParameter(127.1f, -2f, 200f);
        [Tooltip("seed y")]
        public NoInterpClampedFloatParameter y = new NoInterpClampedFloatParameter(43758.5453123f, -2f, 10002f);
        [Tooltip("seed z")]
        public NoInterpClampedFloatParameter z = new NoInterpClampedFloatParameter(311.7f, -2f, 200f);
        [Space]
        [Tooltip("Effect fade.")]
        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("Effect amount")]
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 2f);
        [Tooltip("Stretch on X axes")]
        public ClampedFloatParameter stretch = new ClampedFloatParameter(0f, 0f, 4f);
        [Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(0.5f, 0f, 1f);
        [Space]
        [Tooltip("Red.")]
        public NoInterpClampedFloatParameter rMultiplier = new NoInterpClampedFloatParameter(1f, -1f, 2f);
        [Tooltip("Green.")]
        public NoInterpClampedFloatParameter gMultiplier = new NoInterpClampedFloatParameter(1f, -1f, 2f);
        [Tooltip("Blue.")]
        public NoInterpClampedFloatParameter bMultiplier = new NoInterpClampedFloatParameter(0f, -1f, 2f);
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
        public bool IsActive() => Fade.value > 0;

        public bool IsTileCompatible() => false;
    }

}
#endif