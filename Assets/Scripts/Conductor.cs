using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 节拍/计时核心：以 AudioSettings.dspTime 为唯一时间基准来追踪歌曲播放进度，
    /// 而不是用帧数或 Time.deltaTime 累加（那样会随掉帧产生累积漂移）。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Conductor : MonoBehaviour
    {
        /// <summary>当前歌曲时间（秒），已扣除谱面起始偏移与音频校准偏移。负值表示倒计时阶段（音乐尚未真正出声）。</summary>
        public double SongPosition { get; private set; }

        public bool IsPlaying { get; private set; }
        public AudioClip Clip => _source != null ? _source.clip : null;

        // 声卡/缓冲带来的播放延迟补偿。玩家在校准界面里调，正值让判定相对"听到的声音"提前。
        public double AudioOffset { get; set; }

        // 谱面第一拍相对音频 0 号采样的空拍时间。
        public double ChartStartOffset { get; set; }

        /// <summary>
        /// 播放速度倍率（1=原速，0.8=减速 20%，1.2=加速 20%）。会同时改变音频音高（pitch）与歌曲时间的推进速率，
        /// 从而让音符下落节奏一起变快/变慢。可在播放中随时改变（供卡牌效果调用）。
        /// </summary>
        public double PlaybackSpeed
        {
            get => _speed;
            set
            {
                _speed = System.Math.Max(0.05, value);
                if (_source != null) _source.pitch = (float)_speed;
            }
        }

        private AudioSource _source;
        private double _dspStart;     // 音频 0 号采样真正发声对应的 dspTime
        private double _lastDsp;      // 上次读到的 dspTime
        private double _rawSong;      // 已累计的"歌曲内容秒数"（未扣偏移），按 _speed 推进
        private double _smoothed;     // 在两次 dsp 更新之间用帧时间插值，避免音符位置"跳格"
        private double _speed = 1.0;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.loop = false;
        }

        /// <summary>
        /// 预约播放：先把开始时刻定在未来一小段（scheduleAhead），用 PlayScheduled 保证采样级精确启动。
        /// </summary>
        public void Play(AudioClip clip, double chartStartOffset, double scheduleAhead = 0.3)
        {
            _source.clip = clip;
            ChartStartOffset = chartStartOffset;

            double now = AudioSettings.dspTime;
            _dspStart = now + scheduleAhead;
            _source.PlayScheduled(_dspStart);

            _speed = 1.0;                 // 每局重置速度
            _source.pitch = 1f;
            _rawSong = -scheduleAhead;    // 倒计时阶段（音乐尚未出声）
            _lastDsp = now;               // 从现在开始累计
            _smoothed = _rawSong;
            SongPosition = _smoothed - ChartStartOffset - AudioOffset;
            IsPlaying = true;
        }

        public void Stop()
        {
            if (_source != null) _source.Stop();
            IsPlaying = false;
        }

        private void Update()
        {
            if (!IsPlaying) return;

            double dsp = AudioSettings.dspTime;

            // dspTime 是按音频缓冲块整块更新的（几毫秒到几十毫秒才跳一次），
            // 直接用会让音符一卡一卡地走。这里在两次更新之间用未缩放帧时间平滑推进，
            // dsp 真正更新时再吸附回真实累计值。歌曲时间按 _speed 推进（与音频 pitch 一致），
            // 从而支持变速；速度=1 时行为与原来完全一致。
            if (dsp != _lastDsp)
            {
                _rawSong += (dsp - _lastDsp) * _speed;
                _lastDsp = dsp;
                _smoothed = _rawSong;
            }
            else
            {
                _smoothed += Time.unscaledDeltaTime * _speed;
            }

            SongPosition = _smoothed - ChartStartOffset - AudioOffset;

            if (_source.clip != null && SongPosition > _source.clip.length + 1.0)
                IsPlaying = false;
        }
    }
}
