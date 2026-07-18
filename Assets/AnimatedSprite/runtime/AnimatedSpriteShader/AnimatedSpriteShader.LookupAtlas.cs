using System;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

namespace FlexiSprite
{
    public partial class AnimatedSpriteShader
    {
        const int AIO_LOOKUP_WIDTH = 256;
        const int AIO_LOOKUP_ROWS = 5;

        enum PackedLookupRow
        {
            ColorRamp = 0,
            Gradient = 1,
            RadialGradient = 2,
            RetroPalette = 3,
            PaletteSwap = 4
        }

        [SerializeField, HideInInspector] Texture2D _aioPackedLookupBakedTex;
        [SerializeField, HideInInspector] int _aioPackedLookupBakedHash;

        [NonSerialized] Texture2D _aioPackedLookupTex;
        [NonSerialized] int _aioPackedLookupHash;
        [NonSerialized] bool _aioPackedLookupDirty = true;

        void PushPackedLookupAtlas(MaterialPropertyBlock block)
        {
            var tex = GetPackedLookupAtlas();
            if (tex != null)
            {
                block.SetTexture("_AIOPackedLookupTex", tex);
                block.SetVector("_AIOPackedLookupTex_TexelSize", new Vector4(1f / tex.width, 1f / tex.height, tex.width, tex.height));
            }

            PushPackedLookupRegions(block);
        }

        void PushPackedLookupAtlas(Material mat)
        {
            var tex = GetPackedLookupAtlas();
            if (tex != null)
            {
                mat.SetTexture("_AIOPackedLookupTex", tex);
                mat.SetVector("_AIOPackedLookupTex_TexelSize", new Vector4(1f / tex.width, 1f / tex.height, tex.width, tex.height));
            }

            PushPackedLookupRegions(mat);
        }

        void PushPackedLookupRegions(MaterialPropertyBlock block)
        {
            block.SetVector("_ColorRampTexRegion", LookupRegion(PackedLookupRow.ColorRamp));
            block.SetVector("_GradientTexRegion", LookupRegion(PackedLookupRow.Gradient));
            block.SetVector("_RadialGradTexRegion", LookupRegion(PackedLookupRow.RadialGradient));
            block.SetVector("_RetroPaletteTexRegion", LookupRegion(PackedLookupRow.RetroPalette));
            block.SetVector("_PaletteSwapTexRegion", LookupRegion(PackedLookupRow.PaletteSwap));
        }

        void PushPackedLookupRegions(Material mat)
        {
            mat.SetVector("_ColorRampTexRegion", LookupRegion(PackedLookupRow.ColorRamp));
            mat.SetVector("_GradientTexRegion", LookupRegion(PackedLookupRow.Gradient));
            mat.SetVector("_RadialGradTexRegion", LookupRegion(PackedLookupRow.RadialGradient));
            mat.SetVector("_RetroPaletteTexRegion", LookupRegion(PackedLookupRow.RetroPalette));
            mat.SetVector("_PaletteSwapTexRegion", LookupRegion(PackedLookupRow.PaletteSwap));
        }

        static Vector4 LookupRegion(PackedLookupRow row)
        {
            float h = 1f / AIO_LOOKUP_ROWS;
            return new Vector4(0f, (int)row * h, 1f, h);
        }

        Texture2D GetPackedLookupAtlas()
        {
            int hash = ComputePackedLookupHash();

#if !UNITY_EDITOR
            // Runtime builds must not depend on reading user textures. Use the editor-baked atlas when available.
            if (_aioPackedLookupBakedTex != null && _aioPackedLookupBakedHash == hash)
                return _aioPackedLookupBakedTex;
#endif

            if (_aioPackedLookupTex == null || _aioPackedLookupDirty || hash != _aioPackedLookupHash)
            {
                RebuildPackedLookupAtlas();
                _aioPackedLookupHash = hash;
                _aioPackedLookupDirty = false;
            }

            return _aioPackedLookupTex;
        }

        int ComputePackedLookupHash()
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + TextureHash(colorRampTexture);
                h = h * 31 + TextureHash(gradientBakedTexture);
                h = h * 31 + TextureHash(radialGradBakedTexture);
                h = h * 31 + TextureHash(retroPaletteTex);
                h = h * 31 + TextureHash(paletteSwapTexture);
                h = h * 31 + colorRampColorA.GetHashCode();
                h = h * 31 + colorRampColorB.GetHashCode();
                h = h * 31 + gradientColorA.GetHashCode();
                h = h * 31 + gradientColorB.GetHashCode();
                h = h * 31 + radialGradCenterColor.GetHashCode();
                h = h * 31 + radialGradEdgeColor.GetHashCode();
                return h;
            }
        }

        static int TextureHash(Texture2D tex)
        {
            if (!tex) return 0;
            unchecked
            {
                int h = tex.GetInstanceID();
                h = h * 31 + tex.width;
                h = h * 31 + tex.height;
                return h;
            }
        }

        void RebuildPackedLookupAtlas()
        {
            if (_aioPackedLookupTex == null || _aioPackedLookupTex.width != AIO_LOOKUP_WIDTH || _aioPackedLookupTex.height != AIO_LOOKUP_ROWS)
            {
                _aioPackedLookupTex = new Texture2D(AIO_LOOKUP_WIDTH, AIO_LOOKUP_ROWS, TextureFormat.RGBA32, false, true)
                {
                    name = "AIO Packed Lookup Atlas",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
                };
            }

            WriteLookupRow(PackedLookupRow.ColorRamp, colorRampTexture, t => Color.Lerp(colorRampColorA, colorRampColorB, t));
            WriteLookupRow(PackedLookupRow.Gradient, gradientBakedTexture, t => Color.Lerp(gradientColorA, gradientColorB, t));
            WriteLookupRow(PackedLookupRow.RadialGradient, radialGradBakedTexture, t => Color.Lerp(radialGradCenterColor, radialGradEdgeColor, t));
            WriteLookupRow(PackedLookupRow.RetroPalette, retroPaletteTex, IdentityLookup);
            WriteLookupRow(PackedLookupRow.PaletteSwap, paletteSwapTexture, IdentityLookup);

            _aioPackedLookupTex.Apply(false, false);
        }

        static Color IdentityLookup(float t) => new Color(t, t, t, 1f);

        void WriteLookupRow(PackedLookupRow row, Texture2D source, Func<float, Color> fallback)
        {
            int y = (int)row;
            Color[] rowPixels = TryReadLookupRow(source);

            for (int x = 0; x < AIO_LOOKUP_WIDTH; x++)
            {
                float u = AIO_LOOKUP_WIDTH <= 1 ? 0f : x / (float)(AIO_LOOKUP_WIDTH - 1);
                Color c = rowPixels != null ? rowPixels[x] : SampleLookupSourceReadable(source, u, fallback);
                _aioPackedLookupTex.SetPixel(x, y, c);
            }
        }

        static Color SampleLookupSourceReadable(Texture2D source, float u, Func<float, Color> fallback)
        {
            if (source)
            {
                try
                {
                    return source.GetPixelBilinear(Mathf.Clamp01(u), 0.5f);
                }
                catch
                {
                    // Non-readable source. In editor we use TryReadLookupRow() before this path.
                }
            }
            return fallback != null ? fallback(Mathf.Clamp01(u)) : IdentityLookup(u);
        }

        static Color[] TryReadLookupRow(Texture2D source)
        {
            if (!source) return null;

            // First try the cheap CPU-readable path.
            try
            {
                var pixels = new Color[AIO_LOOKUP_WIDTH];
                for (int x = 0; x < AIO_LOOKUP_WIDTH; x++)
                {
                    float u = AIO_LOOKUP_WIDTH <= 1 ? 0f : x / (float)(AIO_LOOKUP_WIDTH - 1);
                    pixels[x] = source.GetPixelBilinear(Mathf.Clamp01(u), 0.5f);
                }
                return pixels;
            }
            catch
            {
                // Fall through to editor GPU readback. Runtime builds cannot read non-readable textures.
            }

#if UNITY_EDITOR
            return EditorReadLookupRowGPU(source);
#else
            return null;
#endif
        }

#if UNITY_EDITOR
        static Color[] EditorReadLookupRowGPU(Texture2D source)
        {
            if (!source) return null;

            var previous = RenderTexture.active;
            var oldFilter = source.filterMode;
            var oldWrap = source.wrapMode;
            RenderTexture rt = null;
            Texture2D readable = null;

            try
            {
                source.filterMode = FilterMode.Bilinear;
                source.wrapMode = TextureWrapMode.Clamp;

                rt = RenderTexture.GetTemporary(AIO_LOOKUP_WIDTH, 1, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                rt.wrapMode = TextureWrapMode.Clamp;
                rt.filterMode = FilterMode.Bilinear;

                Graphics.Blit(source, rt);
                RenderTexture.active = rt;

                readable = new Texture2D(AIO_LOOKUP_WIDTH, 1, TextureFormat.RGBA32, false, true);
                readable.ReadPixels(new Rect(0, 0, AIO_LOOKUP_WIDTH, 1), 0, 0, false);
                readable.Apply(false, false);

                var colors = readable.GetPixels(0, 0, AIO_LOOKUP_WIDTH, 1);
                return colors;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AnimSprite] Could not GPU-read lookup texture '{source.name}'. Falling back to procedural row. {e.Message}");
                return null;
            }
            finally
            {
                source.filterMode = oldFilter;
                source.wrapMode = oldWrap;
                RenderTexture.active = previous;
                if (rt != null) RenderTexture.ReleaseTemporary(rt);
                if (readable != null) DestroyImmediate(readable);
            }
        }

        public void EditorBakePackedLookupAtlasAsset(string outputFolder = "Assets/AnimatedSprite/Generated/PackedLookupAtlases")
        {
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);

            int hash = ComputePackedLookupHash();
            RebuildPackedLookupAtlas();

            string sceneName = gameObject.scene.IsValid() ? gameObject.scene.name : "Scene";
            string safeName = MakeSafeAssetName($"{sceneName}_{gameObject.name}_{GetInstanceID()}_PackedLookup.asset");
            string path = AssetDatabase.GetAssetPath(_aioPackedLookupBakedTex);
            if (string.IsNullOrEmpty(path)) path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(outputFolder, safeName).Replace('\\', '/'));

            Texture2D assetTex = _aioPackedLookupBakedTex;
            if (assetTex == null || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(assetTex)))
            {
                assetTex = new Texture2D(AIO_LOOKUP_WIDTH, AIO_LOOKUP_ROWS, TextureFormat.RGBA32, false, true)
                {
                    name = Path.GetFileNameWithoutExtension(path),
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
                AssetDatabase.CreateAsset(assetTex, path);
            }

            assetTex.SetPixels(_aioPackedLookupTex.GetPixels());
            assetTex.wrapMode = TextureWrapMode.Clamp;
            assetTex.filterMode = FilterMode.Bilinear;
            assetTex.Apply(false, false);

            _aioPackedLookupBakedTex = assetTex;
            _aioPackedLookupBakedHash = hash;
            EditorUtility.SetDirty(assetTex);
            EditorUtility.SetDirty(this);
        }

        static string MakeSafeAssetName(string s)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

        public static void EditorBakePackedLookupAtlasesForBuild(BuildReport report = null)
        {
            const string folder = "Assets/AnimatedSprite/Generated/PackedLookupAtlases";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            int count = 0;
            var openScenes = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var sc = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (sc.IsValid() && sc.isLoaded && !string.IsNullOrEmpty(sc.path)) openScenes.Add(sc.path);
            }

            var scenePaths = EditorBuildSettings.scenes != null && EditorBuildSettings.scenes.Length > 0
                ? System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(System.Linq.Enumerable.Where(EditorBuildSettings.scenes, s => s.enabled), s => s.path))
                : System.Array.Empty<string>();

            foreach (var path in scenePaths)
            {
                bool wasOpen = openScenes.Contains(path);
                var scene = wasOpen ? EditorSceneManager.GetSceneByPath(path) : EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                bool dirty = false;

                foreach (var root in scene.GetRootGameObjects())
                {
                    var sprites = root.GetComponentsInChildren<AnimatedSpriteShader>(true);
                    foreach (var sprite in sprites)
                    {
                        sprite.EditorBakePackedLookupAtlasAsset(folder);
                        dirty = true;
                        count++;
                    }
                }

                if (dirty)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }

                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[AnimSprite] Baked {count} packed lookup atlas asset(s) for build.");
        }
#endif
    }
}
