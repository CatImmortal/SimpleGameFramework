using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 隐藏实体完成事件
    /// </summary>
    public class HideEntityCompleteEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 隐藏实体完成事件编号
        /// </summary>
        public static readonly int EventId = typeof(HideEntityCompleteEventArgs).GetHashCode();

        /// <summary>
        /// 隐藏实体完成事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 实体编号。
        /// </summary>
        public int EntityId { get; private set; }


        /// <summary>
        /// 实体资源名称
        /// </summary>
        public string EntityAssetName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 实体所属的实体组
        /// </summary>
        public EntityGroup EntityGroup { get; private set; }

        /// <summary>
        /// 清理隐藏实体完成事件。
        /// </summary>
        public override void Clear()
        {
            EntityId = default(int);
            EntityAssetName = default(string);
            EntityGroup = default(EntityGroup);
            UserData = default(object);
        }

        /// <summary>
        /// 填充隐藏实体完成事件
        /// </summary>
        /// <returns>隐藏实体完成事件</returns>
        public HideEntityCompleteEventArgs Fill(int entityId,string entityAssetName,object userData,EntityGroup entityGroup)
        {
            EntityId = entityId;
            EntityAssetName = entityAssetName;
            UserData = userData;

            EntityGroup = entityGroup;

            return this;
        }
    }
}

