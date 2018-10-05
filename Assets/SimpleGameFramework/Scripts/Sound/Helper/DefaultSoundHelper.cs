using SimpleGameFramework.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 默认声音辅助器
    /// </summary>
    public class DefaultSoundHelper : SoundHelperBase
    {
        private ResourceManager m_ResourceManager;

        public DefaultSoundHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
        }

        /// <summary>
        /// 释放声音资源
        /// </summary>
        /// <param name="soundAsset">要释放的声音资源</param>
        public override void ReleaseSoundAsset(object soundAsset)
        {
            m_ResourceManager.UnloadAsset(soundAsset);
        }
    }
}

