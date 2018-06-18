using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体
    /// </summary>
    public class Entity : MonoBehaviour
    {
        #region 字段与属性
        /// <summary>
        /// 实体编号
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 实体资源名称
        /// </summary>
        public string EntityAssetName { get; private set; }

        /// <summary>
        /// 获取实体实例
        /// </summary>
        public object Handle
        {
            get
            {
                return gameObject;
            }
        }

        /// <summary>
        /// 实体所属的实体组。
        /// </summary>
        public EntityGroup EntityGroup { get; private set; }

        /// <summary>
        /// 实体逻辑
        /// </summary>
        public EntityLogic Logic { get; private set; }
        #endregion



        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int entityId, string entityAssetName, EntityGroup entityGroup, bool isNewInstance, object userData)
        {
            Id = entityId;
            EntityAssetName = entityAssetName;

            if (isNewInstance)
            {
                EntityGroup = entityGroup;
            }
            else if (EntityGroup != entityGroup)
            {
                Debug.LogError("非新实例对象的实体初始化时实体组不一致");
                return;
            }
            //获取实体信息
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            //根据信息获取实体逻辑类型
            Type entityLogicType = showEntityInfo.EntityLogicType;
            if (entityLogicType == null)
            {
                Debug.LogError("实体初始化时实体逻辑类型为空");
                return;
            }

            if (Logic != null)
            {
                //已有实体逻辑脚本类型与实体信息中的类型相同时就直接启用
                if (Logic.GetType() == entityLogicType)
                {
                    Logic.enabled = true;
                    return;
                }

                //否则销毁掉
                Destroy(Logic);
                Logic = null;
            }

            //将实体逻辑处理脚本挂到游戏物体上
            Logic = gameObject.AddComponent(entityLogicType) as EntityLogic;
            if (Logic == null)
            {
                Debug.LogError("实体逻辑脚本挂载到游戏物体上失败");
                return;
            }
            //调用它的初始化方法
            Logic.OnInit(showEntityInfo.UserData);
        }

        #region 实体逻辑代理
        /// <summary>
        /// 实体回收
        /// </summary>
        public void OnRecycle()
        {
            Id = 0;
            Logic.enabled = false;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnShow(object userData)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            Logic.OnShow(showEntityInfo.UserData);
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        public void OnHide(object userData)
        {
            Logic.OnHide(userData);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnAttached(Entity childEntity, object userData)
        {
            AttachEntityInfo attachEntityInfo = (AttachEntityInfo)userData;
            Logic.OnAttached(childEntity.Logic, attachEntityInfo.ParentTransform, attachEntityInfo.UserData);
        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnDetached(Entity childEntity, object userData)
        {
            Logic.OnDetached(childEntity.Logic, userData);
        }

        /// <summary>
        /// 实体附加父实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnAttachTo(Entity parentEntity, object userData)
        {
            AttachEntityInfo attachEntityInfo = (AttachEntityInfo)userData;
            Logic.OnAttachTo(parentEntity.Logic, attachEntityInfo.ParentTransform, attachEntityInfo.UserData);
        }

        /// <summary>
        /// 实体解除父实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnDetachFrom(Entity parentEntity, object userData)
        {
            Logic.OnDetachFrom(parentEntity.Logic, userData);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Logic.OnUpdate(elapseSeconds, realElapseSeconds);
        }
        #endregion
    }
}

