using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 显示实体时加载依赖资源事件。
    /// </summary>
    public class ShowEntityDependencyAssetEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 显示实体时加载依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(ShowEntityDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        /// 显示实体时加载依赖资源事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 实体编号
        /// </summary>
        public int EntityId { get; private set; }


        /// <summary>
        /// 实体逻辑类型
        /// </summary>
        public Type EntityLogicType { get; private set; }


        /// <summary>
        /// 实体资源名称
        /// </summary>
        public string EntityAssetName { get; private set; }


        /// <summary>
        /// 实体组名称
        /// </summary>
        public string EntityGroupName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 被加载的依赖资源名称
        /// </summary>
        public string DependencyAssetName { get; private set; }


        /// <summary>
        /// 当前已加载依赖资源数量
        /// </summary>
        public int LoadedCount { get; private set; }


        /// <summary>
        /// 总共加载依赖资源数量
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 清理显示实体时加载依赖资源事件
        /// </summary>
        public override void Clear()
        {
            EntityId = default(int);
            EntityLogicType = default(Type);
            EntityAssetName = default(string);
            EntityGroupName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充显示实体时加载依赖资源事件
        /// </summary>
        /// <returns>显示实体时加载依赖资源事件。</returns>
        public ShowEntityDependencyAssetEventArgs Fill(object userData, int entityId, string entityAssetName, string entityGroupName,string dependencyAssetName, int loadedCount,int totalCount)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            EntityId = entityId;
            EntityLogicType = showEntityInfo.EntityLogicType;
            EntityAssetName = entityAssetName;
            EntityGroupName = entityGroupName;
            UserData = showEntityInfo.UserData;

            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;


            return this;
        }

    }
}

