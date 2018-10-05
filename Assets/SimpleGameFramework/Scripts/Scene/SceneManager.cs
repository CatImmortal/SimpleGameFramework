using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Resource;

namespace SimpleGameFramework.Scene
{
    public class SceneManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 已加载场景
        /// </summary>
        private List<string> m_LoadedSceneAssetNames;

        /// <summary>
        /// 加载中场景
        /// </summary>
        private List<string> m_LoadingSceneAssetNames;

        /// <summary>
        /// 卸载中场景
        /// </summary>
        private List<string> m_UnloadingSceneAssetNames;

        /// <summary>
        /// 加载场景回调方法集
        /// </summary>
        private LoadSceneCallbacks m_LoadSceneCallbacks;

        /// <summary>
        /// 卸载场景回调方法集
        /// </summary>
        private UnloadSceneCallbacks m_UnloadSceneCallbacks;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;
        #endregion

        public SceneManager()
        {
            m_LoadedSceneAssetNames = new List<string>();
            m_LoadingSceneAssetNames = new List<string>();
            m_UnloadingSceneAssetNames = new List<string>();
            m_LoadSceneCallbacks = new LoadSceneCallbacks(LoadSceneSuccessCallback, LoadSceneDependencyAssetCallback, LoadSceneUpdateCallback, LoadSceneFailureCallback);
            m_UnloadSceneCallbacks = new UnloadSceneCallbacks(UnloadSceneSuccessCallback, UnloadSceneFailureCallback);
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();
        }

        public override void Init()
        {
           
        }


        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        /// <summary>
        /// 关闭并清理场景管理器
        /// </summary>
        public override void Shutdown()
        {
            string[] loadedSceneAssetNames = m_LoadedSceneAssetNames.ToArray();
            foreach (string loadedSceneAssetName in loadedSceneAssetNames)
            {
                if (SceneIsUnloading(loadedSceneAssetName))
                {
                    continue;
                }

                UnloadScene(loadedSceneAssetName);
            }

            m_LoadedSceneAssetNames.Clear();
            m_LoadingSceneAssetNames.Clear();
            m_UnloadingSceneAssetNames.Clear();
        }

        #region 场景检查
        /// <summary>
        /// 获取场景是否正在加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在加载</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景名称为空");
            }

            return m_LoadingSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取场景是否已加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否已加载</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景名称为空");
            }

            return m_LoadedSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取场景是否正在卸载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在卸载</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景名称为空");
            }

            return m_UnloadingSceneAssetNames.Contains(sceneAssetName);
        }
        #endregion

        #region 场景名称获取
        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <returns>已加载场景的资源名称</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return m_LoadedSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <returns>正在加载场景的资源名称</returns>
        public string[] GetLoadingSceneAssetNames()
        {
            return m_LoadingSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <returns>正在卸载场景的资源名称</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return m_UnloadingSceneAssetNames.ToArray();
        }
        #endregion

        #region 场景加载
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景名称为空，无法加载场景");
                return;
            }

            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法加载场景");
                return;
            }

            if (SceneIsUnloading(sceneAssetName))
            {
                Debug.LogError("场景正在卸载，无法加载场景");
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                Debug.LogError("场景正在加载，无法加载场景");
                return;
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                Debug.LogError("场景已加载，无法加载场景");
                return;
            }

            m_LoadingSceneAssetNames.Add(sceneAssetName);
            m_ResourceManager.LoadScene(sceneAssetName, m_LoadSceneCallbacks, userData);
        }
        #endregion

        #region 场景卸载

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景名称为空，无法卸载场景");
                return;
            }

            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法卸载场景");
                return;
            }

            if (SceneIsUnloading(sceneAssetName))
            {
                Debug.LogError("场景正在卸载，无法卸载场景");
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                Debug.LogError("场景正在加载，无法卸载场景");
                return;
            }

            if (!SceneIsLoaded(sceneAssetName))
            {
                Debug.LogError("场景未加载，无法卸载场景");
                return;
            }

            m_UnloadingSceneAssetNames.Add(sceneAssetName);
            m_ResourceManager.UnloadScene(sceneAssetName, m_UnloadSceneCallbacks, userData);
        }
        #endregion

        /// <summary>
        /// 获取场景名称
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景名称</returns>
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Debug.LogError("场景资源名称为空，无法获取场景名称");
                return null;
            }

            //检查场景资源名称是否合法（即是否是Assets下的完整路径）
            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if (sceneNamePosition + 1 >= sceneAssetName.Length)
            {
                Debug.LogError("场景资源名称不合法：" + sceneAssetName);
                return null;
            }

            //分割出场景名
            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }

        #region 加载与卸载场景的回调方法
        private void LoadSceneSuccessCallback(string sceneAssetName, float duration, object userData)
        {
            m_LoadingSceneAssetNames.Remove(sceneAssetName);
            m_LoadedSceneAssetNames.Add(sceneAssetName);

            //派发加载场景成功事件
            LoadSceneSuccessEventArgs e = ReferencePool.Acquire<LoadSceneSuccessEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName, duration, userData));
        }

        private void LoadSceneFailureCallback(string sceneAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            m_LoadingSceneAssetNames.Remove(sceneAssetName);
            //派发加载场景失败事件
            LoadSceneFailureEventArgs e = ReferencePool.Acquire<LoadSceneFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName, string.Format("加载场景{0}失败：{1}", sceneAssetName, errorMessage), userData));
        }

        private void LoadSceneUpdateCallback(string sceneAssetName, float progress, object userData)
        {
            //派发加载场景更新事件
            LoadSceneUpdateEventArgs e = ReferencePool.Acquire<LoadSceneUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName, progress, userData));
        }

        private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            //派发加载场景时加载依赖资源事件
            LoadSceneDependencyAssetEventArgs e = ReferencePool.Acquire<LoadSceneDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName,dependencyAssetName,loadedCount,totalCount,userData));
        }

        private void UnloadSceneSuccessCallback(string sceneAssetName, object userData)
        {
            m_UnloadingSceneAssetNames.Remove(sceneAssetName);
            m_LoadedSceneAssetNames.Remove(sceneAssetName);

            //派发卸载场景成功事件
            UnloadSceneSuccessEventArgs e = ReferencePool.Acquire<UnloadSceneSuccessEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName, userData));
        }

        private void UnloadSceneFailureCallback(string sceneAssetName, object userData)
        {
            m_UnloadingSceneAssetNames.Remove(sceneAssetName);

            //派发卸载场景失败事件
            UnloadSceneFailureEventArgs e = ReferencePool.Acquire<UnloadSceneFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(sceneAssetName, userData));
        }

        #endregion

    }
}

