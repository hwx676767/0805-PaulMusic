using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 单条轨道：作为 Playfield 预制体里的一个子物体被编辑。引用背景与按下闪光两个 SpriteRenderer，
    /// 并保存自己的触发按键。位置/宽度等几何信息直接来自它在预制体里被摆放/缩放的样子，
    /// RhythmGameManager 通过 CenterX / Width 读取，不再反算坐标。
    /// </summary>
    public class Lane : MonoBehaviour
    {
        [Tooltip("这条轨道的触发按键。")]
        public KeyCode key = KeyCode.F;

        [Tooltip("轨道背景（其世界宽度即音符宽度参考）。")]
        public SpriteRenderer background;

        [Tooltip("按下时显示的高亮层（可留空）。")]
        public SpriteRenderer flash;

        [Range(0f, 1f)]
        [Tooltip("按住时闪光层的不透明度。")]
        public float flashAlpha = 0.18f;

        [Tooltip("松开后闪光淡出速度（每秒透明度）。")]
        public float flashFadeSpeed = 1.5f;

        /// <summary>轨道中心的世界 X（即音符落下的横向位置）。</summary>
        public float CenterX => transform.position.x;

        /// <summary>轨道世界宽度（取背景的世界缩放）。供需要时读取，音符尺寸现已改为各自 prefab 决定。</summary>
        public float Width => background != null ? Mathf.Abs(background.transform.lossyScale.x) : 1f;

        private bool _pressed;

        public void SetPressed(bool pressed)
        {
            _pressed = pressed;
        }

        private void Update()
        {
            if (flash == null) return;
            var c = flash.color;
            c.a = _pressed ? flashAlpha : Mathf.MoveTowards(c.a, 0f, Time.deltaTime * flashFadeSpeed);
            flash.color = c;
        }
    }
}
