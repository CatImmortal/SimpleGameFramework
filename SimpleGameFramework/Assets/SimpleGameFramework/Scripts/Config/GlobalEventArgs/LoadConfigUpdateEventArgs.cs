using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 加载配置更新事件
    /// </summary>
    public class LoadConfigUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载配置更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载配置更新事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取配置名称
        /// </summary>
        public string ConfigName { get; private set; }


        /// <summary>
        /// 获取配置资源名称
        /// </summary>
        public string ConfigAssetName { get; private set; }


        /// <summary>
        /// 获取加载配置进度
        /// </summary>
        public float Progress { get; private set; }


        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        /// <summary>
        /// 清理加载配置更新事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置更新事件
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>加载配置更新事件。</returns>
        public LoadConfigUpdateEventArgs Fill(object userData, string configAssetName, float progress)
        {
            LoadConfigInfo loadConfigInfo = (LoadConfigInfo)userData;
            ConfigName = loadConfigInfo.ConfigName;
            ConfigAssetName = configAssetName;
            Progress = progress;
            UserData = loadConfigInfo.UserData;

            return this;
        }
    }
}

