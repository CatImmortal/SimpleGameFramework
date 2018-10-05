using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 加载配置失败事件
    /// </summary>
    public class LoadConfigFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载配置失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigFailureEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载配置失败事件编号
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
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }


        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        /// <summary>
        /// 清理加载配置失败事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置失败事件
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>加载配置失败事件。</returns>
        public LoadConfigFailureEventArgs Fill(object userData, string configAssetName, string errorMessage)
        {
            LoadConfigInfo loadConfigInfo = (LoadConfigInfo)userData;
            ConfigName = loadConfigInfo.ConfigName;
            ConfigAssetName = configAssetName;
            ErrorMessage = errorMessage;
            UserData = loadConfigInfo.UserData;

            return this;
        }
    }
}

