using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FlexiSprite
{
    [ExecuteAlways]
    public class AnimatedSpriteUpdater : MonoBehaviour
    {
        private static AnimatedSpriteUpdater _instance;
        private static readonly List<AnimatedSpriteShader> _activeSprites = new List<AnimatedSpriteShader>(256);
        private static readonly HashSet<AnimatedSpriteShader> _registeredSet = new HashSet<AnimatedSpriteShader>();

        private static int _liveSpriteCount;
        private static bool _frustumCullEnabled;
        private static Plane[] _frustumPlanes;
        private static int _lastUpdatedFrame = -1;

#if UNITY_EDITOR
        private static bool _editorUpdateHooked;
#endif

        public static bool FrustumCullEnabled
        {
            get => _frustumCullEnabled;
            set => _frustumCullEnabled = value;
        }

        private void Awake()
        {
            RefreshInstance(this);
        }

        private void OnEnable()
        {
            RefreshInstance(this);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static void NotifyCreated()
        {
            _liveSpriteCount++;
            EnsureInstance();
        }

        public static void NotifyDestroyed()
        {
            _liveSpriteCount = Mathf.Max(0, _liveSpriteCount - 1);

            if (_liveSpriteCount > 0 || _instance == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            var managerGO = _instance.gameObject;
            _instance = null;

            Destroy(managerGO);
        }

        public static void Register(AnimatedSpriteShader sprite)
        {
            if (sprite == null)
                return;

            if (_registeredSet.Add(sprite))
                _activeSprites.Add(sprite);

            EnsureInstance();
#if UNITY_EDITOR
            EnsureEditorUpdateHook();
#endif
        }

        public static void Unregister(AnimatedSpriteShader sprite)
        {
            if (sprite == null)
                return;

            sprite.ProcessUpdate();

            if (_registeredSet.Remove(sprite))
                _activeSprites.Remove(sprite);

#if UNITY_EDITOR
            ReleaseEditorUpdateHookIfUnused();
#endif
        }

#if UNITY_EDITOR
        private static void EnsureEditorUpdateHook()
        {
            if (Application.isPlaying || _editorUpdateHooked)
                return;

            EditorApplication.update += EditorVisualUpdate;
            _editorUpdateHooked = true;
        }

        private static void ReleaseEditorUpdateHookIfUnused()
        {
            if (Application.isPlaying || !_editorUpdateHooked || _registeredSet.Count > 0)
                return;

            EditorApplication.update -= EditorVisualUpdate;
            _editorUpdateHooked = false;
        }

        private static void EditorVisualUpdate()
        {
            if (Application.isPlaying)
                return;

            bool refreshedAny = false;
            float editorTime = (float)EditorApplication.timeSinceStartup;

            for (int i = _activeSprites.Count - 1; i >= 0; i--)
            {
                var sprite = _activeSprites[i];
                if (sprite == null)
                {
                    _activeSprites.RemoveAt(i);
                    continue;
                }

                if (!sprite.isActiveAndEnabled)
                    continue;

                refreshedAny |= sprite.EditorVisualRefreshInEditMode(editorTime);
            }

            if (!refreshedAny)
                return;

            // Force a visual redraw only. No serialized data is dirtied and the normal push path is not run.
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }

#endif

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                CleanupDuplicateManagers(_instance);
                return;
            }

            RefreshInstance(null);

            if (_instance != null)
                return;

#if UNITY_EDITOR
            // Opening a scene in edit mode must not create hidden manager objects.
            // That marks scenes dirty in some Unity versions even when the object uses DontSave flags.
            if (!Application.isPlaying)
                return;
#endif

            var go = new GameObject("[ AnimatedSprite_Manager ]");
            _instance = go.AddComponent<AnimatedSpriteUpdater>();

            if (Application.isPlaying)
                DontDestroyOnLoad(go);

            CleanupDuplicateManagers(_instance);
        }

        private static void RefreshInstance(AnimatedSpriteUpdater fallback)
        {
            var bestInstance = FindBestInstance(fallback);

            if (bestInstance == null)
                return;

            _instance = bestInstance;
            CleanupDuplicateManagers(_instance);
        }

        private static AnimatedSpriteUpdater FindBestInstance(AnimatedSpriteUpdater fallback)
        {
            var updaters = Resources.FindObjectsOfTypeAll<AnimatedSpriteUpdater>();

            AnimatedSpriteUpdater bestInstance = null;
            int bestScore = int.MinValue;

            for (int i = 0; i < updaters.Length; i++)
            {
                var updater = updaters[i];

                if (!IsUsableInstance(updater))
                    continue;

                int score = GetInstanceScore(updater);

                if (score <= bestScore)
                    continue;

                bestInstance = updater;
                bestScore = score;
            }

            return bestInstance != null ? bestInstance : fallback;
        }

        private static bool IsUsableInstance(AnimatedSpriteUpdater updater)
        {
            if (updater == null)
                return false;

#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(updater))
                return false;
#endif

            return true;
        }

        private static int GetInstanceScore(AnimatedSpriteUpdater updater)
        {
            int score = 0;

            if (updater == _instance)
                score += 10;

            if (updater.gameObject.hideFlags == HideFlags.None)
                score += 1000;

            if (updater.gameObject.scene.IsValid())
                score += 100;

            if (updater.gameObject.scene.isLoaded)
                score += 100;

            if (updater.gameObject.activeInHierarchy)
                score += 10;

            if (updater.enabled)
                score += 10;

            return score;
        }

        private static void CleanupDuplicateManagers(AnimatedSpriteUpdater keeper)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            var updaters = Resources.FindObjectsOfTypeAll<AnimatedSpriteUpdater>();

            for (int i = 0; i < updaters.Length; i++)
            {
                var updater = updaters[i];

                if (!IsUsableInstance(updater) || updater == keeper)
                    continue;

                DestroyDuplicate(updater);
            }
        }

        private static void DestroyDuplicate(AnimatedSpriteUpdater updater)
        {
            if (updater == null)
                return;

            var go = updater.gameObject;

            if (Application.isPlaying)
                Destroy(go);
            else
                DestroyImmediate(go);
        }

        private void LateUpdate()
        {
            if (_instance == null)
                RefreshInstance(this);

            if (_instance != this)
                return;

            if (_lastUpdatedFrame == Time.frameCount)
                return;

            _lastUpdatedFrame = Time.frameCount;

            if (_frustumCullEnabled && Camera.main)
                _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            for (int i = _activeSprites.Count - 1; i >= 0; i--)
            {
                var sprite = _activeSprites[i];

                if (sprite == null)
                {
                    _activeSprites.RemoveAt(i);
                    continue;
                }

                if (IsCulled(sprite))
                    continue;

                sprite.ProcessUpdate();
            }
        }

        private static bool IsCulled(AnimatedSpriteShader sprite)
        {
            if (!_frustumCullEnabled || _frustumPlanes == null)
                return false;

            var rend = sprite.GetComponent<Renderer>();
            return rend && !GeometryUtility.TestPlanesAABB(_frustumPlanes, rend.bounds);
        }
    }
}
