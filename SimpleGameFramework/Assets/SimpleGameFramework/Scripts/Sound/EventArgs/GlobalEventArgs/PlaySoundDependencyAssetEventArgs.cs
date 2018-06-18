using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音时加载依赖资源事件
    /// </summary>
    public class PlaySoundDependencyAssetEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 播放声音时加载依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(PlaySoundDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        /// 获取播放声音时加载依赖资源事件编号
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
        /// 被加载的依赖资源名称
        /// </summary>
        public string DependencyAssetName { get; private set; }


        /// <summary>
        /// 当前已加载依赖资源数量
        /// </summary>
        public int LoadedCount { get; private set; }


        /// <summary>
        /// 总共加载依赖资源数量
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 清理播放声音时加载依赖资源事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            SoundAssetName = default(string);
            SoundGroupName = default(string);
            PlaySoundParams = default(PlaySoundParams);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            BindingEntity = default(Entity.Entity);
            UserData = default(object);
        }

        /// <summary>
        /// 填充播放声音时加载依赖资源事件
        /// </summary>
        /// <returns>播放声音时加载依赖资源事件</returns>
        public PlaySoundDependencyAssetEventArgs Fill(object userData,int serialId,string soundAssetName, string soundGroupName,PlaySoundParams playSoundParams,string dependencyAssetName,int loadedCount,int totalCount)
        {
            PlaySoundInfo playSoundInfo = (PlaySoundInfo)userData;

            SerialId = serialId;
            SoundAssetName = soundAssetName;
            SoundGroupName = soundGroupName;
            PlaySoundParams = playSoundParams;
            BindingEntity = playSoundInfo.BindingEntity;
            UserData = playSoundInfo.UserData;

            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;

            return this;
        }

    }
}

