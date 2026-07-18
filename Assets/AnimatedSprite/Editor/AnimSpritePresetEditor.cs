using UnityEngine;
using UnityEditor;


namespace FlexiSprite
{
    [CustomEditor(typeof(AnimSpriteEffectPreset))]
    public class AnimSpritePresetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            EditorGUILayout.Space(8);
        
            if (GUILayout.Button("Apply to Selected Manager", GUILayout.Height(28)))
            {
                var mgr = Selection.activeGameObject?.GetComponent<AnimatedSpriteShader>();
                if (mgr)
                {
                    Undo.RecordObject(mgr, "Apply Preset");
                    ((AnimSpriteEffectPreset)target).ApplyTo(mgr);
                    EditorUtility.SetDirty(mgr);
                }
                else
                {
                    EditorUtility.DisplayDialog("No Manager", "Select a GameObject with AnimatedSpriteShader first.", "OK");
                }
            }
        }
    }
}
