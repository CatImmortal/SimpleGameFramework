using SimpleGameFramework.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Resource
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager : ManagerBase
    {
        public override void Init()
        {
           
        }

        public override void Shutdown()
        {
            
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        #region 资源的加载与卸载
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="loadAssetCallbacks">加载资源回调方法集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("要加载资源的名称为空");
                return;
            }

            if (loadAssetCallbacks == null)
            {
                Debug.LogError("加载资源回调方法集为空");
                return;
            }
            //TODO 使用加载器进行资源加载
            //m_ResourceLoader.LoadAsset(assetName, loadAssetCallbacks, userData);

            //测试文本资源的加载
            //TextAsset textAsset = Resources.Load<TextAsset>(assetName);
            //loadAssetCallbacks.LoadAssetSuccessCallback(assetName, textAsset, 0, userData);

            //测试游戏物体资源的加载
            GameObject go = Resources.Load<GameObject>(assetName);
            loadAssetCallbacks.LoadAssetSuccessCallback(assetName, go, 0, userData);
        }

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            if (asset == null)
            {
                Debug.LogError("要卸载的资源为空");
            }

            //if (m_ResourceLoader == null)
            //{
            //    return;
            //}

            ////TODO使用加载器进行卸载资源
            //m_ResourceLoader.UnloadAsset(asset);
        }
        #endregion

        #region 场景的加载与卸载
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="loadSceneCallbacks">加载场景回调方法集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("要加载场景的名称为空");
                return;
            }

            if (loadSceneCallbacks == null)
            {
                Debug.LogError("加载场景回调方法集为空");
                return;
            }
            //使用加载器加载场景
            //m_ResourceLoader.LoadScene(sceneAssetName, loadSceneCallbacks, userData);

            //测试
            UnityEngine.SceneManagement.SceneManager.LoadScene(Scene.SceneManager.GetSceneName(sceneAssetName));
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调方法集</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("要卸载场景的名称为空");
                return;
            }

            if (unloadSceneCallbacks == null)
            {
                Debug.LogError("卸载场景回调方法集为空");
                return;
            }
            //使用加载器卸载场景
            //m_ResourceLoader.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }
        #endregion
    }
}

