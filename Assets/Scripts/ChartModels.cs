using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>单个音符。time 为击打时刻（秒，相对歌曲起点）；duration>0 表示长按（Hold），=0 为单点（Tap）。</summary>
    [Serializable]
    public class NoteData
    {
        public int lane;        // 0 开始的轨道下标
        public double time;     // 击打时刻（秒）
        public double duration; // 长按时长（秒），0 表示单点

        public bool IsHold => duration > 0.0001;
        public double EndTime => time + duration;
    }

    /// <summary>一张谱面。可用 JsonUtility 直接序列化/反序列化。</summary>
    [Serializable]
    public class ChartData
    {
        public string title = "Untitled";
        public string artist = "";
        public int laneCount = 4;
        public double bpm = 120;
        public double offset = 0;   // 第一拍相对音频起点的空拍（秒）
        public List<NoteData> notes = new List<NoteData>();

        public static ChartData FromJson(string json)
        {
            var chart = JsonUtility.FromJson<ChartData>(json);
            if (chart == null) return null;
            if (chart.notes == null) chart.notes = new List<NoteData>();
            // 保证按时间升序，判定与扫描逻辑依赖这个顺序
            chart.notes.Sort((a, b) => a.time.CompareTo(b.time));
            return chart;
        }

        public string ToJson() => JsonUtility.ToJson(this, true);
    }
}
