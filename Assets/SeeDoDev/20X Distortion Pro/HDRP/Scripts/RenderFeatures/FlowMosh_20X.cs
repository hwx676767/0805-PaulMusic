#if DistortionsX_HAS_HDRP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace DistortionsPro_20X
{
    [Serializable, VolumeComponentMenu("20X Distortions Pro/Flow Mosh")]
    public sealed class FlowMosh_20X : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Global blend between original image (0) and full datamosh (1).")]
        public ClampedFloatParameter Blend = new ClampedFloatParameter(0f, 0f, 1f);

        [Header("Main")]
        [Tooltip("Pixels whose flow magnitude is below this value remain untouched.")]
        public NoInterpClampedFloatParameter FlowThreshold = new NoInterpClampedFloatParameter(0.9f, 0f, 0.9999f);
        public NoInterpClampedFloatParameter FlowGain = new NoInterpClampedFloatParameter(0.8f, 0f, 2f);
        public NoInterpClampedFloatParameter FlowXScale = new NoInterpClampedFloatParameter(0.5f, 0f, 1f);

        [Tooltip("Minimum brightness change that triggers the effect (higher = stronger, lower = weaker).")]
        public NoInterpClampedFloatParameter BrightnessThreshold = new NoInterpClampedFloatParameter(0.4f, 0f, 1f);

        [Tooltip("Boost for very small brightness differences.")]
        public NoInterpClampedFloatParameter MicroContrastBoost = new NoInterpClampedFloatParameter(0.4f, 0f, 1f);

        [Header("Block Grid")]
        [Tooltip("Size of snap blocks in UV space (≈ 0.002 is ~2 px at 1080p).")]
        public NoInterpClampedFloatParameter BlockSize = new NoInterpClampedFloatParameter(0.002f, 0.0001f, 0.2f);

        [Header("Noise")]
        [Tooltip("Blue-noise wobble amplitude (0 = none, 1 = strong).")]
        public NoInterpClampedFloatParameter NoiseAmplitude = new NoInterpClampedFloatParameter(0.9f, 0f, 1f);

        [Header("Chroma Shift")]
        [Tooltip("Chroma glitch: shift red channel.")]
        public NoInterpClampedFloatParameter ChromaShift = new NoInterpClampedFloatParameter(0.5f, 0f, 2f);


        // -------------------- Unity HDRP hooks --------------------

        public bool IsActive() => active && m_Material != null && Blend.value > 0f;


        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "DistortionsPro_20X/FlowMoshHDRP";

        Material m_Material;

        RTHandle m_History;
        bool m_HistoryValid;

        static readonly int _BrightnessThresholdID = Shader.PropertyToID("_BrightnessThreshold");
        static readonly int _MicroContrastBoostID = Shader.PropertyToID("_MicroContrastBoost");
        static readonly int _ThresholdID = Shader.PropertyToID("_Threshold");
        static readonly int _SizeID = Shader.PropertyToID("_Size");
        static readonly int _NoiseAmpID = Shader.PropertyToID("_NoiseAmp");
        static readonly int _ChromaShiftID = Shader.PropertyToID("ChromaShift");
        static readonly int _FadeID = Shader.PropertyToID("fade");
        static readonly int _FlowGainID = Shader.PropertyToID("FLOW_GAIN");
        static readonly int _FlowXScaleID = Shader.PropertyToID("_FlowXScale");
        static readonly int _COffsetID = Shader.PropertyToID("C_Offset");

        static readonly int _MainTexID = Shader.PropertyToID("_MainTex");
        static readonly int _BlitTextureID = Shader.PropertyToID("_BlitTexture");
        static readonly int _PreviousTexID = Shader.PropertyToID("_PreviousTex");

        public override void Setup()
        {
            var shader = Shader.Find(kShaderName);
            if (shader != null)
                m_Material = new Material(shader);
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume FlowMoshHDRPVolume is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            EnsureHistory(cmd, source);

            // If effect was off / history invalidated, make history = current frame to avoid stale feedback
            if (!m_HistoryValid)
            {
                CopyAllSlices(cmd, source, m_History);
                m_HistoryValid = true;
            }

            m_Material.SetFloat(_BrightnessThresholdID, 1f - BrightnessThreshold.value);
            m_Material.SetFloat(_MicroContrastBoostID, 1f - MicroContrastBoost.value);
            m_Material.SetFloat(_ThresholdID, 1f - FlowThreshold.value);
            m_Material.SetFloat(_SizeID, BlockSize.value);
            m_Material.SetFloat(_NoiseAmpID, NoiseAmplitude.value);
            m_Material.SetFloat(_ChromaShiftID, ChromaShift.value);
            m_Material.SetFloat(_FadeID, Blend.value);
            m_Material.SetFloat(_FlowGainID, FlowGain.value);
            m_Material.SetFloat(_FlowXScaleID, FlowXScale.value);
            m_Material.SetFloat(_COffsetID, 1000f);

            m_Material.SetTexture(_BlitTextureID, source);
            m_Material.SetTexture(_PreviousTexID, m_History);

            cmd.SetGlobalTexture(_PreviousTexID, m_History);

            cmd.Blit(source, destination, m_Material, 0);


            // Update history = output
            CopyAllSlices(cmd, destination, m_History);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);

            if (m_History != null)
                RTHandles.Release(m_History);

            m_History = null;
            m_HistoryValid = false;
        }

        void EnsureHistory(CommandBuffer cmd, RTHandle source)
        {
            if (source == null || source.rt == null)
                return;

            bool needAlloc =
                m_History == null ||
                m_History.rt == null ||
                m_History.rt.graphicsFormat != source.rt.graphicsFormat ||
                m_History.rt.dimension != source.rt.dimension ||
                m_History.rt.volumeDepth != source.rt.volumeDepth;

            if (!needAlloc)
                return;

            if (m_History != null)
                RTHandles.Release(m_History);

            // XR-aware allocation + dynamic res friendly
            m_History = RTHandles.Alloc(
                Vector2.one,
                slices: TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: source.rt.graphicsFormat,
                useDynamicScale: true,
                filterMode: FilterMode.Bilinear,
                wrapMode: TextureWrapMode.Clamp,
                name: "_FlowMosh_History_HDRP"
            );

            m_HistoryValid = false;
        }
        static void CopyAllSlices(CommandBuffer cmd, RTHandle src, RTHandle dst)
        {
            if (src == null || dst == null || src.rt == null || dst.rt == null)
                return;

            // For XR (Tex2DArray) copy slice-by-slice
            int slices = Math.Max(1, TextureXR.slices);
            if (slices > 1 || src.rt.dimension == TextureDimension.Tex2DArray || dst.rt.dimension == TextureDimension.Tex2DArray)
            {
                for (int s = 0; s < slices; s++)
                    cmd.CopyTexture(src.rt, s, 0, dst.rt, s, 0);
            }
            else
            {
                cmd.CopyTexture(src.rt, dst.rt);
            }
        }
    }
}
#endif
