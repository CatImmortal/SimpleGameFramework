using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体信息
    /// </summary>
    public class EntityInfo
    {
        #region 字段与属性
        private static readonly Entity[] EmptyArray = new Entity[] { };

        /// <summary>
        /// 实体对象
        /// </summary>
        public Entity Entity { get; private set; }

        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityStatus Status { get;  set; }

        /// <summary>
        /// 父实体
        /// </summary>
        public Entity ParentEntity { get; set; }

        /// <summary>
        /// 子实体列表
        /// </summary>
        private List<Entity> m_ChildEntities;
        #endregion


        public EntityInfo(Entity entity)
        {
            Entity = entity;
            Status = EntityStatus.WillInit;
            ParentEntity = null;
            m_ChildEntities = null;
        }

        /// <summary>
        /// 获取所有子实体
        /// </summary>
        public Entity[] GetChildEntities()
        {
            if (m_ChildEntities == null)
            {
                return EmptyArray;
            }

            return m_ChildEntities.ToArray();
        }

        /// <summary>
        /// 添加子实体
        /// </summary>
        /// <param name="childEntity"></param>
        public void AddChildEntity(Entity childEntity)
        {
            if (m_ChildEntities == null)
            {
                m_ChildEntities = new List<Entity>();
            }

            if (m_ChildEntities.Contains(childEntity))
            {
                Debug.LogError("要添加的子实体已存在");
            }

            m_ChildEntities.Add(childEntity);
        }

        /// <summary>
        /// 删除子实体
        /// </summary>
        /// <param name="childEntity"></param>
        public void RemoveChildEntity(Entity childEntity)
        {
            if (m_ChildEntities == null || !m_ChildEntities.Remove(childEntity))
            {
                Debug.LogError("删除子实体失败");
            }
        }
    }
}

