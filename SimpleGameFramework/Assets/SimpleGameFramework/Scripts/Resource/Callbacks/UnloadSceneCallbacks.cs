using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Resource
{
    #region 卸载场景的回调方法
    /// <summary>
    /// 卸载场景成功回调方法
    /// </summary>
    /// <param name="sceneAssetName">要卸载的场景资源名称</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void UnloadSceneSuccessCallback(string sceneAssetName, object userData);

    /// <summary>
    /// 卸载场景失败回调方法
    /// </summary>
    /// <param name="sceneAssetName">要卸载的场景资源名称</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void UnloadSceneFailureCallback(string sceneAssetName, object userData);
    #endregion

    /// <summary>
    /// 卸载场景回调方法集
    /// </summary>
    public class UnloadSceneCallbacks
    {

        /// <summary>
        /// 卸载场景成功回调方法
        /// </summary>
        public UnloadSceneSuccessCallback UnloadSceneSuccessCallback { get; private set; }

        /// <summary>
        /// 卸载场景失败回调方法
        /// </summary>
        public UnloadSceneFailureCallback UnloadSceneFailureCallback { get; private set; }

        public UnloadSceneCallbacks(UnloadSceneSuccessCallback unloadSceneSuccessCallback, UnloadSceneFailureCallback unloadSceneFailureCallback)
        {
            UnloadSceneSuccessCallback = unloadSceneSuccessCallback;
            UnloadSceneFailureCallback = unloadSceneFailureCallback;
        }
    }
}

