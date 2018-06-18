using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络心跳包丢失事件
    /// </summary>
    public class NetworkMissHeartBeatEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 心跳包丢失事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkMissHeartBeatEventArgs).GetHashCode();

        /// <summary>
        /// 心跳包丢失事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取网络频道
        /// </summary>
        public NetworkChannel NetworkChannel { get; private set; }


        /// <summary>
        /// 心跳包已丢失次数
        /// </summary>
        public int MissCount { get; private set; }


        /// <summary>
        /// 清理网络心跳包丢失事件
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = default(NetworkChannel);
            MissCount = default(int);
        }

        /// <summary>
        /// 填充网络心跳包丢失事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>网络心跳包丢失事件。</returns>
        public NetworkMissHeartBeatEventArgs Fill(NetworkChannel networkChannel,int missCount)
        {
            NetworkChannel = networkChannel;
            MissCount = missCount;

            return this;
        }
    }
}

