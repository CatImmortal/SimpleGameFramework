using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 服务器发往客户端的网络消息包的基类
    /// </summary>
    public abstract class SCPacketBase : PacketBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }

      
    }
}

