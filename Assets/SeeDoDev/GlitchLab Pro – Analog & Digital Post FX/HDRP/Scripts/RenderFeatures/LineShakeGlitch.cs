#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable]
    public sealed class OffsetAxesModeParameter : VolumeParameter<AxisMode> { };
    [Serializable]
    public sealed class IntervalModeParameter : VolumeParameter<IntervalMode> { };

    [Serializable, VolumeComponentMenu("GlitchLab Pro/LineShakeGlitch")]
    public sealed class LineShakeGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 50f);
        public NoInterpClampedFloatParameter randAmount = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Tooltip(" Displacement axis. ")]
        public OffsetAxesModeParameter offsetAxis = new OffsetAxesModeParameter { };
        [Tooltip("shift axis")]
        public OffsetAxesModeParameter shiftMode = new OffsetAxesModeParameter { };
        [Tooltip(" Displacement lines width.")]
        public NoInterpFloatParameter stretchResolution = new NoInterpFloatParameter(220);
        [Range(0f, 1f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Space]
        [Tooltip("Time.unscaledTime .")]
        public BoolParameter unscaledTime = new BoolParameter(false);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && Amount.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/LineShakeGlitch";
        static readonly int screenLinesNum = Shader.PropertyToID("screenLinesNum");
        static readonly int time_ = Shader.PropertyToID("time_");
        static readonly int jitterHAmount = Shader.PropertyToID("jitterHAmount");
        static readonly int M_speed = Shader.PropertyToID("speed");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        private float _time;


        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume AnalogChromaGlitch is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (unscaledTime.value) { _time = Time.unscaledTime; }
            else _time = Time.time;

            m_Material.SetFloat(screenLinesNum, stretchResolution.value);
            m_Material.SetFloat(time_, _time);
            if (shiftMode.value == AxisMode.Horizontal) m_Material.EnableKeyword("SHIFT_H"); else m_Material.DisableKeyword("SHIFT_H");
            switch (offsetAxis.value)
            {
                case AxisMode.Vertical:
                    m_Material.SetFloat("_LinesAxis", 0);
                    break;
                case AxisMode.Horizontal:
                    m_Material.SetFloat("_LinesAxis", 1);
                    break;
                default:
                    break;
            }
            switch (shiftMode.value)
            {
                case AxisMode.Vertical:
                    m_Material.SetFloat("_ShiftH", 0);
                    break;
                case AxisMode.Horizontal:
                    m_Material.SetFloat("_ShiftH", 1);
                    break;
                default:
                    break;
            }
            m_Material.SetFloat(jitterHAmount, Amount.value);
            m_Material.SetFloat(M_speed, speed.value);
            m_Material.SetFloat("randAmount", randAmount.value);


            if (mask.value != null)
            {
                m_Material.SetFloat(_FadeMultiplier, 1);
                m_Material.SetFloat("_adj", maskEdgeFineTuning.value);
                m_Material.SetTexture(_Mask, mask.value);
                ParamSwitch(m_Material, maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
            }
            else
            {
                m_Material.SetFloat(_FadeMultiplier, 0);
            }

            cmd.Blit(source, destination, m_Material, 0);
        }
        private void ParamSwitch(Material mat, bool paramValue, string paramName)
        {
            if (paramValue) mat.EnableKeyword(paramName);
            else mat.DisableKeyword(paramName);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
#endif