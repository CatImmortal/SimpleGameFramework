using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Scene
{
    /// <summary>
    /// 加载场景失败事件
    /// </summary>
    public class LoadSceneFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载场景失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneFailureEventArgs).GetHashCode();

        /// <summary>
        /// 加载场景失败事件编号
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理加载场景失败事件
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景失败事件
        /// </summary>
        /// <returns>加载场景失败事件。</returns>
        public LoadSceneFailureEventArgs Fill(string sceneAssetName,string errorMessage,object userData)
        {
            SceneAssetName = sceneAssetName;
            ErrorMessage = errorMessage;
            UserData = userData;

            return this;
        }

    }
}

