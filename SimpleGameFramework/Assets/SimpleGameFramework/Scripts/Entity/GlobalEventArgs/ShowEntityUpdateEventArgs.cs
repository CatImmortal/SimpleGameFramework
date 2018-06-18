using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 显示实体更新事件
    /// </summary>
    public class ShowEntityUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 显示实体更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(ShowEntityUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 显示实体更新事件编号
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
        /// 获取显示实体进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 清理显示实体更新事件
        /// </summary>
        public override void Clear()
        {
            EntityId = default(int);
            EntityLogicType = default(Type);
            EntityAssetName = default(string);
            EntityGroupName = default(string);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充显示实体更新事件
        /// </summary>
        /// <returns>显示实体更新事件</returns>
        public ShowEntityUpdateEventArgs Fill(object userData, int entityId, string entityAssetName, string entityGroupName,float progress)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            EntityId = entityId;
            EntityLogicType = showEntityInfo.EntityLogicType;
            EntityAssetName = entityAssetName;
            EntityGroupName = entityGroupName;
            UserData = showEntityInfo.UserData;

            Progress = progress;
            return this;
        }
    }
}

