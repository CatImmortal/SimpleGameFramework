using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络类型
    /// </summary>
    public enum NetworkType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// IP 版本 4
        /// </summary>
        IPv4,

        /// <summary>
        /// IP 版本 6
        /// </summary>
        IPv6,
    }
}

