using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 播放声音错误码
    /// </summary>
    public enum PlaySoundErrorCode
    {
        /// <summary>
        /// 声音组不存在
        /// </summary>
        SoundGroupNotExist,

        /// <summary>
        /// 声音组没有声音代理
        /// </summary>
        SoundGroupHasNoAgent,

        /// <summary>
        /// 加载资源失败
        /// </summary>
        LoadAssetFailure,

        /// <summary>
        /// 播放声音因优先级低被忽略
        /// </summary>
        IgnoredDueToLowPriority,

        /// <summary>
        /// 设置声音资源失败
        /// </summary>
        SetSoundAssetFailure,
    }
}

