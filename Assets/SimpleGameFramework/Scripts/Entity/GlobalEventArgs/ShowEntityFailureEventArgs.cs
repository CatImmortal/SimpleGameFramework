using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 显示实体失败事件
    /// </summary>
    public class ShowEntityFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 显示实体失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(ShowEntityFailureEventArgs).GetHashCode();

        /// <summary>
        /// 显示实体失败事件编号
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
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理显示实体失败事件。
        /// </summary>
        public override void Clear()
        {
            EntityId = default(int);
            EntityLogicType = default(Type);
            EntityAssetName = default(string);
            EntityGroupName = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充显示实体失败事件
        /// </summary>
        /// <returns>显示实体失败事件</returns>
        public ShowEntityFailureEventArgs Fill(object userData, int entityId, string entityAssetName, string entityGroupName,string errorMessage)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            EntityId = entityId;
            EntityLogicType = showEntityInfo.EntityLogicType;
            EntityAssetName = entityAssetName;
            EntityGroupName = entityGroupName;
            UserData = showEntityInfo.UserData;

            ErrorMessage = errorMessage;

            return this;
        }
    }
}

