using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Resource;
namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 默认实体辅助器
    /// </summary>
    public class DefaultEntityHelper : EntityHelperBase
    {
        private ResourceManager m_ResourceManager;

        public DefaultEntityHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entityInstance">实体实例</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>实体</returns>
        public override Entity CreateEntity(object entityInstance, EntityGroup entityGroup, object userData)
        {
            GameObject gameObject = entityInstance as GameObject;
            if (gameObject == null)
            {
                Debug.LogError("实例化的实体为空");
                return null;
            }

            //将创建的实体作为辅助器的子物体
            gameObject.transform.SetParent(entityGroup.Helper.transform);

            //挂载Entity脚本
            Entity entity = gameObject.GetOrAddComponent<Entity>();

            return entity;
        }

        /// <summary>
        /// 实例化实体
        /// </summary>
        /// <param name="entityAsset">要实例化的实体资源</param>
        /// <returns>实例化后的实体</returns>
        public override object InstantiateEntity(object entityAsset)
        {
            return Object.Instantiate((Object)entityAsset);
        }

        /// <summary>
        /// 释放实体
        /// </summary>
        /// <param name="entityAsset">要释放的实体资源</param>
        /// <param name="entityInstance">要释放的实体实例</param>
        public override void ReleaseEntity(object entityAsset, object entityInstance)
        {
            m_ResourceManager.UnloadAsset(entityAsset);
            GameObject.Destroy((Object)entityInstance);
        }
    }
}

