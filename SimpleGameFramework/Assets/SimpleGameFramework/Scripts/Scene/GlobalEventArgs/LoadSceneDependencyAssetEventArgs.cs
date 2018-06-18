using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Scene
{
    /// <summary>
    /// 加载场景时加载依赖资源事件
    /// </summary>
    public class LoadSceneDependencyAssetEventArgs : GlobalEventArgs
    {

        /// <summary>
        /// 加载场景时加载依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        /// 加载场景时加载依赖资源事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 场景资源名称
        /// </summary>
        public string SceneAssetName { get; private set; }

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
        /// 清理加载场景时加载依赖资源事件
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景时加载依赖资源事件
        /// </summary>
        /// <returns>加载场景时加载依赖资源事件</returns>
        public LoadSceneDependencyAssetEventArgs Fill(string sceneAssetName,string dependencyAssetName,int loadedCount,int totalCount,object userData)
        {
            SceneAssetName = sceneAssetName;
            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;
            UserData = userData;

            return this;
        }
    }
}

