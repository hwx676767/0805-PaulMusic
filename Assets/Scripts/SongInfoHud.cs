using TMPro;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 参考图右上角代码风信息框：显示曲名与当前攻击倍率等运行时数值。
    /// </summary>
    public class SongInfoHud : MonoBehaviour
    {
        public TMP_Text label;
        public RhythmGameManager manager;
        public CombatSystem combat;
        public string songFallbackName = "Demo";

        private void OnEnable()
        {
            if (manager == null) manager = FindFirstObjectByType<RhythmGameManager>();
            if (combat == null) combat = FindFirstObjectByType<CombatSystem>();
            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (label == null) return;
            string song = songFallbackName;
            if (manager != null && manager.song != null && !string.IsNullOrEmpty(manager.song.name))
                song = manager.song.name;

            int attack = 200;
            if (combat != null)
                attack = Mathf.RoundToInt(200f * combat.DamageDealtMultiplier);

            label.text = $"String Song_name = \"{song}\";\nint attack = {attack};";
        }
    }
}
