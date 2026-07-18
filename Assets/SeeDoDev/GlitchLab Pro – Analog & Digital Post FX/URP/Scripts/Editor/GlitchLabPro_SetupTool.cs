#if GLP_HAS_URP
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
namespace GlitchLabPro
{

    public class GlitchLabPro_SetupWindow : EditorWindow
    {
        public Texture2D icon;
        [MenuItem("Tools/Glitch Lab Pro Setup Tool")]
        public static void ShowWindow()
        {
            GetWindow<GlitchLabPro_SetupWindow>("Glitch Lab Pro");
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (icon)
                GUILayout.Label(icon);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("With this tool you can add or remove all Glitch Lab Pro Render Features to your current Renderer.", MessageType.Info);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                AddRF();
            }
            if (GUILayout.Button("Remove"))
            {
                RemoveRF();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        static void AddRF()
        {
            addRendererFeature<ChromaticNoise_Feature>();
            addRendererFeature<BrokenSignal_Feature>();
            addRendererFeature<AnalogChromaGlitch_Feature>();
            addRendererFeature<DataGlitch_Feature>();
            addRendererFeature<RGBPulse_Feature>();
            addRendererFeature<LineShakeGlitch_Feature>();
            addRendererFeature<ScanlineJitter_Feature>();
            addRendererFeature<SignalCrushGlitch_Feature>();
            addRendererFeature<DigitalGlitch_Feature>();
            addRendererFeature<SignalShock_Feature>();
            addRendererFeature<NoiseEdgeShift_Feature>();
            addRendererFeature<VHSNoise_Feature>();
            addRendererFeature<VHSTrackingGlitch_Feature>();
            addRendererFeature<CompressionCodecGlitch_Feature>();
            addRendererFeature<RGBDesyncGlitch_Feature>();
            addRendererFeature<UVEchoGlitch_Feature>();
            addRendererFeature<DTVSignalBreakup_Feature>();
            addRendererFeature<RFInterferenceGlitch_Feature>();
            addRendererFeature<MosaicGlitch_Feature>();
            addRendererFeature<ColorFoldGlitch_Feature>();
            addRendererFeature<SoftRGBGlitch_Feature>();
            addRendererFeature<DecoderSlipGlitch_Feature>();
            addRendererFeature<GlitchLabCustomTexture_Feature>();
        }

        static void RemoveRF()
        {
            removeRendererFeature<ChromaticNoise_Feature>();
            removeRendererFeature<BrokenSignal_Feature>();
            removeRendererFeature<AnalogChromaGlitch_Feature>();
            removeRendererFeature<DataGlitch_Feature>();
            removeRendererFeature<RGBPulse_Feature>();
            removeRendererFeature<LineShakeGlitch_Feature>();
            removeRendererFeature<ScanlineJitter_Feature>();
            removeRendererFeature<SignalCrushGlitch_Feature>();
            removeRendererFeature<DigitalGlitch_Feature>();
            removeRendererFeature<SignalShock_Feature>();
            removeRendererFeature<NoiseEdgeShift_Feature>();
            removeRendererFeature<VHSNoise_Feature>();
            removeRendererFeature<VHSTrackingGlitch_Feature>();
            removeRendererFeature<CompressionCodecGlitch_Feature>();
            removeRendererFeature<RGBDesyncGlitch_Feature>();
            removeRendererFeature<UVEchoGlitch_Feature>();
            removeRendererFeature<DTVSignalBreakup_Feature>();
            removeRendererFeature<RFInterferenceGlitch_Feature>();
            removeRendererFeature<MosaicGlitch_Feature>();
            removeRendererFeature<ColorFoldGlitch_Feature>();
            removeRendererFeature<SoftRGBGlitch_Feature>();
            removeRendererFeature<DecoderSlipGlitch_Feature>();
            removeRendererFeature<GlitchLabCustomTexture_Feature>();
        }

        static void addRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var handledDataObjects = new List<ScriptableRendererData>();

            // Fetch the current URP asset from GraphicsSettings
            var asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (asset == null)
            {
                Debug.LogError("No Universal Render Pipeline is currently active.");
                return;
            }

            // Get all renderer data from the URP asset
            var rendererDataList = getRendererDataList(asset);

            foreach (var data in rendererDataList)
            {
                if (data == null)
                    continue;

                if (handledDataObjects.Contains(data))
                    continue;

                handledDataObjects.Add(data);

                // Create & add feature if not yet existing
                bool found = false;
                foreach (var feature in data.rendererFeatures)
                {
                    if (feature is T)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // Create the feature
                    var feature = ScriptableObject.CreateInstance<T>();
                    feature.name = typeof(T).Name;

                    // Add it to the renderer data.
                    addRenderFeature(data, feature);

                    Debug.Log("Added render feature '" + feature.name + "' to " + data.name + ". Hope that's okay <3.");
                }
            }
        }

        static void removeRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var handledDataObjects = new List<ScriptableRendererData>();

            // Fetch the current URP asset from GraphicsSettings
            var asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (asset == null)
            {
                Debug.LogError("No Universal Render Pipeline is currently active.");
                return;
            }

            // Get all renderer data from the URP asset
            var rendererDataList = getRendererDataList(asset);

            foreach (var data in rendererDataList)
            {
                if (data == null)
                    continue;

                if (handledDataObjects.Contains(data))
                    continue;

                handledDataObjects.Add(data);

                // Collect indices and features to remove
                var indicesToRemove = new List<int>();
                var featuresToRemove = new List<ScriptableRendererFeature>();

                for (int i = 0; i < data.rendererFeatures.Count; i++)
                {
                    if (data.rendererFeatures[i] is T)
                    {
                        indicesToRemove.Add(i);
                        featuresToRemove.Add(data.rendererFeatures[i]);
                    }
                }

                if (indicesToRemove.Count > 0)
                {
                    // Let's mirror what Unity does.
                    var serializedObject = new SerializedObject(data);

                    var renderFeaturesProp = serializedObject.FindProperty("m_RendererFeatures");
                    var renderFeaturesMapProp = serializedObject.FindProperty("m_RendererFeatureMap");

                    serializedObject.Update();

                    // Remove features starting from the end to avoid index issues
                    for (int i = indicesToRemove.Count - 1; i >= 0; i--)
                    {
                        int index = indicesToRemove[i];

                        // Remove from rendererFeatures list
                        renderFeaturesProp.DeleteArrayElementAtIndex(index);
                        renderFeaturesMapProp.DeleteArrayElementAtIndex(index);
                    }

                    serializedObject.ApplyModifiedProperties();

                    // Remove actual ScriptableObject assets
                    foreach (var feature in featuresToRemove)
                    {
                        if (feature != null)
                        {
                            // Log before destroying the feature
                            Debug.Log("Removed render feature '" + feature.name + "' from " + data.name + ".");

                            if (EditorUtility.IsPersistent(feature))
                            {
                                // Remove the sub-asset from the asset database
                                UnityEngine.Object.DestroyImmediate(feature, true);
                            }
                        }
                    }

                    // Save the asset database after all features are destroyed
                    AssetDatabase.SaveAssets();
                }
            }
        }



        static ScriptableRendererData[] getRendererDataList(UniversalRenderPipelineAsset asset)
        {
            if (asset)
            {
                ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset)
                        .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(asset);

                return rendererDataList;
            }
            else
            {
                Debug.LogError("No Universal Render Pipeline is currently active.");
                return null;
            }
        }

        /// <summary>
        /// Based on Unity add feature code.
        /// See: AddComponent() in https://github.com/Unity-Technologies/Graphics/blob/d0473769091ff202422ad13b7b764c7b6a7ef0be/com.unity.render-pipelines.universal/Editor/ScriptableRendererDataEditor.cs#180
        /// </summary>
        /// <param name="data"></param>
        /// <param name="feature"></param>
        static void addRenderFeature(ScriptableRendererData data, ScriptableRendererFeature feature)
        {
            // Let's mirror what Unity does.
            var serializedObject = new SerializedObject(data);

            var renderFeaturesProp = serializedObject.FindProperty("m_RendererFeatures"); // Let's hope they don't change these.
            var renderFeaturesMapProp = serializedObject.FindProperty("m_RendererFeatureMap");

            serializedObject.Update();

            // Store this new effect as a sub-asset so we can reference it safely afterwards.
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(data))
                AssetDatabase.AddObjectToAsset(feature, data);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out var guid, out long localId);

            // Grow the list first, then add - that's how serialized lists work in Unity
            renderFeaturesProp.arraySize++;
            var componentProp = renderFeaturesProp.GetArrayElementAtIndex(renderFeaturesProp.arraySize - 1);
            componentProp.objectReferenceValue = feature;

            // Update GUID Map
            renderFeaturesMapProp.arraySize++;
            var guidProp = renderFeaturesMapProp.GetArrayElementAtIndex(renderFeaturesMapProp.arraySize - 1);
            guidProp.longValue = localId;

            // Force save / refresh
            if (EditorUtility.IsPersistent(data))
            {
                AssetDatabase.SaveAssetIfDirty(data);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
#endif