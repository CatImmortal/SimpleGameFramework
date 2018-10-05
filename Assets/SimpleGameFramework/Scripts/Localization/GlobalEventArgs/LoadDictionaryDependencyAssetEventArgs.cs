using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 加载字典时加载依赖资源事件
    /// </summary>
    public class LoadDictionaryDependencyAssetEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载字典时加载依赖资源事件
        /// </summary>
        public static readonly int EventId = typeof(LoadDictionaryDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        ///  加载字典时加载依赖资源事件
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 字典名称
        /// </summary>
        public string DictionaryName { get; private set; }

        /// <summary>
        /// 字典资源名称
        /// </summary>
        public string DictionaryAssetName { get; private set; }

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
        /// 清理加载字典时加载依赖资源事件
        /// </summary>
        public override void Clear()
        {
            DictionaryName = default(string);
            DictionaryAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载字典时加载依赖资源事件
        /// </summary>
        /// <returns>加载字典时加载依赖资源事件</returns>
        public LoadDictionaryDependencyAssetEventArgs Fill(object userData,string dictionaryAssetName,string dependencyAssetName,int loadedCount,int totalCount)
        {
            LoadDictionaryInfo loadDictionaryInfo = (LoadDictionaryInfo)userData;
            DictionaryName = loadDictionaryInfo.DictionaryName;
            DictionaryAssetName = dictionaryAssetName;
            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;
            UserData = loadDictionaryInfo.UserData;

            return this;
        }
    }
}

