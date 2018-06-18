using SimpleGameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 默认本地化辅助器
    /// </summary>
    public class DefaultLocalizationHelper : LocalizationHelperBase
    {
        private static string[] s_ColumnSplit = new string[] { "\t" };
        private const int ColumnCount = 4;

        private ResourceManager m_ResourceManager;
        private LocalizationManager m_LocalizationManager;

        public DefaultLocalizationHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_LocalizationManager = FrameworkEntry.Instance.GetManager<LocalizationManager>();
        }

        /// <summary>
        /// 获取系统语言
        /// </summary>
        public override Language SystemLanguage
        {
            get
            {
                switch (Application.systemLanguage)
                {
                   
                    case UnityEngine.SystemLanguage.Chinese:
                        break;
                    case UnityEngine.SystemLanguage.ChineseSimplified:
                        return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseTraditional:
                        return Language.ChineseTraditional;
                    case UnityEngine.SystemLanguage.English:
                        return Language.English;
                    case UnityEngine.SystemLanguage.Japanese:
                        return Language.Japanese;
                    default:
                        return Language.Unspecified;
                }
                return Language.Unspecified;
            }
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="text">要解析的字典文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public override bool ParseDictionary(string text, object userData)
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
                    string[] splitLine = rowTexts[i].Split(s_ColumnSplit, StringSplitOptions.None);
                    if (splitLine.Length != ColumnCount)
                    {
                        Debug.LogError("文本列数与定义列数不符合："+ text);
                        return false;
                    }
                    //第2列和第4列是键值对
                    string key = splitLine[1];
                    string value = splitLine[3];
                    if (!AddRawString(key, value))
                    {
                        Debug.LogError("将本地化资源添加到字典里失败：" + key);
                        return false; 
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError("解析字典时出现异常：" + exception.Message);
                return false;
            }
        }

        /// <summary>
        /// 释放字典资源
        /// </summary>
        /// <param name="dictionaryAsset">要释放的字典资源</param>
        public override void ReleaseDictionaryAsset(object dictionaryAsset)
        {
            m_ResourceManager.UnloadAsset(dictionaryAsset);
        }

        /// <summary>
        /// 加载字典
        /// </summary>
        /// <param name="dictionaryName">字典名称</param>
        /// <param name="dictionaryAsset">字典资源</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>加载是否成功</returns>
        protected override bool LoadDictionary(string dictionaryName, object dictionaryAsset, object userData)
        {
            TextAsset textAsset = dictionaryAsset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("字典资源为空，无法加载字典：" + dictionaryName);
                return false;
            }

            bool retVal = ParseDictionary(textAsset.text, userData);
            if (!retVal)
            {
                Debug.LogError("字典解析失败：" + dictionaryName);
            }

            return retVal;
        }

        /// <summary>
        /// 增加字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <param name="value">字典内容</param>
        /// <returns>是否增加字典成功</returns>
        protected bool AddRawString(string key, string value)
        {
            return m_LocalizationManager.AddRawString(key, value);
        }
    }
}

