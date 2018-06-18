using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音更新事件
    /// </summary>
    public class PlaySoundUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 播放声音更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(PlaySoundUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 播放声音更新事件编号
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
        /// 声音组名称
        /// </summary>
        public string SoundGroupName { get; private set; }

        /// <summary>
        /// 播放声音参数
        /// </summary>
        public PlaySoundParams PlaySoundParams { get; private set; }

        /// <summary>
        /// 声音绑定的实体
        /// </summary>
        public Entity.Entity BindingEntity { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 加载声音进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 清理播放声音更新事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            SoundAssetName = default(string);
            SoundGroupName = default(string);
            PlaySoundParams = default(PlaySoundParams);
            Progress = default(float);
            BindingEntity = default(Entity.Entity);
            UserData = default(object);
        }

        /// <summary>
        /// 填充播放声音更新事件
        /// </summary>
        /// <returns>播放声音更新事件。</returns>
        public PlaySoundUpdateEventArgs Fill(object userData, int serialId, string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams,float progress)
        {
            PlaySoundInfo playSoundInfo = (PlaySoundInfo)userData;

            SerialId = serialId;
            SoundAssetName = soundAssetName;
            SoundGroupName = soundGroupName;
            PlaySoundParams = playSoundParams;
            BindingEntity = playSoundInfo.BindingEntity;
            UserData = playSoundInfo.UserData;

            Progress = progress;

            return this;
        }

    }
}

