using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 附加实体的信息
    /// </summary>
    public class AttachEntityInfo
    {
        /// <summary>
        /// 父实体的Transform
        /// </summary>
        public Transform ParentTransform { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public AttachEntityInfo(Transform parentTransform, object userData)
        {
            ParentTransform = parentTransform;
            UserData = userData;
        }
    }
}

