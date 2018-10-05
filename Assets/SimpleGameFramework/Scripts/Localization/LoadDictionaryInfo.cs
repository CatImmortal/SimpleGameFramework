using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Localization
{
    /// <summary>
    /// 加载字典的信息
    /// </summary>
    public class LoadDictionaryInfo
    {
    
        /// <summary>
        /// 字典名字
        /// </summary>
        public string DictionaryName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public LoadDictionaryInfo(string dictionaryName, object userData)
        {
            DictionaryName = dictionaryName;
            UserData = userData;
        }
    }
}

