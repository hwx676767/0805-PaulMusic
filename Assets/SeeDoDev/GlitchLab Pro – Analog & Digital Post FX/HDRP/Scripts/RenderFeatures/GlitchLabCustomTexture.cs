#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/GlitchLab Custom Texture")]
    public sealed class GlitchLabCustomTexture : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public BoolParameter enable = new BoolParameter(false);
        public TextureParameter texture = new TextureParameter(null);
        [Range(0f, 1f), Tooltip("Passthrough custom texture alpha chanel.")]
        public ClampedFloatParameter alpha = new ClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 1f), Tooltip("fade parameter.")]
        public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && enable.value;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/GlitchLabCustomTexture";



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

            if (texture.value != null)
                m_Material.SetTexture("_CustomTexture", texture.value);
            m_Material.SetFloat("_Fade", fade.value);
            m_Material.SetFloat("_AlphaCutoff", alpha.value);


            cmd.Blit(source, destination, m_Material, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
#endif