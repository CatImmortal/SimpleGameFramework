using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Scene
{
    /// <summary>
    /// 加载场景成功事件
    /// </summary>
    public class LoadSceneSuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载场景成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 加载场景成功事件编号
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
        /// 获取加载持续时间
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 清理加载场景成功事件。
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = default(string);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景成功事件
        /// </summary>
        /// <returns>加载场景成功事件。</returns>
        public LoadSceneSuccessEventArgs Fill(string sceneAssetName,float duration,object userData)
        {
            SceneAssetName = sceneAssetName;
            Duration = duration;
            UserData = userData;

            return this;
        }
    }
}

