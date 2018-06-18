using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Scene;
public class SceneTestMain : MonoBehaviour {

    SceneManager m_SceneManager;

	// Use this for initialization
	void Start () {
        m_SceneManager = FrameworkEntry.Instance.GetManager<SceneManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            m_SceneManager.LoadScene("Asset/SimpleGameFramework/Test/Scene/scenetest");
        }
	}
}
