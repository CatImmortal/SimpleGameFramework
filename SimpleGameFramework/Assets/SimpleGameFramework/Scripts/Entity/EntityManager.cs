using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Event;
using SimpleGameFramework.Resource;
using SimpleGameFramework.ObjectPool;
using System;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 实体信息字典
        /// </summary>
        private Dictionary<int, EntityInfo> m_EntityInfos;

        /// <summary>
        /// 实体组字典
        /// </summary>
        private Dictionary<string, EntityGroup> m_EntityGroups;

        /// <summary>
        /// 正在加载实体字典
        /// </summary>
        private Dictionary<int, int> m_EntitiesBeingLoaded;

        /// <summary>
        /// 需要释放实体哈希集合
        /// </summary>
        private HashSet<int> m_EntitiesToReleaseOnLoad;

        /// <summary>
        /// 回收实体信息链表
        /// </summary>
        private LinkedList<EntityInfo> m_RecycleQueue;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private ObjectPoolManager m_ObjectPoolManager;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 加载资源回调方法集
        /// </summary>
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 实体辅助器
        /// </summary>
        private EntityHelperBase m_EntityHelper;

        /// <summary>
        /// 序列编号
        /// </summary>
        private int m_Serial;

        /// <summary>
        /// 实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_EntityInfos.Count;
            }
        }

        /// <summary>
        /// 实体组数量
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return m_EntityGroups.Count;
            }
        }
        #endregion

        public EntityManager()
        {
            m_EntityInfos = new Dictionary<int, EntityInfo>();
            m_EntityGroups = new Dictionary<string, EntityGroup>();
            m_EntitiesBeingLoaded = new Dictionary<int, int>();
            m_EntitiesToReleaseOnLoad = new HashSet<int>();
            m_RecycleQueue = new LinkedList<EntityInfo>();
           
            m_ObjectPoolManager = FrameworkEntry.Instance.GetManager<ObjectPoolManager>();
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadEntitySuccessCallback, LoadEntityDependencyAssetCallback, LoadEntityFailureCallback, LoadEntityUpdateCallback);
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();

            m_EntityHelper = null;
            m_Serial = 0;

        }

        #region 生命周期
        public override void Init()
        {
            //设置默认实体辅助器
            SetEntityHelper(new DefaultEntityHelper());
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //回收需要回收的实体
            while (m_RecycleQueue.Count > 0)
            {
                EntityInfo entityInfo = m_RecycleQueue.First.Value;
                m_RecycleQueue.RemoveFirst();
                Entity entity = entityInfo.Entity;
                EntityGroup entityGroup = entity.EntityGroup;
                if (entityGroup == null)
                {
                    Debug.LogError("要回收的实体的实体组为空");
                    return;
                }

                entityInfo.Status = EntityStatus.WillRecycle;
                entity.OnRecycle();
                entityInfo.Status = EntityStatus.Recycled;
                entityGroup.UnspawnEntity(entity);
            }

            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                entityGroup.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理实体管理器。
        /// </summary>
        public override void Shutdown()
        {
            HideAllLoadedEntities();
            m_EntityGroups.Clear();
            m_EntitiesBeingLoaded.Clear();
            m_EntitiesToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }
        #endregion

        /// <summary>
        /// 设置实体辅助器
        /// </summary>
        /// <param name="entityHelper">实体辅助器</param>
        public void SetEntityHelper(EntityHelperBase entityHelper)
        {
            if (entityHelper == null)
            {
                Debug.LogError("要设置的实体辅助器为空");
                return;
            }

            m_EntityHelper = entityHelper;
        }

        #region 实体组相关的操作
        /// <summary>
        /// 是否存在实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在实体组</returns>
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                Debug.LogError("要检查是否存在的实体组名称为空");
                return false;
            }

            return m_EntityGroups.ContainsKey(entityGroupName);
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>要获取的实体组</returns>
        public EntityGroup GetEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                Debug.LogError("要获取的实体组名称为空");
            }

            EntityGroup entityGroup = null;
            if (m_EntityGroups.TryGetValue(entityGroupName, out entityGroup))
            {
                return entityGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <returns>所有实体组</returns>
        public EntityGroup[] GetAllEntityGroups()
        {
            int index = 0;
            EntityGroup[] entityGroups = new EntityGroup[m_EntityGroups.Count];
            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                entityGroups[index++] = entityGroup.Value;
            }

            return entityGroups;
        }


        /// <summary>
        /// 增加实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <returns>是否增加实体组成功</returns>
        public bool AddEntityGroup(string entityGroupName, float instanceAutoReleaseInterval = 60f, int instanceCapacity = 16, float instanceExpireTime = 60f)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                Debug.LogError("要增加的实体组名称为空");
                return false;
            }

            if (HasEntityGroup(entityGroupName))
            {
                Debug.LogError("要增加的实体组已存在");
                return false;
            }


            //创建实体组辅助器
            EntityGroupHelperBase entityGroupHelper = new GameObject().AddComponent<DefaultEntityGroupHelper>();
            entityGroupHelper.name = string.Format("Entity Group - {0}", entityGroupName);
            entityGroupHelper.transform.SetParent(FrameworkEntry.Instance.transform);

            //将实体组放入字典
            m_EntityGroups.Add(entityGroupName, new EntityGroup(entityGroupName, instanceAutoReleaseInterval, instanceCapacity, instanceExpireTime, entityGroupHelper, m_ObjectPoolManager));

            return true;
        }
        #endregion

        #region 检查实体
        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            return m_EntityInfos.ContainsKey(entityId);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("要检查是否存在的实体资源名称为空");
                return false;
            }

            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 获取实体
        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>实体信息</returns>
        private EntityInfo GetEntityInfo(int entityId)
        {
            EntityInfo entityInfo = null;
            if (m_EntityInfos.TryGetValue(entityId, out entityInfo))
            {
                return entityInfo;
            }

            return null;
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>要获取的实体。</returns>
        public Entity GetEntity(int entityId)
        {
            EntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                return null;
            }

            return entityInfo.Entity;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public Entity GetEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("要获取实体的资源名称为空");
                return null;
            }

            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    return entityInfo.Value.Entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public Entity[] GetEntities(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("要获取实体的资源名称为空");
                return null;
            }

            List<Entity> entities = new List<Entity>();
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    entities.Add(entityInfo.Value.Entity);
                }
            }

            return entities.ToArray();
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <returns>所有已加载的实体。</returns>
        public Entity[] GetAllLoadedEntities()
        {
            int index = 0;
            Entity[] entities = new Entity[m_EntityInfos.Count];
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                entities[index++] = entityInfo.Value.Entity;
            }

            return entities;
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <returns>所有正在加载实体的编号。</returns>
        public int[] GetAllLoadingEntityIds()
        {
            int index = 0;
            int[] entitiesBeingLoaded = new int[m_EntitiesBeingLoaded.Count];
            foreach (KeyValuePair<int, int> entityBeingLoaded in m_EntitiesBeingLoaded)
            {
                entitiesBeingLoaded[index++] = entityBeingLoaded.Key;
            }

            return entitiesBeingLoaded;
        }

        #endregion

        #region 实体父子关系的操作

        #region 获取

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntityId">要获取父实体的子实体的实体编号</param>
        /// <returns>子实体的父实体</returns>
        public Entity GetParentEntity(int childEntityId)
        {
            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                Debug.LogError("要获取父实体的子实体的实体信息为空：" + childEntityId);
                return null;
            }

            return childEntityInfo.ParentEntity;
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <returns>子实体数组</returns>
        public Entity[] GetChildEntities(int parentEntityId)
        {
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                Debug.LogError("要获取子实体的父实体的实体信息为空：" + parentEntityId);
                return null;
            }

            return parentEntityInfo.GetChildEntities();
        }


        #region 附加与接触
        /// <summary>
        /// 解除父实体
        /// </summary>
        /// <param name="childEntityId">要解除父实体的子实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(int childEntityId, object userData = null)
        {
            //获取子实体信息
            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                Debug.LogError("要解除的子实体的实体信息为空：" + childEntityId);
                return; 
            }

            //获取父实体信息
            Entity parentEntity = childEntityInfo.ParentEntity;
            if (parentEntity == null)
            {
                return;
            }
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntity.Id);
            if (parentEntityInfo == null)
            {
                Debug.LogError("被解除的父实体的实体信息为空：" + parentEntity.Id);
            }

            //子实体解除父实体
            Entity childEntity = childEntityInfo.Entity;
            childEntityInfo.ParentEntity = null;
            childEntity.OnDetachFrom(parentEntity, userData);

            //父实体解除子实体
            parentEntityInfo.RemoveChildEntity(childEntity);
            parentEntity.OnDetached(childEntity, userData);

           
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(int parentEntityId, object userData)
        {
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                Debug.LogError("要接触所有子实体的父实体的实体信息为空：" + parentEntityId);
                return;
            }

            Entity[] childEntities = parentEntityInfo.GetChildEntities();
            foreach (Entity childEntity in childEntities)
            {
                DetachEntity(childEntity.Id, userData);
            }
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, object userData = null)
        {
            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                Debug.LogError("要附加的子实体的实体信息为空：" + childEntityId);
                return;
            }
            if (childEntityInfo.Status >= EntityStatus.WillHide)
            {
                Debug.LogError("要附加的子实体状态不合法，无法附加：" + childEntityInfo.Status.ToString());
                return;
            }

            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                Debug.LogError("被附加的父实体的实体信息为空：" + childEntityId);
                return;
            }
            if (parentEntityInfo.Status >= EntityStatus.WillHide)
            {
                Debug.LogError("被附加的父实体状态不合法，无法附加：" + parentEntityInfo.Status.ToString());
                return;
            }

            Entity childEntity = childEntityInfo.Entity;
            Entity parentEntity = parentEntityInfo.Entity;

            //解除原来的父实体
            DetachEntity(childEntity.Id, userData);

            //附加新的父实体
            childEntityInfo.ParentEntity = parentEntity;
            childEntity.OnAttachTo(parentEntity, userData);

            //附加子实体
            parentEntityInfo.AddChildEntity(childEntity);
            parentEntity.OnAttached(childEntity, userData);
           
        }
        #endregion

        #endregion


        #endregion

        #region 显示实体
        /// <summary>
        /// 是否正在加载实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否正在加载实体</returns>
        public bool IsLoadingEntity(int entityId)
        {
            return m_EntitiesBeingLoaded.ContainsKey(entityId);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityLogicType">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, object userData)
        {
            if (entityLogicType == null)
            {
                Debug.LogError("显示实体时的实体逻辑类型为空");
                return;
            }
            ShowEntityInfo info = new ShowEntityInfo(entityLogicType, userData);

            if (m_ResourceManager == null)
            {
                Debug.LogError("显示实体时的资源管理器为空");
                return;
            }

            if (m_EntityHelper == null)
            {
                Debug.LogError("显示实体时的辅助器为空");
                return;
            }

            if (string.IsNullOrEmpty(entityAssetName))
            {
                Debug.LogError("显示实体时的实体资源名称为空");
                return;
            }

            if (string.IsNullOrEmpty(entityGroupName))
            {
                Debug.LogError("显示实体时的实体组名称为空");
                return;
            }

            if (m_EntityInfos.ContainsKey(entityId))
            {
                Debug.LogError("显示实体时的实体信息已存在");
                return;
            }

            if (IsLoadingEntity(entityId))
            {
                Debug.LogError("要显示的实体已在加载");
                return;
            }

            //实体组检查
            EntityGroup entityGroup = GetEntityGroup(entityGroupName);
            if (entityGroup == null)
            {
                Debug.LogError("显示实体时的实体组不存在");
                return;
            }

            //尝试从对象池获取实体实例
            EntityInstanceObject entityInstanceObject = entityGroup.SpawnEntityInstanceObject(entityAssetName);
            if (entityInstanceObject == null)
            {
                //没获取到就加载该实体
                int serialId = m_Serial++;
                m_EntitiesBeingLoaded.Add(entityId, serialId);
                m_ResourceManager.LoadAsset(entityAssetName, m_LoadAssetCallbacks, new LoadEntityInfo(serialId, entityId, entityGroup, info));
                return;
            }

            ShowEntity(entityId, entityAssetName, entityGroup, entityInstanceObject.Target, false, 0f, info);
        }


        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroup">实体组</param>
        /// <param name="entityInstance">实体实例</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="duration">加载持续时间</param>
        /// <param name="userData">用户自定义数据</param>
        private void ShowEntity(int entityId, string entityAssetName, EntityGroup entityGroup, object entityInstance, bool isNewInstance, float duration, object userData)
        {
            try
            {
                //使用辅助器创建实体
                Entity entity = m_EntityHelper.CreateEntity(entityInstance, entityGroup, userData);
                if (entity == null)
                {
                    throw new Exception("使用辅助器创建实体失败");
                }
                //创建实体信息对象
                EntityInfo entityInfo = new EntityInfo(entity);
                m_EntityInfos.Add(entityId, entityInfo);

                entityInfo.Status = EntityStatus.WillInit;
                entity.OnInit(entityId, entityAssetName, entityGroup, isNewInstance, userData);
                entityInfo.Status = EntityStatus.Inited;

                entityGroup.AddEntity(entity);

                entityInfo.Status = EntityStatus.WillShow;
                entity.OnShow(userData);
                entityInfo.Status = EntityStatus.Showed;

                //派发显示实体成功事件
                ShowEntitySuccessEventArgs se = new ShowEntitySuccessEventArgs();
                m_EventManager.Fire(this, se.Fill(userData, entity, duration));
            }
            catch (Exception exception)
            {
                //派发显示实体失败事件
                ShowEntityFailureEventArgs fe = new ShowEntityFailureEventArgs();
                m_EventManager.Fire(this, fe.Fill(userData, entityId, entityAssetName, entityGroup.Name, exception.Message));
            }
        }
        #endregion

        #region 隐藏实体
        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(int entityId, object userData = null)
        {
            //要隐藏的实体正在加载，就直接释放
            if (IsLoadingEntity(entityId))
            {
                int serialId = 0;
                if (!m_EntitiesBeingLoaded.TryGetValue(entityId, out serialId))
                {
                    Debug.LogError("没找到实体；" + entityId);
                }
                m_EntitiesToReleaseOnLoad.Add(serialId);
                m_EntitiesBeingLoaded.Remove(entityId);
                return;
            }

            //获取到实体信息
            EntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                Debug.LogError("获取要隐藏的实体的实体信息为空：" + entityId);
                return;
            }

            //获取实体与它的子实体
            Entity entity = entityInfo.Entity;
            Entity[] childEntities = entityInfo.GetChildEntities();
            foreach (Entity childEntity in childEntities)
            {
                //递归隐藏子实体
                HideEntity(childEntity.Id, userData);
            }

            //解除自身与父实体的关系
            DetachEntity(entity.Id, userData);

            //隐藏实体
            entityInfo.Status = EntityStatus.WillHide;
            entity.OnHide(userData);
            entityInfo.Status = EntityStatus.Hidden;

            //将隐藏的实体从实体组与实体信息字典中移除
            EntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Debug.LogError("隐藏的实体的实体组为空");
            }
            entityGroup.RemoveEntity(entity);
            if (!m_EntityInfos.Remove(entity.Id))
            {
                Debug.LogError("将隐藏的实体从实体信息字典中移除失败");
            }

            //派发隐藏实体完成事件
            HideEntityCompleteEventArgs e = ReferencePool.Acquire<HideEntityCompleteEventArgs>();
            m_EventManager.Fire(this, e.Fill(entity.Id, entity.EntityAssetName, userData, entityGroup));

            //将隐藏的实体加入回收队列
            m_RecycleQueue.AddLast(entityInfo);
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void HideAllLoadedEntities(object userData = null)
        {
            while (m_EntityInfos.Count > 0)
            {
                foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
                {
                    HideEntity(entityInfo.Value.Entity.Id, userData);
                    break;
                }
            }
        }

        /// <summary>
        /// 隐藏所有正在加载的实体
        /// </summary>
        public void HideAllLoadingEntities()
        {
            foreach (KeyValuePair<int, int> entityBeingLoaded in m_EntitiesBeingLoaded)
            {
                m_EntitiesToReleaseOnLoad.Add(entityBeingLoaded.Value);
            }

            m_EntitiesBeingLoaded.Clear();
        }
        #endregion

        #region 加载资源的4个回调方法
        private void LoadEntitySuccessCallback(string entityAssetName, object entityAsset, float duration, object userData)
        {
            //获取记载实体的信息
            LoadEntityInfo loadEntityInfo = (LoadEntityInfo)userData;
            if (loadEntityInfo == null)
            {
                Debug.LogError("加载实体的信息为空");
                return;
            }
            m_EntitiesBeingLoaded.Remove(loadEntityInfo.EntityId);

            if (m_EntitiesToReleaseOnLoad.Contains(loadEntityInfo.SerialId))
            {
                Debug.LogError(string.Format("需要释放的实体：{0}(id：{1})加载成功", loadEntityInfo.EntityId.ToString(), loadEntityInfo.SerialId.ToString()));
                m_EntitiesToReleaseOnLoad.Remove(loadEntityInfo.SerialId);
                m_EntityHelper.ReleaseEntity(entityAsset, null);
                return;
            }

            //实例化实体，并将实体实例对象放入对象池
            EntityInstanceObject entityInstanceObject = new EntityInstanceObject(entityAssetName, entityAsset, m_EntityHelper.InstantiateEntity(entityAsset), m_EntityHelper);
            loadEntityInfo.EntityGroup.RegisterEntityInstanceObject(entityInstanceObject, true);

            //显示实体
            ShowEntity(loadEntityInfo.EntityId, entityAssetName, loadEntityInfo.EntityGroup, entityInstanceObject.Target, true, duration, loadEntityInfo.UserData);
        }

        private void LoadEntityFailureCallback(string entityAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            LoadEntityInfo loadEntityInfo = (LoadEntityInfo)userData;
            if (loadEntityInfo == null)
            {
                Debug.LogError("加载实体的信息为空");
                return;
            }
            m_EntitiesBeingLoaded.Remove(loadEntityInfo.EntityId);
            m_EntitiesToReleaseOnLoad.Remove(loadEntityInfo.SerialId);

            string message = string.Format("加载实体：{0} 失败，错误信息：{1}", entityAssetName, errorMessage);

            //派发显示实体失败事件
            ShowEntityFailureEventArgs e = ReferencePool.Acquire<ShowEntityFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadEntityInfo.UserData, loadEntityInfo.EntityId, entityAssetName, loadEntityInfo.EntityGroup.Name, message));

        }

        private void LoadEntityUpdateCallback(string entityAssetName, float progress, object userData)
        {
            LoadEntityInfo loadEntityInfo = (LoadEntityInfo)userData;
            if (loadEntityInfo == null)
            {
                Debug.LogError("加载实体的信息为空");
                return;
            }

            //派发显示实体更新事件
            ShowEntityUpdateEventArgs e = ReferencePool.Acquire<ShowEntityUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadEntityInfo.UserData, loadEntityInfo.EntityId, entityAssetName, loadEntityInfo.EntityGroup.Name, progress));
        }

        private void LoadEntityDependencyAssetCallback(string entityAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            LoadEntityInfo loadEntityInfo = (LoadEntityInfo)userData;
            if (loadEntityInfo == null)
            {
                Debug.LogError("加载实体的信息为空");
                return;
            }

            //派发显示实体时加载依赖资源事件
            ShowEntityDependencyAssetEventArgs e = ReferencePool.Acquire<ShowEntityDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadEntityInfo.UserData, loadEntityInfo.EntityId, entityAssetName, loadEntityInfo.EntityGroup.Name, dependencyAssetName, loadedCount, totalCount));
        }

       
        #endregion
    }
}

