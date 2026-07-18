using System.Reflection;
using UnityEngine;

/// <summary>
/// Rendering and keyword routing for AnimatedSpriteShader.
/// </summary>

namespace FlexiSprite
{
    public partial class AnimatedSpriteShader
    {
        private PushStrategy _pushStrategy;
        private bool _pushStrategyResolved;

        private PushStrategy ResolvePushStrategy()
        {
            if (!_pushStrategyResolved)
            {
                _pushStrategy = PipelineRouting.GetStrategy(_shaderVariant, _materialMode);
                _pushStrategyResolved = true;
            }
            return _pushStrategy;
        }

        private void InvalidatePushStrategy() => _pushStrategyResolved = false;

        // Group Keywords

        private int ComputeActiveGroupMask()
        {
            int mask = 0;
            var all = EffectRegistry.All;
            var type = GetType();
            for (int i = 0; i < all.Length; i++)
            {
                var fi = type.GetField(all[i].enableField, BindingFlags.Public | BindingFlags.Instance);
                if (fi != null && fi.FieldType == typeof(bool) && (bool)fi.GetValue(this))
                    mask |= 1 << (int)all[i].group;
            }
            return mask;
        }

        private void PushGroupKeywordsToMaterial(Material mat)
        {
            ApplyGroupKeywordMaskToMaterial(mat, ComputeActiveGroupMask());
        }

        private static void ApplyGroupKeywordMaskToMaterial(Material mat, int groupMask)
        {
            if (!mat) return;

            for (int g = 0; g < (int)EffectGroup._Count; g++)
            {
                string kw = EffectGroupInfo.Keywords[g];
                SetMaterialKeyword(mat, kw, (groupMask & (1 << g)) != 0);
            }
        }

        private static void SetMaterialKeyword(Material mat, string keyword, bool enabled)
        {
            if (!mat || string.IsNullOrEmpty(keyword)) return;

#if UNITY_2021_2_OR_NEWER
            if (mat.shader)
            {
                var localKeyword = new UnityEngine.Rendering.LocalKeyword(mat.shader, keyword);
                if (localKeyword.isValid)
                {
                    if (enabled) mat.EnableKeyword(localKeyword);
                    else mat.DisableKeyword(localKeyword);
                    return;
                }
            }
#endif
            if (enabled) mat.EnableKeyword(keyword);
            else mat.DisableKeyword(keyword);
        }

        private static void RebuildSharedGroupKeywords(int vi)
        {
            int combined = 0;
            foreach (var inst in _activeInstances)
            {
                if (!inst || inst._materialMode != MaterialMode.Shared || (int)inst._shaderVariant != vi) continue;
                combined |= inst.ComputeActiveGroupMask();
            }

            // Keep the cached source material in sync.
            if (_sharedMaterials.TryGetValue(vi, out var mat) && mat)
                ApplyGroupKeywordMaskToMaterial(mat, combined);

            // Also keep every renderer's currently assigned shared material in sync. In edit mode,
            // Unity can preserve or duplicate hidden scene-local generated materials across save/copy
            // operations. Updating only the static cached material leaves those renderer materials with
            // stale group keywords, which makes some effects vanish until play mode rebinds them.
            foreach (var inst in _activeInstances)
            {
                if (!inst || inst._materialMode != MaterialMode.Shared || (int)inst._shaderVariant != vi) continue;

                var rend = inst.Rend;
                if (rend && rend.sharedMaterial)
                    ApplyGroupKeywordMaskToMaterial(rend.sharedMaterial, combined);
            }
        }

        // PUSH (pipeline-aware)

        private void PushKeywordsToMaterial(Material mat) => PushGroupKeywordsToMaterial(mat);

        private static void RebuildSharedKeywords(int vi) => RebuildSharedGroupKeywords(vi);

        public bool IsGroupActive(EffectGroup g) => (ComputeActiveGroupMask() & (1 << (int)g)) != 0;

        public PushStrategy CurrentPushStrategy => ResolvePushStrategy();
    }
    }
