using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 加载字典成功事件
    /// </summary>
    public class LoadDictionarySuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载字典成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDictionarySuccessEventArgs).GetHashCode();

        /// <summary>
        /// 加载字典成功事件编号
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
        /// 清理加载字典成功事件。
        /// </summary>
        public override void Clear()
        {
            DictionaryName = default(string);
            DictionaryAssetName = default(string);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 加载持续时间
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 填充加载字典成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>加载字典成功事件。</returns>
        public LoadDictionarySuccessEventArgs Fill(object userData, string dictionaryAssetName,float duration)
        {
            LoadDictionaryInfo loadDictionaryInfo = (LoadDictionaryInfo)userData;
            DictionaryName = loadDictionaryInfo.DictionaryName;
            DictionaryAssetName = dictionaryAssetName;
            Duration = duration;
            UserData = loadDictionaryInfo.UserData;

            return this;
        }
    }
}

