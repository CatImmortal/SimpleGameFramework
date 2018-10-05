using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Scene
{
    public class LoadSceneUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载场景更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 加载场景更新事件编号
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
        /// 获取加载场景进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 清理加载场景更新事件。
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = default(string);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景更新事件
        /// </summary>
        /// <returns>加载场景更新事件</returns>
        public LoadSceneUpdateEventArgs Fill(string sceneAssetName, float progress, object userData)
        {
            SceneAssetName = sceneAssetName;
            Progress = progress;
            UserData = userData;

            return this;
        }
    }
}

