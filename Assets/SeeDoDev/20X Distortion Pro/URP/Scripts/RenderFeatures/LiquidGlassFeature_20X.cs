#if DistortionsX_HAS_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering.RenderGraphModule;
namespace DistortionsPro_20X
{
    public class LiquidGlass_20X : ScriptableRendererFeature
    {
        [SerializeField]
        private Shader m_Shader;
        private Material material;
        LiquidGlass_Pass Distortion_RenderPass;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

        public override void Create()
        {
            m_Shader = Shader.Find("DistortionsPro_20X/LiquidGlass");
            if (m_Shader == null)
            {
                return;
            }
            material = new Material(m_Shader);
            Distortion_RenderPass = new LiquidGlass_Pass(material);

            Distortion_RenderPass.renderPassEvent = Event;

        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            LiquidGlass myVolume = VolumeManager.instance.stack?.GetComponent<LiquidGlass>();
            if (myVolume == null || !myVolume.IsActive())
                return;

            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(Distortion_RenderPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
#else
                Destroy(material);
#endif
        }
        public class LiquidGlass_Pass : ScriptableRenderPass
        {
            static readonly int _Mask = Shader.PropertyToID("_Mask");
            static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
            static readonly int FlowSpeed = Shader.PropertyToID("FlowSpeed");
            static readonly int SampleRadius = Shader.PropertyToID("SampleRadius");
            static readonly int SampleCount = Shader.PropertyToID("SampleCount");
            static readonly int BlendFactor = Shader.PropertyToID("BlendFactor");
            private Material material;

            public LiquidGlass_Pass(Material material)
            {
                this.material = material;
            }
            private void ParamSwitch(Material mat, bool paramValue, string paramName)
            {
                if (paramValue) mat.EnableKeyword(paramName);
                else mat.DisableKeyword(paramName);
            }

            private static RenderTextureDescriptor GetCopyPassTextureDescriptor(RenderTextureDescriptor desc)
            {
                desc.msaaSamples = 1;

                // This avoids copying the depth buffer tied to the current descriptor as the main pass in this example does not use it
                desc.depthBufferBits = (int)DepthBits.None;

                return desc;
            }
            private static void ExecuteCopyColorPass(RasterCommandBuffer cmd, RTHandle sourceTexture)
            {
                Blitter.BlitTexture(cmd, sourceTexture, new Vector4(1, 1, 0, 0), 0.0f, false);
            }
            private static void ExecuteCopyColorPass(CopyPassData data, RasterGraphContext context)
            {
                ExecuteCopyColorPass(context.cmd, data.inputTexture);
            }

            private static void ExecuteMainPass(RasterCommandBuffer cmd, RTHandle sourceTexture, Material material, int pass)
            {
                Blitter.BlitTexture(cmd, sourceTexture, new Vector4(1f, 1f, 0f, 0f), material, pass);
            }
            private static void ExecuteMainPass(PassData data, RasterGraphContext context, int pass)
            {
                ExecuteMainPass(context.cmd, data.src.IsValid() ? data.src : null, data.material, pass);
            }

            private class PassData
            {
                internal TextureHandle src;
                internal Material material;
            }
            private class CopyPassData
            {
                public TextureHandle inputTexture;
            }
            float TimeX;
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (material == null) return;

                // Use the Volume settings or the default settings if no Volume is set.
                var volumeComponent = VolumeManager.instance.stack.GetComponent<LiquidGlass>();

                material.SetVector("_Lens", new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                material.SetFloat("_RingSize", volumeComponent._RingSize.value);
                material.SetFloat("_Amount", volumeComponent._Amount.value);
                material.SetFloat("_RingWidth", volumeComponent._RingWidth.value);
                material.SetFloat("_RingDistortion", volumeComponent._RingDistortion.value);
                material.SetFloat("_RingNoise", volumeComponent._RingNoise.value);
                material.SetFloat("_Speed", volumeComponent._Speed.value);
                material.SetFloat("_RimRing", volumeComponent._RimRing.value);
                material.SetFloat("_Bulge", volumeComponent._Bulge.value);

                UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                UniversalRenderer renderer = (UniversalRenderer)cameraData.renderer;
                var colorCopyDescriptor = GetCopyPassTextureDescriptor(cameraData.cameraTargetDescriptor);
                TextureHandle copiedColor = TextureHandle.nullHandle;

                copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);

                using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("CustomPostPass_CopyColor", out var passData, profilingSampler))
                {
                    passData.inputTexture = resourcesData.activeColorTexture;
                    builder.UseTexture(resourcesData.activeColorTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(copiedColor, 0, AccessFlags.Write);
                    builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => ExecuteCopyColorPass(data, context));
                }

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("CustomPostPass", out var passData, profilingSampler))
                {
                    passData.material = material;

                    passData.src = copiedColor;
                    builder.UseTexture(copiedColor, AccessFlags.Read);

                    builder.SetRenderAttachment(resourcesData.activeColorTexture, 0, AccessFlags.Write);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteMainPass(data, context, 0));
                }
            }
        }
    }
}
#endif
