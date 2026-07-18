using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace FlexiSprite
{
    public enum AnimWrapMode
    {
        Once,
        Loop,
        PingPong,
        ClampForever
    }

    [RequireComponent(typeof(AnimatedSpriteShader))]
    public class ShaderPropertyAnimator : MonoBehaviour
    {
        [Serializable]
        public class PropertyAnimation
        {
            public string fieldName;
            public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public float duration = 1f;
            public float delay;
            public AnimWrapMode wrapMode = AnimWrapMode.Once;
            public AnimEasing easing = AnimEasing.Linear;

            public float fromFloat;
            public float toFloat = 1f;
            public Color fromColor = Color.white;
            public Color toColor = Color.white;
            public Vector4 fromVector;
            public Vector4 toVector;

            public bool autoPlay = true;

            [NonSerialized] public float elapsed;
            [NonSerialized] public bool playing;
            [NonSerialized] public bool forward = true;
            [NonSerialized] public FieldInfo cachedField;
            [NonSerialized] public Type cachedType;
            [NonSerialized] public int cachedSlot = -1;
        }

        public List<PropertyAnimation> animations = new();

        private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

        private AnimatedSpriteShader _mgr;

        private void Awake()
        {
            _mgr = GetComponent<AnimatedSpriteShader>();
            ResolveAnimationFields();
        }

        private void OnEnable()
        {
            if (!_mgr)
                _mgr = GetComponent<AnimatedSpriteShader>();

            ResolveAnimationFields();

            foreach (var anim in animations)
            {
                if (anim.autoPlay)
                    Play(anim);
            }
        }

        private void Update()
        {
            if (!_mgr) return;

            foreach (var anim in animations)
            {
                if (!anim.playing || anim.cachedField == null) continue;

                anim.elapsed += Time.deltaTime;

                if (anim.elapsed < anim.delay) continue;

                float activeTime = anim.elapsed - anim.delay;
                float rawT = anim.duration > 0 ? activeTime / anim.duration : 1f;
                float t = EvaluateWrap(rawT, anim.wrapMode, ref anim.forward, ref anim.playing);
                float curveT = anim.curve.Evaluate(t);
                float easedT = AnimSpriteEasing.Apply(curveT, anim.easing);

                ApplyValue(anim, easedT);
                _mgr.MarkDirty();
            }
        }

        private void ResolveAnimationFields()
        {
            foreach (var anim in animations)
                ResolveAnimationField(anim);
        }

        private static void ResolveAnimationField(PropertyAnimation anim)
        {
            if (anim == null || anim.cachedField != null || string.IsNullOrEmpty(anim.fieldName))
                return;

            var fi = typeof(AnimatedSpriteShader).GetField(anim.fieldName, Flags);
            if (fi == null)
                return;

            anim.cachedField = fi;
            anim.cachedType = fi.FieldType;
        }

        private float EvaluateWrap(float rawT, AnimWrapMode mode, ref bool forward, ref bool playing)
        {
            switch (mode)
            {
                case AnimWrapMode.Once:
                    if (rawT >= 1f)
                    {
                        playing = false;
                        return 1f;
                    }

                    return rawT;

                case AnimWrapMode.Loop:
                    return rawT % 1f;

                case AnimWrapMode.PingPong:
                    float cycle = rawT % 2f;
                    return cycle <= 1f ? cycle : 2f - cycle;

                case AnimWrapMode.ClampForever:
                    return Mathf.Clamp01(rawT);

                default:
                    return Mathf.Clamp01(rawT);
            }
        }

        private void ApplyValue(PropertyAnimation anim, float t)
        {
            if (anim.cachedType == typeof(float))
            {
                anim.cachedField.SetValue(_mgr, Mathf.LerpUnclamped(anim.fromFloat, anim.toFloat, t));
            }
            else if (anim.cachedType == typeof(int))
            {
                anim.cachedField.SetValue(_mgr, Mathf.RoundToInt(Mathf.LerpUnclamped(anim.fromFloat, anim.toFloat, t)));
            }
            else if (anim.cachedType == typeof(Color))
            {
                anim.cachedField.SetValue(_mgr, Color.LerpUnclamped(anim.fromColor, anim.toColor, t));
            }
            else if (anim.cachedType == typeof(Vector4))
            {
                anim.cachedField.SetValue(_mgr, Vector4.LerpUnclamped(anim.fromVector, anim.toVector, t));
            }
            else if (anim.cachedType == typeof(Vector2))
            {
                anim.cachedField.SetValue(_mgr, Vector2.LerpUnclamped(anim.fromVector, anim.toVector, t));
            }
            else if (anim.cachedType == typeof(bool))
            {
                anim.cachedField.SetValue(_mgr, t > 0.5f);
            }
        }

        public void Play(string fieldName)
        {
            var anim = animations.Find(a => a.fieldName == fieldName);
            if (anim != null)
                Play(anim);
        }

        public void Play(PropertyAnimation anim)
        {
            if (anim == null) return;

            ResolveAnimationField(anim);

            anim.elapsed = 0;
            anim.playing = true;
            anim.forward = true;
        }

        public void PlayAll()
        {
            foreach (var anim in animations)
                Play(anim);
        }

        public void Stop(string fieldName)
        {
            var anim = animations.Find(a => a.fieldName == fieldName);
            if (anim != null)
                anim.playing = false;
        }

        public void StopAll()
        {
            foreach (var anim in animations)
                anim.playing = false;
        }

        public void SetNormalizedTime(string fieldName, float t)
        {
            var anim = animations.Find(a => a.fieldName == fieldName);
            if (anim == null) return;

            ResolveAnimationField(anim);
            if (anim.cachedField == null) return;

            float curveT = anim.curve.Evaluate(Mathf.Clamp01(t));
            float easedT = AnimSpriteEasing.Apply(curveT, anim.easing);

            ApplyValue(anim, easedT);
            _mgr.MarkDirty();
        }

        public PropertyAnimation AddAnimation(
            string fieldName,
            float from,
            float to,
            float duration,
            AnimWrapMode wrapMode = AnimWrapMode.Once,
            AnimEasing easing = AnimEasing.Linear)
        {
            var anim = new PropertyAnimation
            {
                fieldName = fieldName,
                fromFloat = from,
                toFloat = to,
                duration = duration,
                wrapMode = wrapMode,
                easing = easing,
                autoPlay = false
            };

            ResolveAnimationField(anim);

            animations.Add(anim);
            return anim;
        }

        public PropertyAnimation AddColorAnimation(
            string fieldName,
            Color from,
            Color to,
            float duration,
            AnimWrapMode wrapMode = AnimWrapMode.Once,
            AnimEasing easing = AnimEasing.Linear)
        {
            var anim = new PropertyAnimation
            {
                fieldName = fieldName,
                fromColor = from,
                toColor = to,
                duration = duration,
                wrapMode = wrapMode,
                easing = easing,
                autoPlay = false
            };

            ResolveAnimationField(anim);

            animations.Add(anim);
            return anim;
        }
    }
}
