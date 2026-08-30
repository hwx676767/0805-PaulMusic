using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// UI 空闲动效：绕 Z 轴缓慢旋转，同时做轻微呼吸缩放。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UiIdleMotion : MonoBehaviour
    {
        [Header("旋转")]
        [Tooltip("每秒绕局部 Z 轴旋转的角度（正=逆时针）。")]
        public float rotateDegreesPerSecond = 18f;
        [Tooltip("是否反向旋转。")]
        public bool reverseRotation;

        [Header("缩放")]
        [Tooltip("基准缩放（启动时从当前 localScale 采样；也可手动覆盖）。")]
        public Vector3 baseScale = Vector3.one;
        [Tooltip("勾选后在 Awake 用当前 localScale 作为基准。")]
        public bool captureBaseScaleOnAwake = true;
        [Tooltip("相对基准的额外缩放幅度（0.05 = ±5%）。")]
        [Range(0f, 0.5f)] public float scaleAmplitude = 0.06f;
        [Tooltip("呼吸缩放的频率（Hz）。")]
        [Min(0.01f)] public float scaleFrequency = 0.7f;

        [Header("时间")]
        [Tooltip("用 unscaledDeltaTime，暂停时仍可动。")]
        public bool useUnscaledTime = true;

        private float _phase;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = transform as RectTransform;
            if (captureBaseScaleOnAwake)
                baseScale = transform.localScale;
            if (baseScale.sqrMagnitude < 1e-8f)
                baseScale = Vector3.one;
        }

        private void Update()
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float sign = reverseRotation ? -1f : 1f;

            transform.Rotate(0f, 0f, rotateDegreesPerSecond * sign * dt, Space.Self);

            _phase += dt * scaleFrequency * Mathf.PI * 2f;
            float pulse = 1f + Mathf.Sin(_phase) * scaleAmplitude;
            Vector3 scale = baseScale * pulse;
            if (_rect != null)
                _rect.localScale = scale;
            else
                transform.localScale = scale;
        }
    }
}
