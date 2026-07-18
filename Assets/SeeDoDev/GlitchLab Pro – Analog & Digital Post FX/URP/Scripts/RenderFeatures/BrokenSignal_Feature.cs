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
    public class BrokenSignal_Feature : ScriptableRendererFeature
    {
        [SerializeField]
        private Shader m_Shader;
        private Material material;
        Glitch_Pass Glitch_RenderPass;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

        public override void Create()
        {
            m_Shader = Shader.Find("GlitchLabPro/BrokenSignalURP");
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
            BrokenSignal myVolume = VolumeManager.instance.stack?.GetComponent<BrokenSignal>();
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
            bool done = false;
            private Material material;

            Texture2D _noiseTexture;


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

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (material == null) return;

                // Use the Volume settings or the default settings if no Volume is set.
                var volumeComponent = VolumeManager.instance.stack.GetComponent<BrokenSignal>();

                UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                UniversalRenderer renderer = (UniversalRenderer)cameraData.renderer;
                var colorCopyDescriptor = GetCopyPassTextureDescriptor(cameraData.cameraTargetDescriptor);
                TextureHandle copiedColor = TextureHandle.nullHandle;
                copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);


                TextureHandle _trashFrame = TextureHandle.nullHandle;
                TextureHandle _trashFrame1 = TextureHandle.nullHandle;
                TextureHandle _trashFrame2 = TextureHandle.nullHandle;
                _trashFrame = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);
                _trashFrame1 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);
                _trashFrame2 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CustomPostPassColorCopy", false);

                if (!done)
                    SetUpResources(volumeComponent.resolutionMultiplier.value, volumeComponent);

                if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, volumeComponent.speed.value))
                {
                    SetUpResources(volumeComponent.resolutionMultiplier.value, volumeComponent);
                    UpdateNoiseTexture(volumeComponent.resolutionMultiplier.value, volumeComponent);
                }

                // Update trash frames.
                int fcount = Time.frameCount;

                if (fcount % 13 == 0)
                {

                    using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("CustomPostPass_CopyColor", out var passData, profilingSampler))
                    {
                        passData.inputTexture = resourcesData.activeColorTexture;
                        builder.UseTexture(resourcesData.activeColorTexture, AccessFlags.Read);
                        builder.SetRenderAttachment(_trashFrame1, 0, AccessFlags.Write);
                        builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => ExecuteCopyColorPass(data, context));
                    }
                }

                if (fcount % 73 == 0)
                {

                    using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("CustomPostPass_CopyColor", out var passData, profilingSampler))
                    {
                        passData.inputTexture = resourcesData.activeColorTexture;
                        builder.UseTexture(resourcesData.activeColorTexture, AccessFlags.Read);
                        builder.SetRenderAttachment(_trashFrame2, 0, AccessFlags.Write);
                        builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => ExecuteCopyColorPass(data, context));
                    }
                }

                _trashFrame = UnityEngine.Random.value > 0.5f ? _trashFrame1 : _trashFrame2;

                material.SetFloat("_ColorIntensity", volumeComponent.Intensity.value);

                if (_noiseTexture == null)
                {
                    UpdateNoiseTexture(volumeComponent.resolutionMultiplier.value, volumeComponent);
                }
                if (volumeComponent.mask.value != null)
                {
                    material.SetFloat(_FadeMultiplier, 1);
                    material.SetFloat("_adj", volumeComponent.maskEdgeFineTuning.value);
                    material.SetTexture(_Mask, volumeComponent.mask.value);
                    ParamSwitch(material, volumeComponent.maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
                }
                else
                {
                    material.SetFloat(_FadeMultiplier, 0);
                }

                material.SetTexture("_NoiseTex", _noiseTexture);

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

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteMainPass(data, context,0));
                }
            }
            void SetUpResources(float g_2Res, BrokenSignal volumeComponent)
            {

                if (done)
                    return;
                Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 62));
                _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
                {

                    hideFlags = HideFlags.DontSave,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point
                };

                UpdateNoiseTexture(g_2Res, volumeComponent);
                done = true;
            }
            void UpdateNoiseTexture(float g_2Res, BrokenSignal volumeComponent)
            {
                Color color = RandomColor();
                if (_noiseTexture == null)
                {
                    Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
                    _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false);
                }
                for (var y = 0; y < _noiseTexture.height; y++)
                {
                    for (var x = 0; x < _noiseTexture.width; x++)
                    {
                        if (UnityEngine.Random.value > volumeComponent.stretchMultiplier.value) color = RandomColor();
                        _noiseTexture.SetPixel(x, y, color);
                    }
                }

                _noiseTexture.Apply();
            }
            static Color RandomColor()
            {
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }
        }
    }
}
#endif