using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework
{
    /// <summary>
    /// 引用集合
    /// </summary>
    public class ReferenceCollection
    {
        /// <summary>
        /// 引用队列
        /// </summary>
        private Queue<IReference> m_References;

        public ReferenceCollection()
        {
            m_References = new Queue<IReference>();
        }

        #region 引用队列的操作

        /// <summary>
        /// 获取引用
        /// </summary>
        public T Acquire<T>() where T : class, IReference, new()
        {
            lock (m_References)
            {
                if (m_References.Count > 0)
                {
                    return m_References.Dequeue() as T;
                }
            }

            return new T();
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        public IReference Acquire(Type referenceType)
        {
            lock (m_References)
            {
                if (m_References.Count > 0)
                {
                    return m_References.Dequeue();
                }
            }
         
            return (IReference)Activator.CreateInstance(referenceType);
        }

        /// <summary>
        /// 释放引用
        /// </summary>
        public void Release<T>(T reference) where T : class, IReference
        {
            reference.Clear();
            lock (m_References)
            {
                m_References.Enqueue(reference);
            }
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        public void Add<T>(int count) where T : class, IReference, new()
        {
            lock (m_References)
            {
                while (count-- > 0)
                {
                    m_References.Enqueue(new T());
                }
            }
        }

        /// <summary>
        /// 删除引用
        /// </summary>
        public void Remove<T>(int count) where T : class, IReference
        {
            lock (m_References)
            {
                if (count > m_References.Count)
                {
                    count = m_References.Count;
                }

                while (count-- > 0)
                {
                    m_References.Dequeue();
                }
            }
        }

        /// <summary>
        /// 删除所有引用
        /// </summary>
        public void RemoveAll()
        {
            lock (m_References)
            {
                m_References.Clear();
            }
        }
        #endregion
    }
}

