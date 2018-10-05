using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 声音辅助器基类
    /// </summary>
    public abstract class SoundHelperBase
    {
        /// <summary>
        /// 释放声音资源
        /// </summary>
        /// <param name="soundAsset">要释放的声音资源</param>
        public abstract void ReleaseSoundAsset(object soundAsset);
    }
}

