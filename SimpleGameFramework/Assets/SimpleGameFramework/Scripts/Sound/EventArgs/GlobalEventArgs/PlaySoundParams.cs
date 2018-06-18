using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音参数
    /// </summary>
    public class PlaySoundParams
    {

        /// <summary>
        /// 播放位置
        /// </summary>
        public float Time { get; set; }


        /// <summary>
        /// 在声音组内是否静音
        /// </summary>
        public bool MuteInSoundGroup { get; set; }


        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Loop { get; set; }


        /// <summary>
        /// 声音优先级
        /// </summary>
        public int Priority { get; set; }


        /// <summary>
        /// 在声音组内音量大小
        /// </summary>
        public float VolumeInSoundGroup { get; set; }


        /// <summary>
        /// 声音淡入时间，以秒为单位
        /// </summary>
        public float FadeInSeconds { get; set; }


        /// <summary>
        /// 声音音调
        /// </summary>
        public float Pitch { get; set; }


        /// <summary>
        /// 声音立体声声相
        /// </summary>
        public float PanStereo { get; set; }


        /// <summary>
        /// 声音空间混合量
        /// </summary>
        public float SpatialBlend { get; set; }


        /// <summary>
        /// 声音最大距离
        /// </summary>
        public float MaxDistance { get; set; }

        public PlaySoundParams()
        {
            Time = Constant.DefaultTime;
            MuteInSoundGroup = Constant.DefaultMute;
            Loop = Constant.DefaultLoop;
            Priority = Constant.DefaultPriority;
            VolumeInSoundGroup = Constant.DefaultVolume;
            FadeInSeconds = Constant.DefaultFadeInSeconds;
            Pitch = Constant.DefaultPitch;
            PanStereo = Constant.DefaultPanStereo;
            SpatialBlend = Constant.DefaultSpatialBlend;
            MaxDistance = Constant.DefaultMaxDistance;
        }

    }
}

