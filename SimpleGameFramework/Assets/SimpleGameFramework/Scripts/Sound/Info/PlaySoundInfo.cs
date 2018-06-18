using System.Collections;
using System.Collections.Generic;
using SimpleGameFramework.Entity;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音的信息
    /// </summary>
    public class PlaySoundInfo
    {
      
        /// <summary>
        /// 声音绑定的实体
        /// </summary>
        public Entity.Entity BindingEntity { get; private set; }

        /// <summary>
        /// 声音的世界位置
        /// </summary>
        public Vector3 WorldPosition { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public PlaySoundInfo(Entity.Entity bindingEntity, Vector3 worldPosition, object userData)
        {
            BindingEntity = bindingEntity;
            WorldPosition = worldPosition;
            UserData = userData;
        }
    }
}

