using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace FlexiSprite
{
    [Serializable]
    public class EffectBobbingPair
    {
        public EffectId effect;
        public bool enableBobbing;
    }

    public sealed class EffectShowcaseCard : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private TMP_Text effectNameText;
        [SerializeField] private float frameHeight = 3.0f;
        [SerializeField] private float titlePadding = 0.25f;

        [Header("Effect Sprite Mirror (on sprite child)")]
        [SerializeField] private EffectShowcaseEffectMirror mirror;

        [Header("Sprite child transform (for custom sprite scaling)")]
        [SerializeField] private Transform spriteChild;

        [Header("Scaling Settings")]
        [SerializeField] private float spritePadding = 0.1f;
        [SerializeField] private bool allowUpscaling = true;

        [Header("Bobbing Animation")]
        [SerializeField] private float bobDistance = 1.0f;
        [SerializeField] private float bobMoveDuration = 1.0f;
        [SerializeField] private float bobWaitDuration = 0.5f;

        [Tooltip("Enable bobbing only for specific effects in this list")]
        [SerializeField] private List<EffectBobbingPair> perEffectBobbing = new List<EffectBobbingPair>();

        private float _alpha = 1f;
        private Vector3 _defaultSpriteScale = Vector3.one;

        private bool _isBobbingEnabledForCurrentEffect;
        private float _bobTimer;
        private Vector3 _spriteChildBasePos;
        private Vector3 _textBasePos;

        private void Awake()
        {
            if (spriteChild)
            {
                _defaultSpriteScale = spriteChild.localScale;
                _spriteChildBasePos = spriteChild.localPosition;
            }

            if (effectNameText)
                _textBasePos = effectNameText.transform.localPosition;
        }

        private void Update()
        {
            float currentOffset = GetBobbingOffset();

            if (spriteChild)
                spriteChild.localPosition = _spriteChildBasePos + new Vector3(0f, currentOffset, 0f);

            if (effectNameText)
                effectNameText.transform.localPosition = _textBasePos + new Vector3(0f, currentOffset, 0f);
        }

        private float GetBobbingOffset()
        {
            if (!_isBobbingEnabledForCurrentEffect || bobMoveDuration <= 0f)
            {
                _bobTimer = 0f;
                return 0f;
            }

            _bobTimer += Time.deltaTime;

            float fullCycleTime = (bobMoveDuration * 2f) + (bobWaitDuration * 2f);
            _bobTimer %= fullCycleTime;

            if (_bobTimer < bobMoveDuration)
            {
                float t = _bobTimer / bobMoveDuration;
                return Mathf.SmoothStep(0f, bobDistance, t);
            }

            if (_bobTimer < bobMoveDuration + bobWaitDuration)
                return bobDistance;

            if (_bobTimer < (bobMoveDuration * 2f) + bobWaitDuration)
            {
                float t = (_bobTimer - bobMoveDuration - bobWaitDuration) / bobMoveDuration;
                return Mathf.SmoothStep(bobDistance, 0f, t);
            }

            return 0f;
        }

        public void SetSourceDummy(AnimatedSpriteShader dummy)
        {
            if (mirror)
                mirror.SetSource(dummy);
        }

        public void BindEffect(EffectDef def, Sprite customSprite = null)
        {
            if (def == null) return;

            UpdateBobbingState(def.id);
            _bobTimer = 0f;

            if (effectNameText)
                BindLabel(def);

            if (!mirror) return;

            mirror.SetEffect(def.id);

            if (customSprite != null)
            {
                mirror.SetCustomSprite(customSprite);
                ApplyCustomSpriteScale(customSprite);
            }
            else
            {
                mirror.ClearCustomSprite();
                ResetSpriteScale();
            }
        }

        private void UpdateBobbingState(EffectId effectId)
        {
            _isBobbingEnabledForCurrentEffect = false;

            if (perEffectBobbing == null) return;

            foreach (var pair in perEffectBobbing)
            {
                if (pair.effect != effectId) continue;

                _isBobbingEnabledForCurrentEffect = pair.enableBobbing;
                break;
            }
        }

        private void BindLabel(EffectDef def)
        {
            effectNameText.text = def.name;

            var t = effectNameText.transform;
            var p = t.localPosition;
            p.y = (frameHeight * 0.5f) + titlePadding;

            _textBasePos = p + new Vector3(0f, -2f, 0f);
            t.localPosition = _textBasePos;
        }

        private void ApplyCustomSpriteScale(Sprite sprite)
        {
            if (!spriteChild || sprite == null) return;

            float ppu = sprite.pixelsPerUnit;
            float spriteW = sprite.rect.width / ppu;
            float spriteH = sprite.rect.height / ppu;
            float targetSize = frameHeight * (1f - spritePadding);
            float scaleX = targetSize / spriteW;
            float scaleY = targetSize / spriteH;
            float bestScale = Mathf.Min(scaleX, scaleY);

            if (!allowUpscaling)
                bestScale = Mathf.Min(bestScale, 1f);

            spriteChild.localScale = _defaultSpriteScale * bestScale;

            if (spriteChild.localScale.sqrMagnitude < 0.0001f)
                spriteChild.localScale = _defaultSpriteScale;
        }

        private void ResetSpriteScale()
        {
            if (spriteChild)
                spriteChild.localScale = _defaultSpriteScale;
        }

        public void ApplyPose(Vector3 localPos, Quaternion localRot, float scale, float alpha)
        {
            transform.localPosition = localPos;
            transform.localRotation = localRot;
            transform.localScale = Vector3.one * scale;

            effectNameText.transform.localScale = Vector3.one * Mathf.Clamp01(Mathf.Pow(2.5f, scale) - 1);
            SetAlpha(alpha);
        }

        public void SetAlpha(float a)
        {
            _alpha = Mathf.Clamp01(a);

            if (effectNameText)
            {
                var c = effectNameText.color;
                c.a = _alpha;
                effectNameText.color = c;
            }

            var srs = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                var c = sr.color;
                c.a = _alpha;
                sr.color = c;
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void HideLabel()
        {
            if (effectNameText)
                effectNameText.gameObject.SetActive(false);
        }
    }
}
