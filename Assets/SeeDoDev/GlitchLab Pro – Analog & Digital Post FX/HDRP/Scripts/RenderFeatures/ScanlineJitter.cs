#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/ScanlineJitter")]
    public sealed class ScanlineJitter : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect amount.")]
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 200f);
        [Tooltip("Infinite - 0. Periodic- 1, Random - 2")]
        public IntervalModeParameter interval = new IntervalModeParameter { };
        [Tooltip("min/max ranom interval, if Interval = 2.")]
        public NoInterpFloatRangeParameter minMax = new NoInterpFloatRangeParameter(new Vector2(0.1f, 2f), 0, 60);
        [Range(0f, 25f), Tooltip("Glitch periodic interval in seconds.")]
        public NoInterpClampedFloatParameter frequency = new NoInterpClampedFloatParameter(1f, 0f, 25f);
        [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
        public NoInterpClampedFloatParameter rate = new NoInterpClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 15f), Tooltip("effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(1f, 0f, 15f);

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

        const string kShaderName = "GlitchLabPro/ScanlineJitter";
        static readonly int time_ = Shader.PropertyToID("time_");
        static readonly int jitterVFreq = Shader.PropertyToID("jitterVFreq");
        static readonly int jitterVRate = Shader.PropertyToID("jitterVRate");
        static readonly int jitterVAmount = Shader.PropertyToID("jitterVAmount");
        static readonly int jitterVSpeed = Shader.PropertyToID("jitterVSpeed");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        static readonly int TempTargetId = Shader.PropertyToID("ScanlineJitter");

        private float _time;
        private float tempVFR;
        float t;

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

            if (interval.value == IntervalMode.Random)
            {
                t -= Time.deltaTime;
                if (t <= 0)
                {
                    tempVFR = UnityEngine.Random.Range(minMax.value.x, minMax.value.y);
                    t = tempVFR;
                }
            }

            if (unscaledTime.value) { _time = Time.unscaledTime; }
            else _time = Time.time;

            m_Material.SetFloat(time_, _time);

            m_Material.EnableKeyword("VHS_JITTER_V_ON");

            if (interval.value == IntervalMode.Infinite) m_Material.EnableKeyword("JITTER_V_CUSTOM"); else m_Material.DisableKeyword("JITTER_V_CUSTOM");
            if (interval.value == IntervalMode.Random)
                m_Material.SetFloat(jitterVFreq, tempVFR);
            else
                m_Material.SetFloat(jitterVFreq, frequency.value);

            m_Material.SetFloat(jitterVRate, 1 - rate.value);

            m_Material.SetFloat(jitterVAmount, Amount.value);
            m_Material.SetFloat(jitterVSpeed, speed.value);

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