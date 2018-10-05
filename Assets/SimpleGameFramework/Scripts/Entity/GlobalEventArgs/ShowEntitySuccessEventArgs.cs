using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 显示实体成功事件
    /// </summary>
    public class ShowEntitySuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 显示实体成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(ShowEntitySuccessEventArgs).GetHashCode();

        /// <summary>
        /// 显示实体成功事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }


        /// <summary>
        /// 实体逻辑类型
        /// </summary>
        public Type EntityLogicType { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 显示成功的实体
        /// </summary>
        public Entity Entity { get; private set; }

        /// <summary>
        /// 获取加载持续时间
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 清理显示实体成功事件
        /// </summary>
        public override void Clear()
        {
            EntityLogicType = default(Type);
            Entity = default(Entity);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充显示实体成功事件
        /// </summary>
        /// <returns>显示实体成功事件</returns>
        public ShowEntitySuccessEventArgs Fill(object userData,Entity entity,float duration)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            EntityLogicType = showEntityInfo.EntityLogicType;
            UserData = showEntityInfo.UserData;

            Entity = entity;
            Duration = duration;

            return this;
        }
    }
}

