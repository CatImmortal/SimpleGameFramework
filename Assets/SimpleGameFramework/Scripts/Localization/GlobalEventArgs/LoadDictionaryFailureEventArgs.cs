using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 加载字典失败事件
    /// </summary>
    public class LoadDictionaryFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载字典失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDictionaryFailureEventArgs).GetHashCode();

        /// <summary>
        /// 加载字典失败事件编号
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
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理加载字典失败事件
        /// </summary>
        public override void Clear()
        {
            DictionaryName = default(string);
            DictionaryAssetName = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载字典失败事件
        /// </summary>
        /// <returns>加载字典失败事件</returns>
        public LoadDictionaryFailureEventArgs Fill(object userData,string dictionaryAssetName, string errorMessage)
        {
            LoadDictionaryInfo loadDictionaryInfo = (LoadDictionaryInfo)userData;
            DictionaryName = loadDictionaryInfo.DictionaryName;
            DictionaryAssetName = dictionaryAssetName;
            ErrorMessage = errorMessage;
            UserData = loadDictionaryInfo.UserData;

            return this;
        }
    }
}

