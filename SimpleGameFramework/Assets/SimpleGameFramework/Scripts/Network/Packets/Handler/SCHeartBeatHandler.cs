using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 服务器向客户端发送的心跳包的处理器
    /// </summary>
    public class SCHeartBeatHandler : PacketHandlerBase
    {
        public override int Id
        {
            get
            {
                return 2;
            }
        }

        public override void Handle(object sender, PacketBase packet)
        {
            SCHeartBeat packetImpl = (SCHeartBeat)packet;
            Debug.Log("接收到心跳包：" + packetImpl.Id.ToString());
        }
    }
}

