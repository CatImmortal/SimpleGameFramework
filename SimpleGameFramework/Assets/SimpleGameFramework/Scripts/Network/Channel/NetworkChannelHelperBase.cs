using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络频道辅助器基类
    /// </summary>
    public abstract class NetworkChannelHelperBase
    {
        /// <summary>
        /// 消息包头长度
        /// </summary>
        public abstract int PacketHeaderLength { get; }

        /// <summary>
        /// 初始化网络频道辅助器
        /// </summary>
        /// <param name="networkChannel">网络频道</param>
        public abstract void Initialize(NetworkChannel networkChannel);

        /// <summary>
        /// 关闭并清理网络频道辅助器
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// 发送心跳消息包
        /// </summary>
        /// <returns>是否发送心跳消息包成功</returns>
        public abstract bool SendHeartBeat();

        /// <summary>
        /// 序列化消息包
        /// </summary>
        /// <typeparam name="T">消息包类型</typeparam>
        /// <param name="packet">要序列化的消息包</param>
        /// <returns>序列化后的消息包字节数组</returns>
        public abstract byte[] Serialize<T>(T packet) where T : PacketBase;

        /// <summary>
        /// 反序列化消息包头
        /// </summary>
        /// <param name="source">要反序列化的来源流</param>
        /// <param name="customErrorData">用户自定义错误数据</param>
        /// <returns></returns>
        public abstract PacketHeaderBase DeserializePacketHeader(Stream source, out object customErrorData);

        /// <summary>
        /// 反序列化消息包
        /// </summary>
        /// <param name="packetHeader">消息包头</param>
        /// <param name="source">要反序列化的来源流</param>
        /// <param name="customErrorData">用户自定义错误数据</param>
        /// <returns>反序列化后的消息包</returns>
        public abstract PacketBase DeserializePacket(PacketHeaderBase packetHeader, Stream source, out object customErrorData);
    }
}

