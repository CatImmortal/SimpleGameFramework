using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public class ConnectState
    {
        /// <summary>
        /// 套接字
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public ConnectState(Socket socket, object userData)
        {
            Socket = socket;
            UserData = userData;
        }
    }

    /// <summary>
    /// 心跳包状态
    /// </summary>
    public class HeartBeatState
    {
       
        /// <summary>
        /// 心跳包流逝时间
        /// </summary>
        public float HeartBeatElapseSeconds { get; set; }

        /// <summary>
        /// 丢失的心跳包数量
        /// </summary>
        public int MissHeartBeatCount { get; set; }

        public HeartBeatState()
        {
            HeartBeatElapseSeconds = 0f;
            MissHeartBeatCount = 0;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="resetHeartBeatElapseSeconds">是否重置心跳包流逝时间</param>
        public void Reset(bool resetHeartBeatElapseSeconds)
        {
            if (resetHeartBeatElapseSeconds)
            {
                HeartBeatElapseSeconds = 0f;
            }

            MissHeartBeatCount = 0;
        }
    }

    /// <summary>
    /// 接收状态
    /// </summary>
    public class ReceiveState
    {
        /// <summary>
        /// 默认接收缓冲区大小
        /// </summary>
        private const int DefaultBufferLength = 1024 * 8;

        /// <summary>
        /// 内存流
        /// </summary>
        public MemoryStream Stream { get; private set; }

        /// <summary>
        /// 网络消息包头
        /// </summary>
        public PacketHeaderBase PacketHeader { get; private set; }

        public ReceiveState()
        {
            Stream = new MemoryStream(DefaultBufferLength);
            PacketHeader = null;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="targetLength">内存流的目标长度</param>
        /// <param name="packetHeader">网络消息包头</param>
        private void Reset(int targetLength, PacketHeaderBase packetHeader)
        {
            if (targetLength < 0)
            {
                Debug.LogError("内存流的目标长度小于0");
            }

            Stream.Position = 0L;
            Stream.SetLength(targetLength);
            PacketHeader = packetHeader;
        }

        /// <summary>
        /// 为接收网络消息包头作准备
        /// </summary>
        public void PrepareForPacketHeader(int packetHeaderLength)
        {
            Reset(packetHeaderLength, null);
        }

        /// <summary>
        /// 为接收网络消息包作准备
        /// </summary>
        public void PrepareForPacket(PacketHeaderBase packetHeader)
        {
            if (packetHeader == null)
            {
                Debug.LogError("网络消息包头为空");
            }

            Reset(packetHeader.PacketLength, packetHeader);
        }
    }

    /// <summary>
    /// 发送状态
    /// </summary>
    public class SendState
    {
        /// <summary>
        /// 要发送的网络消息包字节数组
        /// </summary>
        private byte[] m_PacketBytes;

        /// <summary>
        /// 偏移
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// 是否空闲
        /// </summary>
        public bool IsFree
        {
            get
            {
                return m_PacketBytes == null && Offset == 0 && Length == 0;
            }
        }

        public SendState()
        {
            m_PacketBytes = null;
            Offset = 0;
            Length = 0;
        }

        /// <summary>
        /// 获取要发送的网络消息包字节数组
        /// </summary>
        public byte[] GetPacketBytes()
        {
            return m_PacketBytes;
        }

        /// <summary>
        /// 设置网络消息包
        /// </summary>
        /// <param name="packetBytes">网络消息包字节数组</param>
        public void SetPacket(byte[] packetBytes)
        {
            m_PacketBytes = packetBytes;
            Offset = 0;
            Length = packetBytes.Length;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            m_PacketBytes = null;
            Offset = 0;
            Length = 0;
        }
    }
}

