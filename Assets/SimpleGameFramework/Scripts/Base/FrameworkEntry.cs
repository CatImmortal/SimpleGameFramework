using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
    /// <summary>
    /// 框架入口，维护所有模块管理器
    /// </summary>
    public class FrameworkEntry :ScriptSingleton<FrameworkEntry>
    {
        /// <summary>
        /// 所有模块管理器的链表
        /// </summary>
        private  LinkedList<ManagerBase> m_Managers = new LinkedList<ManagerBase>();


         void Update()
        {
            //轮询所有管理器
            foreach (ManagerBase manager in m_Managers)
            {
                manager.Update(Time.deltaTime,Time.unscaledDeltaTime);
            }
        }

        protected override void OnDestroy()
        {
            //关闭并清理所有管理器
            for (LinkedListNode<ManagerBase> current = m_Managers.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }
            m_Managers.Clear();
        }


        /// <summary>
        /// 获取指定管理器
        /// </summary>
        public T GetManager<T>() where T : ManagerBase
        {
            Type managerType = typeof(T);
            foreach (ManagerBase manager in  m_Managers)
            {
                if (manager.GetType() == managerType)
                {
                    return manager as T;
                }
            }

            //没找到就创建
            return CreateManager(managerType) as T;
        }

        /// <summary>
        /// 创建模块管理器
        /// </summary>
        private ManagerBase CreateManager(Type managerType)
        {

            ManagerBase manager = (ManagerBase)Activator.CreateInstance(managerType);

            if (manager == null)
            {
                Debug.LogError("模块管理器创建失败：" + manager.GetType().FullName);
            }


            //根据模块优先级决定它在链表里的位置
            LinkedListNode<ManagerBase> current = m_Managers.First;
            while (current != null)
            {
                
                if (manager.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }
            if (current != null)
            {
                
                m_Managers.AddBefore(current, manager);
            }
            else
            {
                
                m_Managers.AddLast(manager);
            }

            //初始化模块管理器
            manager.Init();
            return manager;
        }



    }


