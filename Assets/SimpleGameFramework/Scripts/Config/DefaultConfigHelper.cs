using SimpleGameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Config
{
    public class DefaultConfigHelper : ConfigHelperBase
    {
        private static readonly string[] ColumnSplit = new string[] { "\t" };
        private const int ColumnCount = 4;
        private ResourceManager m_ResourceManager = null;
        private ConfigManager m_ConfigManager = null;

        public DefaultConfigHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_ConfigManager  = FrameworkEntry.Instance.GetManager<ConfigManager>();
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="text">要解析的配置文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功。</returns>
        public override bool ParseConfig(string text, object userData)
        {
            try
            {
                //按行切分
                string[] rowTexts = Utility.Text.SplitToLines(text);
                for (int i = 0; i < rowTexts.Length; i++)
                {
                    //跳过#注释行
                    if (rowTexts[i].Length <= 0 || rowTexts[i][0] == '#')
                    {
                        continue;
                    }

                    //按列切分
                    string[] splitLine = rowTexts[i].Split(ColumnSplit, StringSplitOptions.None);
                    if (splitLine.Length != ColumnCount)
                    {
                        Debug.LogError("无法解析配置：" + text);
                        return false;
                    }

                    //第2列和第4列是键值对
                    string configName = splitLine[1];
                    string stringValue = splitLine[3];

                    bool boolValue = default(bool);
                    bool.TryParse(stringValue, out boolValue);

                    int intValue = default(int);
                    int.TryParse(stringValue, out intValue);

                    float floatValue = default(float);
                    float.TryParse(stringValue, out floatValue);

                    if (!m_ConfigManager.AddConfig(configName, boolValue, intValue, floatValue, stringValue))
                    {
                        Debug.LogError("添加配置失败：" + configName);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError("无法解析配置：" + exception.Message);
                return false;
            }
        }
        

        /// <summary>
        /// 释放配置资源
        /// </summary>
        /// <param name="dataTableAsset">要释放的配置资源</param>
        public override void ReleaseConfigAsset(object configAsset)
        {
            m_ResourceManager.UnloadAsset(configAsset);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称（报错用）</param>
        /// <param name="configAsset">配置资源</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>加载是否成功</returns>
        protected override bool LoadConfig(string configName, object configAsset, object userData)
        {

            TextAsset textAsset = configAsset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("要加载的配置资源为空：" + configName);
                return false;
            }

            bool retVal = ParseConfig(textAsset.text, userData);
            if (!retVal)
            {
                Debug.LogError("配置资源解析失败");
            }

            return retVal;
        }
    }
}

