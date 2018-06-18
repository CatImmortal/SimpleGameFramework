using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 声音组辅助器基类
    /// </summary>
    public class SoundGroupHelperBase : MonoBehaviour
    {
        /// <summary>
        /// 获取或设置声音组辅助器所在的混音组
        /// </summary>
        public AudioMixerGroup AudioMixerGroup { get; set; }
    }
}

