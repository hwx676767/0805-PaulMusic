#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;


namespace FlexiSprite
{
    public class AnimSpriteKeyframeEditor : EditorWindow
    {
        enum Mode { Float, Color, Vector }

        AnimatedSpriteShader _target;
        string _path, _label;
        Mode _mode;

        float _fromF, _midF, _toF;
        Color _fromC = Color.white, _midC = Color.grey, _toC = Color.white;
        Vector4 _fromV, _midV, _toV;
        bool _useMid;

        float _duration = 0.5f;
        int _easing;
        int _fps = 30;
        bool _loop, _pingPong;

        bool _clipPickerOpen;
        int _clipPickerIndex;

        static readonly string[] Easings = { "Linear", "Ease In", "Ease Out", "Ease In-Out", "Bounce", "Elastic" };

        public static void OpenFloat(AnimatedSpriteShader target, string path, string label, float val, float lo, float hi)
        {
            var w = CreateInstance<AnimSpriteKeyframeEditor>();
            w._target = target;
            w._path = path;
            w._label = label;
            w._mode = Mode.Float;
            w._fromF = val;
            w._midF = Mathf.Lerp(lo, hi, 0.5f);
            w._toF = hi > lo ? hi : val;
            w.titleContent = new GUIContent("Animate \u2014 " + label);
            w.minSize = new Vector2(360, 340);
            w.ShowUtility();
        }

        public static void OpenColor(AnimatedSpriteShader target, string path, string label, Color val)
        {
            var w = CreateInstance<AnimSpriteKeyframeEditor>();
            w._target = target;
            w._path = path;
            w._label = label;
            w._mode = Mode.Color;
            w._fromC = val;
            w._midC = Color.Lerp(val, Color.white, 0.5f);
            w._toC = Color.white;
            w.titleContent = new GUIContent("Animate \u2014 " + label);
            w.minSize = new Vector2(360, 360);
            w.ShowUtility();
        }

        public static void OpenVector(AnimatedSpriteShader target, string path, string label, Vector4 val)
        {
            var w = CreateInstance<AnimSpriteKeyframeEditor>();
            w._target = target;
            w._path = path;
            w._label = label;
            w._mode = Mode.Vector;
            w._fromV = val;
            w._toV = Vector4.zero;
            w.titleContent = new GUIContent("Animate \u2014 " + label);
            w.minSize = new Vector2(360, 400);
            w.ShowUtility();
        }

        void OnGUI()
        {
            if(!_target) { Close(); return; }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(_target.name + "  \u00B7  " + _path, EditorStyles.miniLabel);
            EditorGUILayout.Space(6);

            DrawValueFields();

            EditorGUILayout.Space(6);
            _duration = EditorGUILayout.Slider("Duration", _duration, 0.05f, 10f);
            _easing = EditorGUILayout.Popup("Easing", _easing, Easings);
            _fps = EditorGUILayout.IntSlider("Bake FPS", _fps, 12, 60);
            EditorGUILayout.Space(2);
            _loop = EditorGUILayout.Toggle("Loop", _loop);
            if(_loop) _pingPong = EditorGUILayout.Toggle("Ping Pong", _pingPong);

            EditorGUILayout.Space(4);
            if(_mode == Mode.Float) DrawCurvePreview();
            EditorGUILayout.Space(6);

            if(GUILayout.Button("Save as New Clip", GUILayout.Height(26)))
                SaveNewClip();

            EditorGUILayout.Space(2);

            DrawClipPicker();
        }

        void DrawValueFields()
        {
            _useMid = EditorGUILayout.Toggle("Use Mid Point", _useMid);

            switch(_mode)
            {
                case Mode.Float:
                    _fromF = EditorGUILayout.FloatField("From", _fromF);
                    if(_useMid) _midF = EditorGUILayout.FloatField("Mid", _midF);
                    _toF = EditorGUILayout.FloatField("To", _toF);
                    break;
                case Mode.Color:
                    _fromC = EditorGUILayout.ColorField("From", _fromC);
                    if(_useMid) _midC = EditorGUILayout.ColorField("Mid", _midC);
                    _toC = EditorGUILayout.ColorField("To", _toC);
                    break;
                case Mode.Vector:
                    _fromV = EditorGUILayout.Vector4Field("From", _fromV);
                    if(_useMid) _midV = EditorGUILayout.Vector4Field("Mid", _midV);
                    _toV = EditorGUILayout.Vector4Field("To", _toV);
                    break;
            }
        }

        void DrawCurvePreview()
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(48));
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.12f));

            Handles.color = new Color(0.3f, 0.6f, 1f);
            var prev = Vector3.zero;
            for(int i = 0; i <= 80; i++)
            {
                float t = i / 80f;
                float v = SampleNormalized(t);
                float lo = _useMid ? Mathf.Min(_fromF, Mathf.Min(_midF, _toF)) : Mathf.Min(_fromF, _toF);
                float hi = _useMid ? Mathf.Max(_fromF, Mathf.Max(_midF, _toF)) : Mathf.Max(_fromF, _toF);
                float range = hi - lo;
                float norm = range > 0.0001f ? (v - lo) / range : 0.5f;
                var pt = new Vector3(rect.x + t * rect.width, rect.yMax - norm * rect.height);
                if(i > 0) Handles.DrawLine(prev, pt);
                prev = pt;
            }
            Handles.color = Color.white;
        }

        void DrawClipPicker()
        {
            var animator = _target.GetComponent<Animator>();
            AnimationClip[] clips = null;
            if(animator && animator.runtimeAnimatorController)
                clips = animator.runtimeAnimatorController.animationClips;

            bool hasClips = clips != null && clips.Length > 0;
            var active = TryGetActiveClip();

            if(active)
            {
                if(GUILayout.Button("Add to: " + active.name, GUILayout.Height(26)))
                    InjectIntoClip(active);
            }

            if(hasClips)
            {
                _clipPickerOpen = EditorGUILayout.Foldout(_clipPickerOpen, "Pick from controller", true);
                if(_clipPickerOpen)
                {
                    var names = new string[clips.Length];
                    for(int i = 0; i < clips.Length; i++) names[i] = clips[i].name;
                    _clipPickerIndex = Mathf.Clamp(_clipPickerIndex, 0, clips.Length - 1);

                    EditorGUILayout.BeginHorizontal();
                    _clipPickerIndex = EditorGUILayout.Popup(_clipPickerIndex, names);
                    if(GUILayout.Button("Add", GUILayout.Width(40)))
                        InjectIntoClip(clips[_clipPickerIndex]);
                    EditorGUILayout.EndHorizontal();
                }
            }

            if(!active && !hasClips)
                EditorGUILayout.HelpBox("Open the Animation window or add an Animator to pick a clip.", MessageType.Info);
        }

        float Ease(float t)
        {
            switch(_easing)
            {
                case 1: return t * t;
                case 2: return 1f - (1f - t) * (1f - t);
                case 3: return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
                case 4:
                    float n = 7.5625f, d = 2.75f, x = 1f - t, b;
                    if(x < 1f/d) b = n*x*x;
                    else if(x < 2f/d) b = n*(x -= 1.5f/d)*x + 0.75f;
                    else if(x < 2.5f/d) b = n*(x -= 2.25f/d)*x + 0.9375f;
                    else b = n*(x -= 2.625f/d)*x + 0.984375f;
                    return 1f - b;
                case 5:
                    if(t <= 0f) return 0f;
                    if(t >= 1f) return 1f;
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (Mathf.PI * 2f) / 3f) + 1f;
                default: return t;
            }
        }

        float SampleNormalized(float rawT)
        {
            float t = rawT;
            if(_loop && _pingPong) { t = rawT * 2f; if(t > 1f) t = 2f - t; }

            float eased = Ease(t);

            if(_useMid)
            {
                if(_mode == Mode.Float)
                    return eased <= 0.5f
                        ? Mathf.Lerp(_fromF, _midF, eased * 2f)
                        : Mathf.Lerp(_midF, _toF, (eased - 0.5f) * 2f);
            }

            switch(_mode)
            {
                case Mode.Float: return Mathf.Lerp(_fromF, _toF, eased);
                default: return eased;
            }
        }

        float SampleFloat(float eased)
        {
            if(_useMid)
                return eased <= 0.5f
                    ? Mathf.Lerp(_fromF, _midF, eased * 2f)
                    : Mathf.Lerp(_midF, _toF, (eased - 0.5f) * 2f);
            return Mathf.Lerp(_fromF, _toF, eased);
        }

        Color SampleColor(float eased)
        {
            if(_useMid)
                return eased <= 0.5f
                    ? Color.Lerp(_fromC, _midC, eased * 2f)
                    : Color.Lerp(_midC, _toC, (eased - 0.5f) * 2f);
            return Color.Lerp(_fromC, _toC, eased);
        }

        Vector4 SampleVector(float eased)
        {
            if(_useMid)
                return eased <= 0.5f
                    ? Vector4.Lerp(_fromV, _midV, eased * 2f)
                    : Vector4.Lerp(_midV, _toV, (eased - 0.5f) * 2f);
            return Vector4.Lerp(_fromV, _toV, eased);
        }

        float EvalTime(float rawT)
        {
            float t = rawT;
            if(_loop && _pingPong) { t *= 2f; if(t > 1f) t = 2f - t; }
            return Ease(t);
        }

        void BuildCurves(out AnimationCurve[] curves, out string[] paths)
        {
            int frames = Mathf.Max(2, Mathf.RoundToInt(_duration * _fps));
            float dur = (_loop && _pingPong) ? _duration * 2f : _duration;
            int total = (_loop && _pingPong) ? frames * 2 : frames;

            switch(_mode)
            {
                case Mode.Float:
                    paths = new[] { _path };
                    curves = new[] { new AnimationCurve() };
                    for(int i = 0; i <= total; i++)
                    {
                        float rt = (float)i / total;
                        curves[0].AddKey(new Keyframe(rt * dur, SampleFloat(EvalTime(rt))));
                    }
                    return;

                case Mode.Color:
                    paths = new[] { _path + ".r", _path + ".g", _path + ".b", _path + ".a" };
                    curves = new AnimationCurve[4];
                    for(int c = 0; c < 4; c++) curves[c] = new AnimationCurve();
                    for(int i = 0; i <= total; i++)
                    {
                        float rt = (float)i / total;
                        Color v = SampleColor(EvalTime(rt));
                        curves[0].AddKey(new Keyframe(rt * dur, v.r));
                        curves[1].AddKey(new Keyframe(rt * dur, v.g));
                        curves[2].AddKey(new Keyframe(rt * dur, v.b));
                        curves[3].AddKey(new Keyframe(rt * dur, v.a));
                    }
                    return;

                case Mode.Vector:
                    paths = new[] { _path + ".x", _path + ".y", _path + ".z", _path + ".w" };
                    curves = new AnimationCurve[4];
                    for(int c = 0; c < 4; c++) curves[c] = new AnimationCurve();
                    for(int i = 0; i <= total; i++)
                    {
                        float rt = (float)i / total;
                        Vector4 v = SampleVector(EvalTime(rt));
                        curves[0].AddKey(new Keyframe(rt * dur, v.x));
                        curves[1].AddKey(new Keyframe(rt * dur, v.y));
                        curves[2].AddKey(new Keyframe(rt * dur, v.z));
                        curves[3].AddKey(new Keyframe(rt * dur, v.w));
                    }
                    return;

                default:
                    paths = System.Array.Empty<string>();
                    curves = System.Array.Empty<AnimationCurve>();
                    return;
            }
        }

        void WriteCurves(AnimationClip clip, AnimationCurve[] curves, string[] paths)
        {
            var type = typeof(AnimatedSpriteShader);
            // Resolve the binding's hierarchy path. An empty string means "bound to the GameObject
            // that owns the Animator". When the Animator is on a parent (common pattern), an empty
            // path misses the AnimatedSpriteShader component entirely and the clip silently does
            // nothing at runtime. Compute the transform path from the Animator root down to our
            // target so the binding finds the right component regardless of hierarchy depth.
            string hierarchyPath = ResolveHierarchyPath();
            for (int i = 0; i < curves.Length; i++)
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(hierarchyPath, type, paths[i]), curves[i]);
        }

        /// <summary>
        /// Returns the transform path from the Animator's root GameObject down to the target sprite.
        /// Empty string if the Animator is on the same GameObject (Unity's "self" binding).
        /// </summary>
        string ResolveHierarchyPath()
        {
            if (_target == null) return "";
            var animator = _target.GetComponentInParent<Animator>();
            if (animator == null || animator.transform == _target.transform) return "";

            // Walk from target up to the animator root, collecting names in reverse.
            var names = new System.Collections.Generic.List<string>();
            var t = _target.transform;
            var root = animator.transform;
            while (t != null && t != root)
            {
                names.Add(t.name);
                t = t.parent;
            }
            if (t != root) return ""; // target isn't actually under the Animator — self-bind and hope.
            names.Reverse();
            return string.Join("/", names);
        }

        void SaveNewClip()
        {
            string name = _label.Replace(" ", "");
            string file = EditorUtility.SaveFilePanelInProject("Save Clip", name, "anim", "");
            if(string.IsNullOrEmpty(file)) return;

            var clip = new AnimationClip { name = name, frameRate = _fps };
            var s = AnimationUtility.GetAnimationClipSettings(clip);
            s.loopTime = _loop;
            AnimationUtility.SetAnimationClipSettings(clip, s);

            BuildCurves(out var curves, out var paths);
            WriteCurves(clip, curves, paths);

            AssetDatabase.CreateAsset(clip, file);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(file));
            Close();
        }

        void InjectIntoClip(AnimationClip clip)
        {
            if(!clip) return;
            Undo.RecordObject(clip, "Add curves");
            BuildCurves(out var curves, out var paths);
            WriteCurves(clip, curves, paths);
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();
            Close();
        }

        /// <summary>
        /// Attempts to read the currently-selected clip from the Animation window via reflection.
        /// This accesses Unity internals (UnityEditor.AnimationWindow.state.activeAnimationClip)
        /// which have changed across Unity versions. Wrapped in a try/catch so that on Unity 6 (or
        /// any future version where the property is renamed/removed) the keyframe editor still
        /// works — users just fall back to the "Save as New Clip" button or the Animator-picker.
        /// </summary>
        AnimationClip TryGetActiveClip()
        {
            try
            {
                var wt = System.Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
                if (wt == null) return null;
                var wins = Resources.FindObjectsOfTypeAll(wt);
                if (wins.Length == 0) return null;

                var sp = wt.GetProperty("state", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (sp == null) return null;
                var state = sp.GetValue(wins[0]);
                if (state == null) return null;

                var cp = state.GetType().GetProperty("activeAnimationClip", BindingFlags.Public | BindingFlags.Instance);
                return cp?.GetValue(state) as AnimationClip;
            }
            catch
            {
                // Internal API shifted — return null so UI falls through to the Animator-picker path.
                return null;
            }
        }
    }
}
#endif
