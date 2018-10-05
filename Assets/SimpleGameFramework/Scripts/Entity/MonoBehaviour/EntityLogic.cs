﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体逻辑基类
    /// </summary>
    public abstract class EntityLogic : MonoBehaviour
    {
        #region 字段与属性
        private Transform m_OriginalTransform = null;

        /// <summary>
        /// 已缓存的 Transform
        /// </summary>
        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// 获取实体
        /// </summary>
        public Entity Entity
        {
            get
            {
                return GetComponent<Entity>();
            }
        }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        /// <summary>
        /// 获取实体是否可用
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return gameObject.activeSelf;
            }
        }
        #endregion

        #region 实体相关方法
        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnInit(object userData)
        {
            if (CachedTransform == null)
            {
                CachedTransform = transform;
            }

            m_OriginalTransform = CachedTransform.parent;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnShow(object userData)
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnHide(object userData)
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnDetached(EntityLogic childEntity, object userData)
        {

        }

        /// <summary>
        /// 实体附加父实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            CachedTransform.SetParent(parentTransform);
        }

        /// <summary>
        /// 实体解除父实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            CachedTransform.SetParent(m_OriginalTransform);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
        #endregion



    }
}

