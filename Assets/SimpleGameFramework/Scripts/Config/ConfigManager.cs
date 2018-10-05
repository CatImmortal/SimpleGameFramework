using SimpleGameFramework.Base;
using SimpleGameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public class ConfigManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 配置数据字典
        /// </summary>
        private Dictionary<string, ConfigData> m_ConfigDatas;

        /// <summary>
        /// 加载资源回调方法集
        /// </summary>
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 配置辅助器
        /// </summary>
        private ConfigHelperBase m_ConfigHelper;
        #endregion

        #region 构造方法
        public ConfigManager()
        {
            m_ConfigDatas = new Dictionary<string, ConfigData>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadConfigSuccessCallback, LoadConfigDependencyAssetCallback, LoadConfigFailureCallback, LoadConfigUpdateCallback);
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();
            m_ConfigHelper = null;
        }
        #endregion

        public override void Init()
        {
            SetConfigHelper(new DefaultConfigHelper());
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public override void Shutdown()
        {
            
        }

        /// <summary>
        /// 设置配置辅助器
        /// </summary>
        /// <param name="configHelper">配置辅助器</param>
        public void SetConfigHelper(ConfigHelperBase configHelper)
        {
            if (configHelper == null)
            {
                Debug.LogError("要设置的配置辅助器为空");
            }

            m_ConfigHelper = configHelper;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadConfig(string configName, string configAssetName, object userData = null)
        {
            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法加载配置");
                return;
            }

            if (m_ConfigHelper == null)
            {
                Debug.LogError("配置辅助器为空，无法加载配置");
                return;
            }
            if (string.IsNullOrEmpty(configName))
            {
                Debug.LogError("要加载的配置名称为空，无法加载配置");
                return;
            }
            if (string.IsNullOrEmpty(configAssetName))
            {
                Debug.LogError("要加载的配置资源名称为空，无法加载配置");
                return;
            }
            LoadConfigInfo info = new LoadConfigInfo(configName, userData);
            m_ResourceManager.LoadAsset(configAssetName, m_LoadAssetCallbacks, info);
        }

        /// <summary>
        /// 检查是否存在指定配置项
        /// </summary>
        /// <param name="configName">要检查配置项的名称</param>
        /// <returns>指定的配置项是否存在</returns>
        public bool HasConfig(string configName)
        {
            return GetConfigData(configName).HasValue;
        }

        /// <summary>
        /// 增加指定配置项
        /// </summary>
        /// <param name="configName">要增加配置项的名称</param>
        /// <param name="boolValue">配置项布尔值</param>
        /// <param name="intValue">配置项整数值</param>
        /// <param name="floatValue">配置项浮点数值</param>
        /// <param name="stringValue">配置项字符串值</param>
        /// <returns>是否增加配置项成功</returns>
        public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            if (HasConfig(configName))
            {
                return false;
            }

            m_ConfigDatas.Add(configName, new ConfigData(boolValue, intValue, floatValue, stringValue));
            return true;
        }

        /// <summary>
        /// 移除指定配置项
        /// </summary>
        /// <param name="configName">要移除配置项的名称</param>
        public void RemoveConfig(string configName)
        {
            m_ConfigDatas.Remove(configName);
        }

        /// <summary>
        /// 清空所有配置项
        /// </summary>
        public void RemoveAllConfigs()
        {
            m_ConfigDatas.Clear();
        }

        #region 读取配置项中的各类型数据
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns></returns>
        private ConfigData? GetConfigData(string configName)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Debug.LogError("配置名为空，无法获取配置数据");
                return null;
            }

            ConfigData configData = default(ConfigData);
            if (m_ConfigDatas.TryGetValue(configName, out configData))
            {
                return configData;
            }

            return null;
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                Debug.LogError("配置数据不存在：" + configName);
                return default(bool);
            }

            return configData.Value.BoolValue;
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName, bool defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.BoolValue : defaultValue;
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                Debug.LogError("配置数据不存在：" + configName);
                return default(int);
            }

            return configData.Value.IntValue;
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName, int defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.IntValue : defaultValue;
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                Debug.LogError("配置数据不存在：" + configName);
                return default(float);
            }

            return configData.Value.FloatValue;
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName, float defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.FloatValue : defaultValue;
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                Debug.LogError("配置数据不存在：" + configName);
                return string.Empty;
            }

            return configData.Value.StringValue;
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName, string defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.StringValue : defaultValue;
        }
        #endregion

        #region 加载资源的4个回调方法
        private void LoadConfigSuccessCallback(string configAssetName, object configAsset, float duration, object userData)
        {
            try
            {
                //这里的userData是LoadConfigInfo对象
                if (!m_ConfigHelper.LoadConfig(configAsset,userData))
                {
                    throw new Exception(string.Format("加载配置失败： '{0}'.", configAssetName));
                }
            }
            catch (Exception exception)
            {
                //派发加载配置失败事件
                LoadConfigFailureEventArgs fe = ReferencePool.Acquire<LoadConfigFailureEventArgs>();
                m_EventManager.Fire(this, fe.Fill(userData, configAssetName, exception.ToString()));

                throw;
            }
            finally
            {
                m_ConfigHelper.ReleaseConfigAsset(configAsset);
            }

            //派发加载配置成功事件
            LoadConfigSuccessEventArgs se = ReferencePool.Acquire<LoadConfigSuccessEventArgs>();
            m_EventManager.Fire(this, se.Fill(userData,configAssetName,duration));
        }

        private void LoadConfigFailureCallback(string configAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            //派发加载配置失败事件
            LoadConfigFailureEventArgs e = ReferencePool.Acquire<LoadConfigFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, configAssetName, string.Format("加载配置{0}失败：{1}",configAssetName,errorMessage)));
        }

        private void LoadConfigUpdateCallback(string configAssetName, float progress, object userData)
        {
            //派发加载配置更新事件
            LoadConfigUpdateEventArgs e = ReferencePool.Acquire<LoadConfigUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, configAssetName, progress));
        }

        private void LoadConfigDependencyAssetCallback(string configAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            //派发加载配置依赖资源事件
            LoadConfigDependencyAssetEventArgs e = ReferencePool.Acquire<LoadConfigDependencyAssetEventArgs>();
            m_EventManager.Fire(this,e.Fill(userData,configAssetName, dependencyAssetName, loadedCount, totalCount));
        }

       
        #endregion
    }
}

