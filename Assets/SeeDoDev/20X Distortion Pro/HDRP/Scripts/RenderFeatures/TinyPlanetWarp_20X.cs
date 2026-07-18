#if DistortionsX_HAS_HDRP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


namespace DistortionsPro_20X
{



    [Serializable, VolumeComponentMenu("20X Distortions Pro/Tiny Planet Warp")]
    public sealed class TinyPlanetWarp_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Overall strength.")]
        public ClampedFloatParameter _Blend = new ClampedFloatParameter(0f, 0f, 1f);
        Material m_Material;

        public bool IsActive() => active && m_Material != null && _Blend.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/TinyPlanetWarpHDRP";


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


            m_Material.SetFloat("_Strength", _Blend.value);
    
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