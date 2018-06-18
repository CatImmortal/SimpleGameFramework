using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 客户端发往服务器的心跳包
    /// </summary>
    public class CSHeartBeat : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 1;
            }
        }

        public override void Clear()
        {
            
        }
    }
}

