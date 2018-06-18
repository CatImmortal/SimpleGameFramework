using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.ObjectPool;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体实例对象
    /// </summary>
    public class EntityInstanceObject : ObjectBase
    {
        /// <summary>
        /// 实体资源
        /// </summary>
        private object m_EntityAsset;

        /// <summary>
        /// 实体辅助器
        /// </summary>
        private EntityHelperBase m_EntityHelper;

        public EntityInstanceObject(string name,object entityAsset,object entityInstance,EntityHelperBase entityHelper)
            : base(entityInstance,name)
        {
            if (entityAsset == null)
            {
                Debug.LogError("用来构造实体实例对象的实体资源为空");
            }
            if (entityHelper == null)
            {
                Debug.LogError("用来构造实体实例对象的实体辅助器为空");
            }

            m_EntityAsset = entityAsset;
            m_EntityHelper = entityHelper;
        }

        public override void Release()
        {
            m_EntityHelper.ReleaseEntity(m_EntityAsset, Target);
        }
    }
}

