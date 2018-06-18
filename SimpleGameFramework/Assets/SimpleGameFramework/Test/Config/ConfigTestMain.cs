using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Config;
public class ConfigTestMain : MonoBehaviour {

    private ConfigManager m_ConfigManager;

    private void Start()
    {
        m_ConfigManager = FrameworkEntry.Instance.GetManager<ConfigManager>();
        m_ConfigManager.LoadConfig("Default", "DefaultConfig");
        int num = m_ConfigManager.GetInt("Scene.Menu");
        Debug.Log(num);
        string str = m_ConfigManager.GetString("Game.Id");
        Debug.Log(str);
    }
}
