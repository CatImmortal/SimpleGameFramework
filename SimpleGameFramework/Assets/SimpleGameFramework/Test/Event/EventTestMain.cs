using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using SimpleGameFramework.Base;
using System;
using SimpleGameFramework;

public class EventTestArgs : GlobalEventArgs
{
    public string m_Name;

    public override int Id
    {
        get
        {
            return 1;
        }
    }

    

    public override void Clear()
    {
        m_Name = string.Empty;
    }

    /// <summary>
    /// 事件填充
    /// </summary>
    public EventTestArgs Fill(string name)
    {
        m_Name = name;

        return this;
    }
}

public class EventTestMain : MonoBehaviour
{

     void Start()
    {
        //订阅事件
        FrameworkEntry.Instance.GetManager<EventManager>().Subscribe(1, EventTestMethod);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventTestArgs e = ReferencePool.Acquire<EventTestArgs>();
           
            //派发事件
            FrameworkEntry.Instance.GetManager<EventManager>().Fire(this, e.Fill("EventArgs"));
        }
    }

    /// <summary>
    /// 事件处理方法
    /// </summary>
    private void EventTestMethod(object sender, GlobalEventArgs e)
    {
        EventTestArgs args = e as EventTestArgs;
        Debug.Log(args.m_Name);
    }


    


}
