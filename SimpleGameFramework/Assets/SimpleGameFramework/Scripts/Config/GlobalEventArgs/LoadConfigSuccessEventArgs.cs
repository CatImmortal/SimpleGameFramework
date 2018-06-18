using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 加载配置成功事件
    /// </summary>
    public class LoadConfigSuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载配置成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载配置成功事件编号
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
        /// 获取加载持续时间
        /// </summary>
        public float Duration { get; private set; }


        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        /// <summary>
        /// 清理加载配置成功事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置成功事件
        /// </summary>
        /// <returns>加载配置成功事件。</returns>
        public LoadConfigSuccessEventArgs Fill(object userData, string configAssetName, float duration)
        {
            LoadConfigInfo loadConfigInfo = (LoadConfigInfo)userData;
            ConfigName = loadConfigInfo.ConfigName;
            ConfigAssetName = configAssetName;
            Duration = duration;
            UserData = loadConfigInfo.UserData;

            return this;
        }
    }
}

