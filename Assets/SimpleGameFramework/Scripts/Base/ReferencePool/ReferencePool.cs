﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework
{
    /// <summary>
    /// 引用池
    /// </summary>
    public static class ReferencePool
    {
        /// <summary>
        /// 引用集合的字典
        /// </summary>
        private static Dictionary<string,ReferenceCollection> s_ReferenceCollections = new Dictionary<string, ReferenceCollection>();

        /// <summary>
        /// 获取引用池的数量
        /// </summary>
        public static int Count
        {
            get
            {
                return s_ReferenceCollections.Count;
            }
        }

        /// <summary>
        /// 获取引用集合
        /// </summary>
        private static ReferenceCollection GetReferenceCollection(string fullName)
        {
            ReferenceCollection referenceCollection = null;
            lock (s_ReferenceCollections)
            {
                if (!s_ReferenceCollections.TryGetValue(fullName, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection();
                    s_ReferenceCollections.Add(fullName, referenceCollection);
                }
            }

            return referenceCollection;
        }
        /// <summary>
        /// 清除所有引用池
        /// </summary>
        public static void ClearAll()
        {
            lock (s_ReferenceCollections)
            {
                foreach (KeyValuePair<string, ReferenceCollection> referenceCollection in s_ReferenceCollections)
                {
                    referenceCollection.Value.RemoveAll();
                }

                s_ReferenceCollections.Clear();
            }
        }

        #region 引用池的操作

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">追加数量</param>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T).FullName).Add<T>(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">移除数量</param>
        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T).FullName).Remove<T>(count);
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T).FullName).Acquire<T>();
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        public static IReference Acquire(Type referenceType)
        {
            return GetReferenceCollection(referenceType.FullName).Acquire(referenceType);
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="reference">引用</param>
        public static void Release<T>(T reference) where T : class, IReference
        {
            if (reference == null)
            {
                Debug.LogError("要归还的引用为空");
            }

            GetReferenceCollection(typeof(T).FullName).Release(reference);
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T).FullName).RemoveAll();
        }

        #endregion
    }
}

