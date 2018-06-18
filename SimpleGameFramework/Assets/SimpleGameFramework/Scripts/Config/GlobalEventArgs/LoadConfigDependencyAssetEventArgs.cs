using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 加载配置时加载依赖资源事件。
    /// </summary>
    public class LoadConfigDependencyAssetEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载配置失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigDependencyAssetEventArgs).GetHashCode();

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
        /// 获取被加载的依赖资源名称
        /// </summary>
        public string DependencyAssetName { get; private set; }

        /// <summary>
        /// 获取当前已加载依赖资源数量
        /// </summary>
        public int LoadedCount { get; private set; }

        /// <summary>
        /// 获取总共加载依赖资源数量
        /// </summary>
        public int TotalCount { get; private set; }


        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载配置时加载依赖资源事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置时加载依赖资源事件
        /// </summary>
        /// <returns>加载配置时加载依赖资源事件</returns>
        public LoadConfigDependencyAssetEventArgs Fill(object userData,string configAssetName, string dependencyAssetName, int loadedCount, int totalCount)
        {
            LoadConfigInfo loadConfigInfo = (LoadConfigInfo)userData;
            ConfigName = loadConfigInfo.ConfigName;
            ConfigAssetName = configAssetName;
            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;
            UserData = loadConfigInfo.UserData;

            return this;
        }

    }
}

