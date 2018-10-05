using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音成功事件
    /// </summary>
    public class PlaySoundSuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 播放声音成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(PlaySoundSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 播放声音成功事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 声音的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 声音资源名称
        /// </summary>
        public string SoundAssetName { get; private set; }

        /// <summary>
        /// 声音绑定的实体
        /// </summary>
        public Entity.Entity BindingEntity { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 用于播放的声音代理
        /// </summary>
        public SoundAgent SoundAgent { get; private set; }

        /// <summary>
        /// 加载持续时间
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 清理播放声音成功事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            SoundAssetName = default(string);
            SoundAgent = default(SoundAgent);
            Duration = default(float);
            BindingEntity = default(Entity.Entity);
            UserData = default(object);
        }

        /// <summary>
        /// 填充播放声音成功事件
        /// </summary>
        /// <returns>播放声音成功事件</returns>
        public PlaySoundSuccessEventArgs Fill(object userData, int serialId, string soundAssetName,SoundAgent soundAgent,float duration)
        {
            PlaySoundInfo playSoundInfo = (PlaySoundInfo)userData;

            SerialId = serialId;
            SoundAssetName = soundAssetName;
            BindingEntity = playSoundInfo.BindingEntity;
            UserData = playSoundInfo.UserData;

            SoundAgent = soundAgent;
            Duration = duration;

            return this;
        }
    }
}

