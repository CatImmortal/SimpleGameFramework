using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络连接成功事件
    /// </summary>
    public class NetworkConnectedEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 连接成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkConnectedEventArgs).GetHashCode();

        /// <summary>
        /// 获取连接成功事件编号
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
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        /// <summary>
        /// 清理网络连接成功事件
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = default(NetworkChannel);
            UserData = default(object);
        }

        /// <summary>
        /// 填充网络连接成功事件
        /// </summary>
        public NetworkConnectedEventArgs Fill(NetworkChannel networkChannel,object userData)
        {
            NetworkChannel = networkChannel;
            UserData = userData;

            return this;
        }

    }
}

