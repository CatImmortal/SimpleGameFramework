using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Entity;
public class EntityTestMain : MonoBehaviour {

    private EntityManager m_EntityManager;
    private int id = 0;
	// Use this for initialization
	void Start () {
        m_EntityManager = FrameworkEntry.Instance.GetManager<EntityManager>();

        m_EntityManager.AddEntityGroup("Cube",5,5,5);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            m_EntityManager.ShowEntity(id++, typeof(EntityTestLogic), "EntityTestCube", "Cube", null);
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_EntityManager.HideEntity(--id);
        }
	}
}
