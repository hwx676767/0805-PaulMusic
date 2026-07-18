using UnityEngine;
using UnityEditor;


namespace FlexiSprite
{
    public class AnimatedSpriteShaderGUI : ShaderGUI
    {
        bool _showRaw;

        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            EditorGUILayout.HelpBox(
                "This material is controlled by AnimatedSpriteShader.\n" +
                "Add that component to your GameObject to edit effects.", MessageType.Info);

            EditorGUILayout.Space(4);
            _showRaw = EditorGUILayout.Foldout(_showRaw, "Raw Properties", true);
            if(_showRaw)
            {
                EditorGUILayout.Space(2);
                foreach(var p in props)
                    editor.ShaderProperty(p, p.displayName);
            }
        }
    }
}
