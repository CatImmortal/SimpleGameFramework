using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 加载声音的信息
    /// </summary>
    public class LoadSoundInfo
    {

        /// <summary>
        /// 声音序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 声音组
        /// </summary>
        public SoundGroup SoundGroup { get; private set; }

        /// <summary>
        /// 播放声音参数
        /// </summary>
        public PlaySoundParams PlaySoundParams { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public LoadSoundInfo(int serialId, SoundGroup soundGroup, PlaySoundParams playSoundParams, object userData)
        {
            SerialId = serialId;
            SoundGroup = soundGroup;
            PlaySoundParams = playSoundParams;
            UserData = userData;
        }

    }
}

