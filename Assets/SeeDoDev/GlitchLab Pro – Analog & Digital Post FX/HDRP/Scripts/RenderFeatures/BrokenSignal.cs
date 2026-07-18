#if GLP_HAS_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace GlitchLabPro
{
    [Serializable, VolumeComponentMenu("GlitchLab Pro/BrokenSignal")]
    public sealed class BrokenSignal : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Effect intensity.")]
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);
        [Range(0f, 2f), Tooltip("Effect speed.")]
        public NoInterpClampedFloatParameter speed = new NoInterpClampedFloatParameter(.1f, 0f, 1f);
        [Range(1f, 2f), Tooltip("block size (higher value = smaller blocks.")]
        public NoInterpClampedFloatParameter resolutionMultiplier = new NoInterpClampedFloatParameter(1f, 1f, 2f);
        [Range(0f, 1f), Tooltip("blocks width (max value makes effect fullscreen).")]
        public NoInterpClampedFloatParameter stretchMultiplier = new NoInterpClampedFloatParameter(0.88f, 0f, 1f);
        [Space]
        [Tooltip("Mask texture")]
        public TextureParameter mask = new TextureParameter(null);
        [Tooltip("If your mask texture is not alpha transparent, use a red channel.")]
        public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
        [Tooltip("Value to adjust mask edge thickness.")]
        public NoInterpClampedFloatParameter maskEdgeFineTuning = new NoInterpClampedFloatParameter(.15f, 0.000001f, 1f);

        Material m_Material;

        public bool IsActive() => m_Material != null && Intensity.value > 0;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "GlitchLabPro/BrokenSignal";
        static readonly int _trashFrame1Id = Shader.PropertyToID("_trashFrame1");
        static readonly int _trashFrame2Id = Shader.PropertyToID("_trashFrame2");
        static readonly int _trashFrameId = Shader.PropertyToID("_trashFrame");
        static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");
        static readonly int _Mask = Shader.PropertyToID("_Mask");
        bool done = false;
        Texture2D _noiseTexture;



        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume AnalogChromaGlitch is unable to load.");
        }
        float _time;

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            int _trashFrame1 = _trashFrame1Id;
            int _trashFrame2 = _trashFrame2Id;
            int _trashFrame = _trashFrameId;
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

            cmd.GetTemporaryRT(_trashFrame1, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.GetTemporaryRT(_trashFrame2, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);

            if (!done)
                SetUpResources(resolutionMultiplier.value);

            if (UnityEngine.Random.value > 1f - speed.value)
            {
                SetUpResources(resolutionMultiplier.value);
                UpdateNoiseTexture(resolutionMultiplier.value);
            }

            // Update trash frames.
            int fcount = Time.frameCount;

            if (fcount % 13 == 0) cmd.Blit(source, _trashFrame1);
            if (fcount % 73 == 0) cmd.Blit(source, _trashFrame2);

            _trashFrame = UnityEngine.Random.value > 0.3f ? _trashFrame1 : _trashFrame2;

            m_Material.SetFloat("_ColorIntensity", Intensity.value);

            if (_noiseTexture == null)
            {
                UpdateNoiseTexture(resolutionMultiplier.value);
            }

            m_Material.SetTexture("_NoiseTex", _noiseTexture);

            cmd.SetGlobalTexture("_TrashTex", _trashFrame);

            cmd.Blit(source, destination, m_Material, 0);
        }
        void SetUpResources(float g_2Res)
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

            UpdateNoiseTexture(g_2Res);
            done = true;
        }
        void UpdateNoiseTexture(float g_2Res)
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
                    if (UnityEngine.Random.value > stretchMultiplier.value) color = RandomColor();
                    _noiseTexture.SetPixel(x, y, color);
                }
            }

            _noiseTexture.Apply();
        }
        static Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
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