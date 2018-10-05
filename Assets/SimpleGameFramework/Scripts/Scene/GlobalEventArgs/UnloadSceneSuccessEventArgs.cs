using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Scene
{
    /// <summary>
    /// 卸载场景成功事件
    /// </summary>
    public class UnloadSceneSuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载场景成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(UnloadSceneSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载场景成功事件编号
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
        /// 清理卸载场景失败事件
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充卸载场景成功事件
        /// </summary>
        /// <returns>卸载场景成功事件</returns>
        public UnloadSceneSuccessEventArgs Fill(string sceneAssetName, object userData)
        {
            SceneAssetName = sceneAssetName;
            UserData = userData;

            return this;
        }
    }
}

