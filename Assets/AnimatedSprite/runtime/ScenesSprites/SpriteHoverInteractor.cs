using UnityEngine;
using UnityEngine.EventSystems;


namespace FlexiSprite
{
    [RequireComponent(typeof(AnimatedSpriteShader))]
    public class SpriteHoverInteractor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum HoverStyle
        {
            Button1_ScaleBounce,
            Button2_ScaleRotate,
            Button3_GlowExpand,
            Button4_OutlineSpeedUp,
            Card_ScaleTilt,
            Map_Brightness,
            Sword1_DimFrame,
            Sword2_HexDissolveFrame
        }

        [Header("Hover Configuration")]
        public HoverStyle style;

        [Tooltip("Leave empty to animate this object. Assign a child/external object to animate that transform instead.")]
        public Transform targetToAnimate;

        private AnimatedSpriteShader _shader;
        private bool _isHovered;

        private Vector3 _baseScale;
        private Vector3 _targetScale;
        private Vector3 _scaleVel;

        private float _baseRot;
        private float _targetRot;
        private float _currentRot;
        private float _rotVel;

        private float _shaderT;
        private float _shaderDuration = 0.3f;
        private AnimEasing _shaderEasing = AnimEasing.EaseOut;

        private void Start()
        {
            _shader = GetComponent<AnimatedSpriteShader>();

            if (targetToAnimate == null)
                targetToAnimate = transform;

            _baseScale = targetToAnimate.localScale;
            _targetScale = _baseScale;

            _baseRot = targetToAnimate.localEulerAngles.z;
            _currentRot = _baseRot;
            _targetRot = _baseRot;

            SetupShaderInitialState();
        }

        private void SetupShaderInitialState()
        {
            switch (style)
            {
                case HoverStyle.Button3_GlowExpand:
                    _shader.glowEnabled = true;
                    _shaderEasing = AnimEasing.Elastic;
                    _shaderDuration = 0.4f;
                    _shader.MarkEffectDirty(EffectId.Glow);
                    break;

                case HoverStyle.Button4_OutlineSpeedUp:
                    _shaderEasing = AnimEasing.EaseOut;
                    _shaderDuration = 0.3f;

                    break;

                case HoverStyle.Map_Brightness:
                case HoverStyle.Sword1_DimFrame:
                    _shader.brightnessEnabled = true;
                    _shaderEasing = AnimEasing.EaseOut;
                    _shaderDuration = 0.3f;
                    _shader.MarkEffectDirty(EffectId.Brightness);
                    break;

                case HoverStyle.Sword2_HexDissolveFrame:
                    _shader.hexDissolveEnabled = true;
                    _shaderEasing = AnimEasing.EaseIn;
                    _shaderDuration = 0.2f;
                    _shader.MarkEffectDirty(EffectId.HexDissolve);
                    break;
            }

          
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateTargetTransforms();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateTargetTransforms();
        }

        private void UpdateTargetTransforms()
        {
            _shader.MarkGlobalsDirty();
            switch (style)
            {

                case HoverStyle.Button1_ScaleBounce:
                case HoverStyle.Button3_GlowExpand:
                case HoverStyle.Button4_OutlineSpeedUp:
                case HoverStyle.Map_Brightness:
                   
                    _targetScale = _isHovered ? _baseScale * 1.2f : _baseScale;
                    break;

                case HoverStyle.Button2_ScaleRotate:
              
                    _targetScale = _isHovered ? _baseScale * 1.2f : _baseScale;
                    _targetRot = _isHovered ? _baseRot - 15f : _baseRot;
                    break;

                case HoverStyle.Card_ScaleTilt:
                    _targetScale = _isHovered ? _baseScale * 1.15f : _baseScale;
                    _targetRot = _isHovered ? _baseRot - 8f : _baseRot;
                    break;

                case HoverStyle.Sword1_DimFrame:
                case HoverStyle.Sword2_HexDissolveFrame:

                    _targetScale = _isHovered ? _baseScale * 0.9f : _baseScale;
                    break;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            ApplyTransformSpring(dt);

            if (!AdvanceShaderTime(dt))
                return;

            ApplyShaderHoverState();
           
        }

        private void ApplyTransformSpring(float dt)
        {
            const float tension = 250f;
            float dampening = _isHovered ? 10f : 14f;

            _scaleVel += (_targetScale - targetToAnimate.localScale) * tension * dt;
            _scaleVel -= _scaleVel * dampening * dt;
            targetToAnimate.localScale += _scaleVel * dt;

            _rotVel += (_targetRot - _currentRot) * tension * dt;
            _rotVel -= _rotVel * dampening * dt;
            _currentRot += _rotVel * dt;

            targetToAnimate.localRotation = Quaternion.Euler(0f, 0f, _currentRot);
        }

        private bool AdvanceShaderTime(float dt)
        {
            if (_isHovered && _shaderT < 1f)
            {
                _shaderT = Mathf.Clamp01(_shaderT + dt / _shaderDuration);
                return true;
            }

            if (!_isHovered && _shaderT > 0f)
            {
                _shaderT = Mathf.Clamp01(_shaderT - dt / _shaderDuration);
                return true;
            }

            return false;
        }

        private void ApplyShaderHoverState()
        {
            float easedT = AnimSpriteEasing.Apply(_shaderT, _shaderEasing);

            switch (style)
            {
                case HoverStyle.Button3_GlowExpand:
                    _shader.MarkEffectDirty(EffectId.Glow);
                    _shader.glowIntensity = Mathf.LerpUnclamped(0f, 10f, easedT);
                    break;

                case HoverStyle.Button4_OutlineSpeedUp:
                    _shader.MarkEffectDirty(EffectId.SelectionHighlight);
                    _shader.selectHighlightSize = Mathf.LerpUnclamped(0.04f, 0.2f, easedT);
                    _shader.animOutlineSpeed = Mathf.LerpUnclamped(2f, 10f, easedT);
                    break;

                case HoverStyle.Map_Brightness:
                    _shader.MarkEffectDirty(EffectId.Brightness);
                    _shader.brightnessAmount = Mathf.LerpUnclamped(0f, 0.3f, easedT);
                    break;

                case HoverStyle.Sword1_DimFrame:
                    _shader.MarkEffectDirty(EffectId.Brightness);
                    _shader.brightnessAmount = Mathf.LerpUnclamped(0f, -0.4f, easedT);
                    break;

                case HoverStyle.Sword2_HexDissolveFrame:
                    _shader.MarkEffectDirty(EffectId.HexDissolve);
                    _shader.hexDissolveAmount = Mathf.LerpUnclamped(0f, 1f, easedT);
                    break;
            }
        }
    }
}
