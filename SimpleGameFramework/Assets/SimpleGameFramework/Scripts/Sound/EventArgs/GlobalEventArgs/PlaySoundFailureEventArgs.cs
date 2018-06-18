using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音失败事件
    /// </summary>
    public class PlaySoundFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 播放声音失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(PlaySoundFailureEventArgs).GetHashCode();

        /// <summary>
        /// 播放声音失败事件编号
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
        /// 错误码
        /// </summary>
        public PlaySoundErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理播放声音失败事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            SoundAssetName = default(string);
            SoundGroupName = default(string);
            PlaySoundParams = default(PlaySoundParams);
            BindingEntity = default(Entity.Entity);
            ErrorCode = default(int);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充播放声音失败事件
        /// </summary>
        /// <returns>播放声音失败事件</returns>
        public PlaySoundFailureEventArgs Fill(object userData, int serialId, string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams,PlaySoundErrorCode errorCode,string errorMessage)
        {
            PlaySoundInfo playSoundInfo = (PlaySoundInfo)userData;

            SerialId = serialId;
            SoundAssetName = soundAssetName;
            SoundGroupName = soundGroupName;
            PlaySoundParams = playSoundParams;
            BindingEntity = playSoundInfo.BindingEntity;
            UserData = playSoundInfo.UserData;

            ErrorCode = errorCode;
            ErrorMessage = errorMessage;

            return this;
        }
    }
}

