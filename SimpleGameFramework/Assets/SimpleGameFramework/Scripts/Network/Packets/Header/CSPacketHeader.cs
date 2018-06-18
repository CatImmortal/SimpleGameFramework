using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 客户端发往服务器的消息包头
    /// </summary>
    public class CSPacketHeader : PacketHeaderBase
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

