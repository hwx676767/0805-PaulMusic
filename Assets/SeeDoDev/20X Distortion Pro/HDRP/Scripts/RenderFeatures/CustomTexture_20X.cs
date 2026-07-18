#if DistortionsX_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Custom Texture")]
    public sealed class CustomTexture_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("�ustom texture.")]
        public TextureParameter texture = new TextureParameter(null);
        [Range(0f, 1f), Tooltip("Passthrough custom texture alpha chanel.")]
        public ClampedFloatParameter alpha = new ClampedFloatParameter(1f, 0f, 1f);
        [Range(0f, 1f), Tooltip("fade parameter.")]
        public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
        //
        Material m_Material;

        public bool IsActive() => active && m_Material != null && intensity.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("DistortionsPro_20X/CustomTextureHDRP") != null)
                m_Material = new Material(Shader.Find("DistortionsPro_20X/CustomTextureHDRP"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;
            if (texture.value != null)
                m_Material.SetTexture("_CustomTexture", texture.value);
            m_Material.SetFloat("_Fade", fade.value);
            m_Material.SetFloat("_AlphaCutoff", alpha.value );
            m_Material.SetFloat("_Intensity", intensity.value);
            cmd.Blit(source, destination, m_Material, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
#endif
