using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 加载配置的信息
    /// </summary>
    public class LoadConfigInfo
    {
        public LoadConfigInfo(string configName, object userData)
        {
            ConfigName = configName;
            UserData = userData;
        }

        public string ConfigName { get; private set; }
        public object UserData { get; private set; }
    }
}

