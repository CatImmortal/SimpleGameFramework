using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络消息包处理器基类
    /// </summary>
    public abstract class PacketHandlerBase
    {
        /// <summary>
        /// 网络消息包协议编号
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// 网络消息包处理方法
        /// </summary>
        /// <param name="sender">网络消息包源</param>
        /// <param name="packet">网络消息包内容</param>
        public abstract void Handle(object sender, PacketBase packet);

    }
}

