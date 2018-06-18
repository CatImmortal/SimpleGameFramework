using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络消息包头基类
    /// </summary>
    public abstract class PacketHeaderBase : IReference
    {
        /// <summary>
        /// 网络消息包类型
        /// </summary>
        public abstract PacketType PacketType { get; }

        /// <summary>
        ///  网络消息包编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 网络消息包长度
        /// </summary>
        public int PacketLength { get; set; }

        /// <summary>
        ///  网络消息包是否合法
        /// </summary>
        public bool IsValid
        {
            get
            {
                return PacketType != PacketType.Undefined && Id > 0 && PacketLength >= 0;
            }
        }

        /// <summary>
        /// 清理网络消息包头信息
        /// </summary>
        public void Clear()
        {
            Id = 0;
            PacketLength = 0;
        }

    }
}

