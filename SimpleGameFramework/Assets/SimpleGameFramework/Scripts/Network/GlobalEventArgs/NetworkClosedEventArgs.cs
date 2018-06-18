using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络连接关闭事件
    /// </summary>
    public class NetworkClosedEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 连接关闭事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkClosedEventArgs).GetHashCode();

        /// <summary>
        /// 连接关闭事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取网络频道。
        /// </summary>
        public NetworkChannel NetworkChannel { get; private set; }

        /// <summary>
        /// 清理网络连接关闭事件
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = default(NetworkChannel);
        }

        /// <summary>
        /// 填充网络连接关闭事件
        /// </summary>
        public NetworkClosedEventArgs Fill(NetworkChannel networkChannel)
        {
            NetworkChannel = networkChannel;

            return this;
        }
    }
}

