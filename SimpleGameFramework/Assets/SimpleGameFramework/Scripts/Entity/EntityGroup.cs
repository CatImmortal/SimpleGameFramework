using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.ObjectPool;
namespace SimpleGameFramework.Entity
{
    public class EntityGroup
    {
        #region 字段与属性
        /// <summary>
        /// 实体组名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 实体组辅助器
        /// </summary>
        public EntityGroupHelperBase Helper { get; private set; }

        /// <summary>
        /// 实体实例对象池
        /// </summary>
        private ObjectPool<EntityInstanceObject> m_InstancePool;

        /// <summary>
        /// 实体组的实体
        /// </summary>
        private LinkedList<Entity> m_Entities;


        /// <summary>
        /// 获取实体组中实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_Entities.Count;
            }
        }

        /// <summary>
        /// 实体组实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get
            {
                return m_InstancePool.AutoReleaseInterval;
            }
            set
            {
                m_InstancePool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 实体组实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get
            {
                return m_InstancePool.Capacity;
            }
            set
            {
                m_InstancePool.Capacity = value;
            }
        }

        /// <summary>
        /// 获取或设置实体组实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get
            {
                return m_InstancePool.ExpireTime;
            }
            set
            {
                m_InstancePool.ExpireTime = value;
            }
        }

        #endregion
       
        /// <summary>
        /// 初始化实体组的新实例
        /// </summary>
        /// <param name="name">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <param name="entityGroupHelper">实体组辅助器</param>
        /// <param name="objectPoolManager">对象池管理器</param>
        public EntityGroup(string name, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, EntityGroupHelperBase entityGroupHelper, ObjectPoolManager objectPoolManager)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("用来构造实体组对象的名称为空");
            }

            if (entityGroupHelper == null)
            {
                Debug.LogError("用来构造实体组对象的实体组辅助器为空");
            }

            Name = name;
            Helper = entityGroupHelper;
            m_InstancePool = objectPoolManager.CreateObjectPool<EntityInstanceObject>( instanceCapacity, instanceExpireTime);
            m_InstancePool.AutoReleaseInterval = instanceAutoReleaseInterval;
            m_Entities = new LinkedList<Entity>();
        }

        /// <summary>
        /// 实体组轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<Entity> current = m_Entities.First;
            while (current != null)
            {
                LinkedListNode<Entity> next = current.Next;
                current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                current = next;
            }
        }

        #region 检查实体
        /// <summary>
        /// 实体组中是否存在实体
        /// </summary>
        /// <param name="entityId">实体序列编号</param>
        /// <returns>实体组中是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            foreach (Entity entity in m_Entities)
            {
                if (entity.Id == entityId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 实体组中是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>实体组中是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("实体资源名称为空，无法检查实体是否存在");
                return false;
            }

            foreach (Entity entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 获取实体

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityId">实体序列编号</param>
        /// <returns>要获取的实体</returns>
        public Entity GetEntity(int entityId)
        {
            foreach (Entity entity in m_Entities)
            {
                if (entity.Id == entityId)
                {
                    return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public Entity GetEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("实体资源名称为空，无法获取实体");
                return null;
            }

            foreach (Entity entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 从实体组中获取实体。
        /// </summary>
        /// <param name="entityAssetName">实体资源名称。</param>
        /// <returns>要获取的实体。</returns>
        public Entity[] GetEntities(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                if (string.IsNullOrEmpty(entityAssetName))
                {
                    Debug.LogError("实体资源名称为空，无法获取实体");
                    return null;
                }
            }

            List<Entity> entities = new List<Entity>();
            foreach (Entity entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    entities.Add(entity);
                }
            }

            return entities.ToArray();
        }

        /// <summary>
        /// 从实体组中获取所有实体
        /// </summary>
        /// <returns>实体组中的所有实体</returns>
        public Entity[] GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            foreach (Entity entity in m_Entities)
            {
                entities.Add(entity);
            }

            return entities.ToArray();
        }

        #endregion

        #region 增加与移除实体

        /// <summary>
        /// 往实体组增加实体
        /// </summary>
        /// <param name="entity">要增加的实体</param>
        public void AddEntity(Entity entity)
        {
            m_Entities.AddLast(entity);
        }

        /// <summary>
        /// 从实体组移除实体
        /// </summary>
        /// <param name="entity">要移除的实体</param>
        public void RemoveEntity(Entity entity)
        {
            m_Entities.Remove(entity);
        }
        #endregion

        #region 实体实例对象池的操作

        /// <summary>
        /// 注册实体实例对象到对象池
        /// </summary>
        public void RegisterEntityInstanceObject(EntityInstanceObject obj, bool spawned = false)
        {
            m_InstancePool.Register(obj, spawned);
        }

        /// <summary>
        /// 从对象池获取实体实例对象
        /// </summary>
        public EntityInstanceObject SpawnEntityInstanceObject(string name = "")
        {
            return m_InstancePool.Spawn(name);
        }
        
        /// <summary>
        /// 回收实体实例对象到对象池
        /// </summary>
        public void UnspawnEntity(Entity entity)
        {
            m_InstancePool.Unspawn(entity.Handle);
        }
        #endregion
    }
}

