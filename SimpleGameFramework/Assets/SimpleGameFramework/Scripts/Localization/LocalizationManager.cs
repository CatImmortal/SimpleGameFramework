using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Resource;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 本地化管理器
    /// </summary>
    public class LocalizationManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 本地化资源的字典
        /// </summary>
        private Dictionary<string, string> m_Dictionary;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 加载资源回调方法集
        /// </summary>
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 本地化辅助器
        /// </summary>
        private LocalizationHelperBase m_LocalizationHelper;

        /// <summary>
        /// 本地化语言
        /// </summary>
        private Language m_Language;

        /// <summary>
        /// 本地化语言
        /// </summary>
        public Language Language
        {
            get
            {
                return m_Language;
            }
            set
            {
                if (value == Language.Unspecified)
                {
                    Debug.LogError("本地化语言未指定");
                }

                m_Language = value;
            }
        }


        /// <summary>
        /// 系统语言
        /// </summary>
        public Language SystemLanguage
        {
            get
            {
                if (m_LocalizationHelper == null)
                {
                    Debug.LogError("获取系统语言时本地化辅助器为空");
                }

                return m_LocalizationHelper.SystemLanguage;
            }
        }

        /// <summary>
        /// 获取字典数量
        /// </summary>
        public int DictionaryCount
        {
            get
            {
                return m_Dictionary.Count;
            }
        }
        #endregion



        public LocalizationManager()
        {
            m_Dictionary = new Dictionary<string, string>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadDictionarySuccessCallback, LoadDictionaryDependencyAssetCallback,LoadDictionaryFailureCallback, LoadDictionaryUpdateCallback);
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();
            m_LocalizationHelper = null;
            m_Language = Language.Unspecified;
        }

        public override void Init()
        {
            
        }

        public override void Shutdown()
        {
            
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        /// <summary>
        /// 设置本地化辅助器。
        /// </summary>
        /// <param name="localizationHelper">本地化辅助器。</param>
        public void SetLocalizationHelper(LocalizationHelperBase localizationHelper)
        {
            if (localizationHelper == null)
            {
                Debug.LogError("要设置的本地化辅助器为空");
                return;
            }

            m_LocalizationHelper = localizationHelper;
        }

        /// <summary>
        /// 加载字典
        /// </summary>
        /// <param name="dictionaryName">字典名称</param>
        /// <param name="dictionaryAssetName">字典资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDictionary(string dictionaryName, string dictionaryAssetName, object userData = null)
        {
            if (string.IsNullOrEmpty(dictionaryName))
            {
                Debug.LogError("要加载的字典名称为空");
                return;
            }

            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法加载字典");
                return;
            }

            if (m_LocalizationHelper == null)
            {
                Debug.LogError("本地化辅助器为空，无法加载字典");
                return;
            }

            LoadDictionaryInfo info = new LoadDictionaryInfo(dictionaryName, userData);
            m_ResourceManager.LoadAsset(dictionaryAssetName, m_LoadAssetCallbacks, info);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <param name="args">字典参数。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GetString(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("要获取内容的字典主键为空");
                return string.Empty;
            }

            string value = null;
            if (!m_Dictionary.TryGetValue(key, out value))
            {
                return string.Format("<NoKey>{0}", key);
            }

            try
            {
                return string.Format(value, args);
            }
            catch (Exception exception)
            {
                string errorString = string.Format("<Error>{0},{1}", key, value);
                if (args != null)
                {
                    foreach (object arg in args)
                    {
                        errorString += "," + arg.ToString();
                    }
                }

                errorString += "," + exception.Message;
                return errorString;
            }
        }

        /// <summary>
        /// 是否存在字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否存在字典</returns>
        public bool HasRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("字典主键为空，无法检测字典是否存在");
                return false;
            }

            return m_Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 根据字典主键获取字典值
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>字典值</returns>
        public string GetRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("字典主键为空，无法获取字典值");
                return string.Empty;
            }

            string value = null;
            if (m_Dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return string.Format("<NoKey>{0}", key);
        }

        /// <summary>
        /// 增加字典。
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <param name="value">字典内容</param>
        /// <returns>是否增加字典成功</returns>
        public bool AddRawString(string key, string value)
        {
            if (HasRawString(key))
            {
                return false;
            }

            m_Dictionary.Add(key, value ?? string.Empty);
            return true;
        }

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否移除字典成功</returns>
        public bool RemoveRawString(string key)
        {
            if (!HasRawString(key))
            {
                return false;
            }

            return m_Dictionary.Remove(key);
        }

        #region 加载资源的4个回调方法
        private void LoadDictionarySuccessCallback(string dictionaryAssetName, object dictionaryAsset, float duration, object userData)
        {
            try
            {
                //这里的userData是LoadDictionaryInfo对象
                if (!m_LocalizationHelper.LoadDictionary(dictionaryAsset,userData))
                {
                    throw new Exception("辅助器加载字典失败：" + dictionaryAssetName);
                }
            }
            catch (Exception exception)
            {
                //派发加载字典失败事件
                LoadDictionaryFailureEventArgs fe = ReferencePool.Acquire<LoadDictionaryFailureEventArgs>();
                m_EventManager.Fire(this, fe.Fill(userData, dictionaryAssetName, exception.Message));
            }
            finally
            {
                m_LocalizationHelper.ReleaseDictionaryAsset(dictionaryAsset);
            }

            //派发加载字典成功事件
            LoadDictionarySuccessEventArgs se = ReferencePool.Acquire<LoadDictionarySuccessEventArgs>();
            m_EventManager.Fire(this, se.Fill(userData, dictionaryAssetName, duration));
        }

        private void LoadDictionaryFailureCallback(string dictionaryAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            //派发加载字典失败事件
            LoadDictionaryFailureEventArgs fe = ReferencePool.Acquire<LoadDictionaryFailureEventArgs>();
            m_EventManager.Fire(this, fe.Fill(userData, dictionaryAssetName, errorMessage));
        }

        private void LoadDictionaryUpdateCallback(string dictionaryAssetName, float progress, object userData)
        {
            //派发加载字典更新事件
            LoadDictionaryUpdateEventArgs e = ReferencePool.Acquire<LoadDictionaryUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, dictionaryAssetName, progress));
        }

        private void LoadDictionaryDependencyAssetCallback(string dictionaryAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            //派发加载字典时加载依赖资源事件
            LoadDictionaryDependencyAssetEventArgs e = ReferencePool.Acquire<LoadDictionaryDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, dictionaryAssetName, dependencyAssetName, loadedCount, totalCount));
        }
        #endregion
    }
}

