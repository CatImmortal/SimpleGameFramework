using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 用户自定义网络错误事件
    /// </summary>
    public class NetworkCustomErrorEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 用户自定义网络错误事件编号。
        /// </summary>
        public static readonly int EventId = typeof(NetworkCustomErrorEventArgs).GetHashCode();

        /// <summary>
        /// 用户自定义网络错误事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 网络频道。
        /// </summary>
        public NetworkChannel NetworkChannel { get; private set; }


        /// <summary>
        /// 用户自定义错误数据
        /// </summary>
        public object CustomErrorData { get; private set; }


        /// <summary>
        /// 清理用户自定义网络错误事件
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = default(NetworkChannel);
            CustomErrorData = default(object);
        }

        /// <summary>
        /// 填充用户自定义网络错误事件
        /// </summary>
        public NetworkCustomErrorEventArgs Fill(NetworkChannel networkChannel, object customErrorData)
        {
            NetworkChannel = networkChannel;
            CustomErrorData = customErrorData;

            return this;
        }

    }
}

