using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Resource
{
    #region 加载场景的回调方法
    /// <summary>
    /// 加载场景时加载依赖资源回调方法
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 加载场景成功回调方法
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称</param>
    /// <param name="duration">加载持续时间</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadSceneSuccessCallback(string sceneAssetName, float duration, object userData);

    /// <summary>
    /// 加载场景更新回调函数
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称</param>
    /// <param name="progress">加载场景进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadSceneUpdateCallback(string sceneAssetName, float progress, object userData);

    /// <summary>
    /// 加载场景失败回调函数
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称</param>
    /// <param name="status">加载场景状态</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadSceneFailureCallback(string sceneAssetName, LoadResourceStatus status, string errorMessage, object userData);
    #endregion

    /// <summary>
    /// 加载场景回调方法集
    /// </summary>
    public class LoadSceneCallbacks
    {
        public LoadSceneCallbacks(LoadSceneSuccessCallback loadSceneSuccessCallback, LoadSceneDependencyAssetCallback loadSceneDependencyAssetCallback = null, LoadSceneUpdateCallback loadSceneUpdateCallback = null, LoadSceneFailureCallback loadSceneFailureCallback = null)
        {
            LoadSceneDependencyAssetCallback = loadSceneDependencyAssetCallback;
            LoadSceneSuccessCallback = loadSceneSuccessCallback;
            LoadSceneUpdateCallback = loadSceneUpdateCallback;
            LoadSceneFailureCallback = loadSceneFailureCallback;
        }

        /// <summary>
        /// 加载场景时加载依赖资源回调方法
        /// </summary>
        public LoadSceneDependencyAssetCallback LoadSceneDependencyAssetCallback { get; private set; }

        /// <summary>
        /// 加载场景成功回调方法
        /// </summary>
        public LoadSceneSuccessCallback LoadSceneSuccessCallback { get; private set; }

        /// <summary>
        /// 加载场景更新回调方法
        /// </summary>
        public LoadSceneUpdateCallback LoadSceneUpdateCallback { get; private set; }

        /// <summary>
        /// 加载场景失败回调方法
        /// </summary>
        public LoadSceneFailureCallback LoadSceneFailureCallback { get; private set; }

    }
}

