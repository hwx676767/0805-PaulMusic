#if GLP_HAS_URP

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace GlitchLabPro
{
    public class ChromaticNoise_Feature : ScriptableRendererFeature
    {
        [SerializeField]
        private Shader m_Shader;
        private Material material;
        Glitch_Pass Glitch_RenderPass;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        

        public override void Create()
        {
            m_Shader = Shader.Find("GlitchLabPro/ChromaticNoiseURP");
            if (m_Shader == null)
            {
                Debug.Log("Shader not found");
                return;
            }
            material = new Material(m_Shader);
            Glitch_RenderPass = new Glitch_Pass(material);

            Glitch_RenderPass.renderPassEvent = Event;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            ChromaticNoise myVolume = VolumeManager.instance.stack?.GetComponent<ChromaticNoise>();
            if (myVolume == null || !myVolume.IsActive())
                return;
            if (!renderingData.cameraData.postProcessEnabled && myVolume.GlobalPostProcessingSettings.value) return;

            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(Glitch_RenderPass);
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

        public class Glitch_Pass : ScriptableRenderPass
        {
            static readonly int _Mask = Shader.PropertyToID("_Mask");
            static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
            static readonly int MaskThreshold = Shader.PropertyToID("MaskThreshold");
            static readonly int Strength = Shader.PropertyToID("Strength");
            static readonly int x = Shader.PropertyToID("x");
            static readonly int y = Shader.PropertyToID("y");
            static readonly int angleY = Shader.PropertyToID("angleY");
            static readonly int Stretch = Shader.PropertyToID("Stretch");
            static readonly int Speed = Shader.PropertyToID("Speed");
            static readonly int mR = Shader.PropertyToID("mR");
            static readonly int mG = Shader.PropertyToID("mG");
            static readonly int mB = Shader.PropertyToID("mB");
            static readonly int Fade = Shader.PropertyToID("Fade");
            static readonly int m_T = Shader.PropertyToID("T");

            private Material material;

            public Glitch_Pass(Material material)
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
            float T;
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (material == null) return;

                // Use the Volume settings or the default settings if no Volume is set.
                var volumeComponent = VolumeManager.instance.stack.GetComponent<ChromaticNoise>();

                if (volumeComponent.mask.value != null)
                {
                    material.SetFloat(_FadeMultiplier, 1);
                    material.SetFloat(MaskThreshold, volumeComponent.maskEdgeFineTuning.value);
                    material.SetTexture(_Mask, volumeComponent.mask.value);
                    ParamSwitch(material, volumeComponent.maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
                }
                else
                {
                    material.SetFloat(_FadeMultiplier, 0);
                }

                material.SetFloat(Strength, volumeComponent.amount.value);

                material.SetFloat(x, volumeComponent.x.value);
                material.SetFloat(y, volumeComponent.y.value);
                material.SetFloat(angleY, volumeComponent.z.value);
                material.SetFloat(Stretch, volumeComponent.stretch.value);
                material.SetFloat(Speed, volumeComponent.speed.value);

                material.SetFloat(mR, volumeComponent.rMultiplier.value);
                material.SetFloat(mG, volumeComponent.gMultiplier.value);
                material.SetFloat(mB, volumeComponent.bMultiplier.value);
                material.SetFloat(mB, volumeComponent.bMultiplier.value);
                material.SetFloat(Fade, volumeComponent.Fade.value);
                T += Time.time;
                material.SetFloat(m_T, T);


                UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                var colorCopyDescriptor = GetCopyPassTextureDescriptor(cameraData.cameraTargetDescriptor);

                TextureHandle copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);

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