using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Entity
{
    /// <summary>
    /// 显示实体的信息
    /// </summary>
    public class ShowEntityInfo
    {
        /// <summary>
        /// 实体逻辑脚本类型
        /// </summary>
        public Type EntityLogicType { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public ShowEntityInfo(Type entityLogicType, object userData)
        {
            EntityLogicType = entityLogicType;
            UserData = userData;
        }
    }
}

