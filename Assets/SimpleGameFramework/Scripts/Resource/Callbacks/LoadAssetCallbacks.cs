using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Resource
{
    #region 加载资源的回调方法
    /// <summary>
    /// 加载资源时加载依赖资源回调方法
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 加载资源成功回调方法
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="asset">已加载的资源。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData);

    /// <summary>
    /// 加载资源更新回调方法
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="progress">加载资源进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetUpdateCallback(string assetName, float progress, object userData);

    /// <summary>
    /// 加载资源失败回调方法
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="status">加载资源状态</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData);
    #endregion

    /// <summary>
    /// 加载资源回调方法集
    /// </summary>
    public class LoadAssetCallbacks
    {
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback = null,  LoadAssetFailureCallback loadAssetFailureCallback = null, LoadAssetUpdateCallback loadAssetUpdateCallback = null)
        {
            LoadAssetDependencyAssetCallback = loadAssetDependencyAssetCallback;
            LoadAssetSuccessCallback = loadAssetSuccessCallback;
            LoadAssetFailureCallback = loadAssetFailureCallback;
            LoadAssetUpdateCallback = loadAssetUpdateCallback;
        }

        /// <summary>
        /// 加载资源时加载依赖资源回调方法
        /// </summary>
        public LoadAssetDependencyAssetCallback LoadAssetDependencyAssetCallback { get; private set; }

        /// <summary>
        /// 加载资源成功回调方法
        /// </summary>
        public LoadAssetSuccessCallback LoadAssetSuccessCallback { get; private set; }

        /// <summary>
        /// 加载资源失败回调方法
        /// </summary>
        public LoadAssetFailureCallback LoadAssetFailureCallback { get; private set; }

        /// <summary>
        /// 加载资源更新回调方法
        /// </summary>
        public LoadAssetUpdateCallback LoadAssetUpdateCallback { get; private set; }

    }

}

