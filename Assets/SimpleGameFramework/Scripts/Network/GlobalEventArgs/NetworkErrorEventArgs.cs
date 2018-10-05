using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络错误事件
    /// </summary>
    public class NetworkErrorEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 连接错误事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkErrorEventArgs).GetHashCode();

        /// <summary>
        /// 连接错误事件编号
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
        /// 获取错误码
        /// </summary>
        public NetworkErrorCode ErrorCode { get; private set; }


        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }


        /// <summary>
        /// 清理网络错误事件
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = default(NetworkChannel);
            ErrorCode = default(NetworkErrorCode);
            ErrorMessage = default(string);
        }

        /// <summary>
        /// 填充网络错误事件
        /// </summary>
        public NetworkErrorEventArgs Fill(NetworkChannel networkChannel,NetworkErrorCode errorCode,string errorMessage)
        {
            NetworkChannel = networkChannel;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;

            return this;
        }

    }
}

