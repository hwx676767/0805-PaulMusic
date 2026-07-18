using UnityEngine;
using UnityEditor;


namespace FlexiSprite
{
    public static class AnimSpriteMenuItems
    {
        [MenuItem("Assets/Create/Animated Sprite/Material (Standard)", false, 300)]
        static void CreateStandardMaterial()
        {
            var s = Shader.Find("AnimatedSprite/Standard");
            if (!s) { Debug.LogError("AnimatedSprite/Standard shader not found"); return; }
            var mat = new Material(s) { name = "AnimSprite_New" };
            ProjectWindowUtil.CreateAsset(mat, "AnimSprite_New.mat");
        }

        [MenuItem("Assets/Create/Animated Sprite/Material (URP 2D Lit)", false, 301)]
        static void CreateURP2DMaterial()
        {
            var s = Shader.Find("AnimatedSprite/2D Lit URP");
            if (!s) { Debug.LogError("AnimatedSprite/2D Lit URP shader not found"); return; }
            var mat = new Material(s) { name = "AnimSprite_URP2D" };
            ProjectWindowUtil.CreateAsset(mat, "AnimSprite_URP2D.mat");
        }

        [MenuItem("Assets/Create/Animated Sprite/Effect Preset", false, 310)]
        static void CreatePreset()
        {
            var preset = ScriptableObject.CreateInstance<AnimSpriteEffectPreset>();
            ProjectWindowUtil.CreateAsset(preset, "EffectPreset.asset");
        }

        [MenuItem("GameObject/Animated Sprite/Add Manager", false, 10)]
        static void AddManagerToSelected()
        {
            if (!Selection.activeGameObject) return;
            var go = Selection.activeGameObject;
            if (!go.GetComponent<AnimatedSpriteShader>())
            {
                Undo.AddComponent<AnimatedSpriteShader>(go);
            }
        }

        [MenuItem("CONTEXT/AnimatedSpriteShader/Save as Effect Preset")]
        static void SaveAsPreset(MenuCommand cmd)
        {
            var mgr = cmd.context as AnimatedSpriteShader;
            if (!mgr) return;
            var path = EditorUtility.SaveFilePanelInProject("Save Preset", "EffectPreset", "asset", "Save effect preset");
            if (string.IsNullOrEmpty(path)) return;
            var preset = ScriptableObject.CreateInstance<AnimSpriteEffectPreset>();
            preset.SaveFrom(mgr);
            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AnimSprite] Preset saved to {path}");
        }

        [MenuItem("CONTEXT/AnimatedSpriteShader/Load Effect Preset")]
        static void LoadPreset(MenuCommand cmd)
        {
            var mgr = cmd.context as AnimatedSpriteShader;
            if (!mgr) return;
            var path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = "Assets" + path.Substring(Application.dataPath.Length);
            var preset = AssetDatabase.LoadAssetAtPath<AnimSpriteEffectPreset>(path);
            if (preset)
            {
                Undo.RecordObject(mgr, "Load Preset");
                preset.ApplyTo(mgr);
                EditorUtility.SetDirty(mgr);
            }
        }

        [MenuItem("CONTEXT/AnimatedSpriteShader/Copy Material Settings")]
        static void CopyMaterial(MenuCommand cmd)
        {
            var mgr = cmd.context as AnimatedSpriteShader;
            if (!mgr) return;
            var rend = mgr.GetComponent<Renderer>();
            if (!rend || !rend.sharedMaterial) return;
            _copiedMaterial = rend.sharedMaterial;
            Debug.Log("[AnimSprite] Material settings copied.");
        }

        [MenuItem("CONTEXT/AnimatedSpriteShader/Paste Material Settings")]
        static void PasteMaterial(MenuCommand cmd)
        {
            if (!_copiedMaterial) { Debug.LogWarning("Nothing copied"); return; }
            var mgr = cmd.context as AnimatedSpriteShader;
            if (!mgr) return;
            var rend = mgr.GetComponent<Renderer>();
            if (!rend) return;
            Undo.RecordObject(rend, "Paste Material");
            rend.sharedMaterial.CopyPropertiesFromMaterial(_copiedMaterial);
            mgr.MarkDirty();
            Debug.Log("[AnimSprite] Material settings pasted.");
        }

        static Material _copiedMaterial;
    }
}
