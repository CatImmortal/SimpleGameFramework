using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 加载实体的信息
    /// </summary>
    public class LoadEntityInfo
    {
       
        /// <summary>
        /// 序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 实体编号
        /// </summary>
        public int EntityId { get; private set; }

        /// <summary>
        /// 实体组
        /// </summary>
        public EntityGroup EntityGroup { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public LoadEntityInfo(int serialId, int entityId, EntityGroup entityGroup, object userData)
        {
            SerialId = serialId;
            EntityId = entityId;
            EntityGroup = entityGroup;
            UserData = userData;
        }
    }
}

