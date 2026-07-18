#if DistortionsX_HAS_HDRP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Liquid Glass")]
    public sealed class LiquidGlass_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Overall strength of ringing/halos around edges.")]
        public ClampedFloatParameter _Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _RingSize = new ClampedFloatParameter(0.7f, 0f, 2f);
        public ClampedFloatParameter _RingWidth = new ClampedFloatParameter(0f, 0f, 2f);
        public ClampedFloatParameter _RingDistortion = new ClampedFloatParameter(0f, 0f, 3f);
        public ClampedFloatParameter _RingNoise = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _Speed = new ClampedFloatParameter(1f, 0f, 10f);
        public ClampedFloatParameter _RimRing = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _Bulge = new ClampedFloatParameter(0f, 0, 1f);
        Material m_Material;

        public bool IsActive() => active && m_Material != null && _Amount.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/LiquidGlassHDRP";


        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume PrismaticDisplacement is unable to load.");
        }
        float _t;
        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetVector("_Lens", new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            m_Material.SetFloat("_RingSize", _RingSize.value);
            m_Material.SetFloat("_Amount", _Amount.value);
            m_Material.SetFloat("_RingWidth", _RingWidth.value);
            m_Material.SetFloat("_RingDistortion", _RingDistortion.value);
            m_Material.SetFloat("_RingNoise", _RingNoise.value);
            m_Material.SetFloat("_Speed", _Speed.value);
            m_Material.SetFloat("_RimRing", _RimRing.value);
            m_Material.SetFloat("_Bulge", _Bulge.value);
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