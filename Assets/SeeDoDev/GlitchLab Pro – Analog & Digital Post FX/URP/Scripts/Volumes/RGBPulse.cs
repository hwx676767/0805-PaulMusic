#if GLP_HAS_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GlitchLabPro
{
    [VolumeComponentMenu("GlitchLab Pro/RGBPulse")]
    public class RGBPulse : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 2f), Tooltip("effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 2f);
        [Range(0f, 21f), Tooltip("Glitch periodic interval in seconds.")]
        public NoInterpClampedFloatParameter interval = new NoInterpClampedFloatParameter(1f, 0f, 21f);
        [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
        public NoInterpClampedFloatParameter rate = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 50f), Tooltip("color shift.")]
        public NoInterpClampedFloatParameter RGBSplit = new NoInterpClampedFloatParameter(1f, 0f, 50f);
        [Range(0f, 1f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip(" true - Enables ability to adjust resolution. false - screen resolution.")]
        public BoolParameter customResolution = new BoolParameter(false);
        [Tooltip("jitter resolution.")]
        public NoInterpVector2Parameter resolution = new NoInterpVector2Parameter(new Vector2(640f, 480f));
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