using UnityEngine;


namespace FlexiSprite
{
    [CreateAssetMenu(menuName = "AllInOne Sprite Shader/Effect Preset", fileName = "NewEffectPreset")]
    public class AnimSpriteEffectPreset : ScriptableObject
    {
        [SerializeField, HideInInspector] string jsonData;
        [SerializeField, HideInInspector] bool hasData;

        public bool HasData => hasData;

        public void SaveFrom(AnimatedSpriteShader source)
        {
            if (!source) return;

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Save Effect Preset");
#endif
            jsonData = JsonUtility.ToJson(source);
            hasData = true;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public void ApplyTo(AnimatedSpriteShader target)
        {
            if (!target || !hasData || string.IsNullOrEmpty(jsonData)) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(target, "Apply Preset");
#endif
            JsonUtility.FromJsonOverwrite(jsonData, target);
            target.RefreshAfterPresetApply();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
            if (target.gameObject.scene.IsValid() && target.gameObject.scene.isLoaded)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
#endif
        }
    }
}
