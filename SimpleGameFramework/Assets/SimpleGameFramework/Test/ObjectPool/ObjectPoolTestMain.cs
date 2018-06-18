using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleGameFramework.ObjectPool;
public class ObjectPoolTestMain : MonoBehaviour
{

    ObjectPool<TestObject> m_testPool;
   
     void Awake()
    {
        ObjectPoolManager m_objectPoolManager = FrameworkEntry.Instance.GetManager<ObjectPoolManager>();
        m_testPool = m_objectPoolManager.CreateObjectPool<TestObject>();

        TestObject testObject = new TestObject("hello ObjectPool","test1");
        m_testPool.Register(testObject, false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TestObject testObject = m_testPool.Spawn("test1");
            Debug.Log(testObject.Target);
            m_testPool.Unspawn(testObject.Target);
        }

    }
}
