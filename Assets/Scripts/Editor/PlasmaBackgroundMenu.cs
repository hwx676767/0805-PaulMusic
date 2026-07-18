using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 右键 Hierarchy 或顶部 GameObject 菜单 → "蓝色等离子背景"，
    /// 一键在当前场景创建一个铺满屏幕的流动等离子背景。
    /// </summary>
    internal static class PlasmaBackgroundMenu
    {
        [MenuItem("GameObject/DeusEx/蓝色等离子背景", false, 10)]
        private static void CreatePlasmaBackground(MenuCommand command)
        {
            var go = new GameObject("PlasmaBackground", typeof(PlasmaBackground));
            GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Plasma Background");
            Selection.activeObject = go;
            if (go.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(go.scene);
        }
    }
}
