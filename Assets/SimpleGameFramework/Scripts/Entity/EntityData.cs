using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 实体数据基类
    /// </summary>
    public abstract class EntityData
    {
        /// <summary>
        /// 实体编号
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 实体类型编号（用于从数据表里读取对应数据行）
        /// </summary>
        public int TypeId { get; private set; }
        /// <summary>
        /// 实体位置
        /// </summary>
        public Vector3 Positoin { get; set; }

        /// <summary>
        /// 实体旋转
        /// </summary>
        public Quaternion Rotation { get; set; }

        protected EntityData(int id, int typeId)
        {
            Id = id;
            TypeId = typeId;
        }

    }

}
