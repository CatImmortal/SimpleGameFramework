using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络频道
    /// </summary>
    public class NetworkChannel : IDisposable
    {
        #region 字段与属性
        /// <summary>
        /// 默认心跳包间隔
        /// </summary>
        private const float DefaultHeartBeatInterval = 30f;

        /// <summary>
        /// 网络频道名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 网络类型
        /// </summary>
        public NetworkType NetworkType { get; private set; }

        /// <summary>
        /// 要发送的消息包队列
        /// </summary>
        private Queue<PacketBase> m_SendPacketPool;

        /// <summary>
        /// 接收到的消息包的池
        /// </summary>
        private EventPool<PacketBase> m_ReceivePacketPool;

        /// <summary>
        /// 网络频道辅助器基类
        /// </summary>
        private NetworkChannelHelperBase m_NetworkChannelHelper;

        /// <summary>
        /// 接收到消息包时是否重置心跳包流逝时间
        /// </summary>
        public bool ResetHeartBeatElapseSecondsWhenReceivePacket { get; set; }

        /// <summary>
        /// 心跳包间隔
        /// </summary>
        public float HeartBeatInterval { get; set; }

        /// <summary>
        /// 套接字
        /// </summary>
        private Socket m_Socket;

        /// <summary>
        /// 发送状态
        /// </summary>
        private SendState m_SendState;

        /// <summary>
        /// 接收状态
        /// </summary>
        private ReceiveState m_ReceiveState;

        /// <summary>
        /// 心跳包状态
        /// </summary>
        private HeartBeatState m_HeartBeatState;

        /// <summary>
        /// 是否激活
        /// </summary>
        private bool m_Active;

        /// <summary>
        /// 是否释放过
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        //向网络管理器通知网络事件的委托
        public UnityAction<NetworkChannel, object> NetworkChannelConnected;
        public UnityAction<NetworkChannel> NetworkChannelClosed;
        public UnityAction<NetworkChannel, int> NetworkChannelMissHeartBeat;
        public UnityAction<NetworkChannel, NetworkErrorCode, string> NetworkChannelError;
        public UnityAction<NetworkChannel, object> NetworkChannelCustomError;

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool Connected
        {
            get
            {
                if (m_Socket != null)
                {
                    return m_Socket.Connected;
                }

                return false;
            }
        }

        /// <summary>
        /// 本地终结点的 IP 地址
        /// </summary>
        public IPAddress LocalIPAddress
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket未连接，无法获取本地终结点的IP");
                    return null;
                }

                IPEndPoint ipEndPoint = (IPEndPoint)m_Socket.LocalEndPoint;
                if (ipEndPoint == null)
                {
                    Debug.LogError("本地终结点为空，无法获取IP");
                    return null;
                }

                return ipEndPoint.Address;
            }
        }

        /// <summary>
        /// 本地终结点的端口号
        /// </summary>
        public int LocalPort
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket未连接，无法获取本地终结点的端口号");
                    return -1;
                }

                IPEndPoint ipEndPoint = (IPEndPoint)m_Socket.LocalEndPoint;
                if (ipEndPoint == null)
                {
                    Debug.LogError("本地终结点为空，无法获取端口号");
                    return -1;
                }

                return ipEndPoint.Port;
            }
        }

        /// <summary>
        /// 远程终结点的 IP 地址
        /// </summary>
        public IPAddress RemoteIPAddress
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket未连接，无法获取远程终结点的IP");
                    return null;
                }

                IPEndPoint ipEndPoint = (IPEndPoint)m_Socket.RemoteEndPoint;
                if (ipEndPoint == null)
                {
                    Debug.LogError("远程终结点为空，无法获取IP");
                    return null;
                }

                return ipEndPoint.Address;
            }
        }

        /// <summary>
        /// 远程终结点的端口号
        /// </summary>
        public int RemotePort
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket未连接，无法获取远程终结点的端口号");
                    return -1;
                }

                IPEndPoint ipEndPoint = (IPEndPoint)m_Socket.RemoteEndPoint;
                if (ipEndPoint == null)
                {
                    Debug.LogError("远程终结点为空，无法获取端口号");
                    return -1;
                }

                return ipEndPoint.Port;
            }
        }

        /// <summary>
        /// 要发送的消息包数量
        /// </summary>
        public int SendPacketCount
        {
            get
            {
                return m_SendPacketPool.Count;
            }
        }

        /// <summary>
        /// 接收缓冲区字节数
        /// </summary>
        public int ReceiveBufferSize
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket为空，无法获取接收缓冲区字节数");
                    return -1;
                }

                return m_Socket.ReceiveBufferSize;
            }
            set
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket为空，无法设置接收缓冲区字节数");
                    return;
                }

                m_Socket.ReceiveBufferSize = value;
            }
        }

        /// <summary>
        /// 发送缓冲区字节数
        /// </summary>
        public int SendBufferSize
        {
            get
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket为空，无法获取发送缓冲区字节数");
                    return -1;
                }

                return m_Socket.SendBufferSize;
            }
            set
            {
                if (m_Socket == null)
                {
                    Debug.LogError("Socket为空，无法设置发送缓冲区字节数");
                    return;
                }

                m_Socket.SendBufferSize = value;
            }
        }

        #endregion


        public NetworkChannel(string name, NetworkChannelHelperBase networkChannelHelper)
        {
            Name = name ?? string.Empty;
            m_SendPacketPool = new Queue<PacketBase>();
            m_ReceivePacketPool = new EventPool<PacketBase>();
            m_NetworkChannelHelper = networkChannelHelper;
            NetworkType = NetworkType.Unknown;
            ResetHeartBeatElapseSecondsWhenReceivePacket = false;
            HeartBeatInterval = DefaultHeartBeatInterval;
            m_Socket = null;
            m_SendState = new SendState();
            m_ReceiveState = new ReceiveState();
            m_HeartBeatState = new HeartBeatState();
            m_Active = false;
            m_Disposed = false;

            NetworkChannelConnected = null;
            NetworkChannelClosed = null;
            NetworkChannelMissHeartBeat = null;
            NetworkChannelError = null;
            NetworkChannelCustomError = null;

            networkChannelHelper.Initialize(this);
        }

        #region 生命周期
        /// <summary>
        /// 网络频道轮询
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Socket == null || !m_Active)
            {
                return;
            }

            //消息包发送处理
            ProcessSend();

            //轮询事件池
            m_ReceivePacketPool.Update(elapseSeconds, realElapseSeconds);

            if (HeartBeatInterval > 0f)
            {
                
                bool sendHeartBeat = false;
                int missHeartBeatCount = 0;
                lock (m_HeartBeatState)
                {
                    //开始心跳包发送计时
                    m_HeartBeatState.HeartBeatElapseSeconds += realElapseSeconds;
                    if (m_HeartBeatState.HeartBeatElapseSeconds >= HeartBeatInterval)
                    {
                        
                        sendHeartBeat = true;
                        missHeartBeatCount = m_HeartBeatState.MissHeartBeatCount;
                        m_HeartBeatState.HeartBeatElapseSeconds = 0f;
                        m_HeartBeatState.MissHeartBeatCount++;
                    }
                }

                //发送心跳包
                if (sendHeartBeat && m_NetworkChannelHelper.SendHeartBeat())
                {
                    if (missHeartBeatCount > 0 && NetworkChannelMissHeartBeat != null)
                    {
                        NetworkChannelMissHeartBeat(this, missHeartBeatCount);
                    }
                }
            }
        }

        /// <summary>
        /// 关闭网络频道
        /// </summary>
        public void Shutdown()
        {
            Close();
            m_ReceivePacketPool.Shutdown();
            m_NetworkChannelHelper.Shutdown();
        }

        /// <summary>
        /// 关闭连接并释放所有相关资源
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (m_Socket == null)
                {
                    return;
                }

                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Clear();
                }

                m_ReceivePacketPool.Clear();

                m_Active = false;
                try
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    m_Socket.Close();
                    m_Socket = null;

                    if (NetworkChannelClosed != null)
                    {
                        NetworkChannelClosed(this);
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记</param>
        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }

            m_Disposed = true;
        }
        #endregion

        /// <summary>
        /// 注册网络消息包处理方法
        /// </summary>
        /// <param name="handler">要注册的网络消息包处理器</param>
        public void RegisterHandler(PacketHandlerBase handler)
        {
            if (handler == null)
            {
                Debug.LogError("要注册的网络消息包处理器为空");
            }

            m_ReceivePacketPool.Subscribe(handler.Id, handler.Handle);
        }

        #region 网络连接
        /// <summary>
        /// 连接到远程主机
        /// </summary>
        /// <param name="ipAddress">远程主机的 IP 地址</param>
        /// <param name="port">远程主机的端口号</param>
        /// <param name="userData">用户自定义数据</param>
        public void Connect(IPAddress ipAddress, int port, object userData = null)
        {
            if (m_Socket != null)
            {
                Close();
                m_Socket = null;
            }

            //检查协议族
            switch (ipAddress.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    NetworkType = NetworkType.IPv4;
                    break;
                case AddressFamily.InterNetworkV6:
                    NetworkType = NetworkType.IPv6;
                    break;
                default:
                    string errorMessage = string.Format("不支持的协议组： {0}", ipAddress.AddressFamily.ToString());
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.AddressFamilyError, errorMessage);
                        return;
                    }

                    throw new Exception(errorMessage);
            }

            //创建Socket对象
            m_Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (m_Socket == null)
            {
                string errorMessage = "进行网络连接时创建Socket对象失败";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SocketError, errorMessage);
                    Debug.LogError(errorMessage);
                    return;
                }
            }

            //为接收消息包头作准备
            m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);

            //开始连接
            try
            {
                m_Socket.BeginConnect(ipAddress, port, ConnectCallback, new ConnectState(m_Socket, userData));
            }
            catch (Exception exception)
            {
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.ConnectError, exception.Message);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// 网络连接的回调
        /// </summary>
        private void ConnectCallback(IAsyncResult ar)
        {
            ConnectState socketUserData = (ConnectState)ar.AsyncState;

            //结束连接
            try
            {
                socketUserData.Socket.EndConnect(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                m_Active = false;

                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.ConnectError, exception.Message);
                    return;
                }

                throw;
            }

            //网络频道激活
            m_Active = true;


            lock (m_HeartBeatState)
            {
                //重置心跳包状态
                m_HeartBeatState.Reset(true);
            }

            if (NetworkChannelConnected != null)
            {
                NetworkChannelConnected(this, socketUserData.UserData);
            }

            //开始接收消息
            Receive();
        }
        #endregion

        #region 接收网络消息
        /// <summary>
        /// 接收网络消息
        /// </summary>
        private void Receive()
        {
            try
            {
                //异步接收
                m_Socket.BeginReceive(m_ReceiveState.Stream.GetBuffer(), (int)m_ReceiveState.Stream.Position, (int)(m_ReceiveState.Stream.Length - m_ReceiveState.Stream.Position), SocketFlags.None, ReceiveCallback, m_Socket);
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.ReceiveError, exception.Message);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// 接收网络消息的回调
        /// </summary>
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            int bytesReceived = 0;
            try
            {
                //计算接收到的消息字节数量
                bytesReceived = socket.EndReceive(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.ReceiveError, exception.Message);
                    return;
                }

                throw;
            }

            //接收到的消息字节数量不足，关闭连接
            if (bytesReceived <= 0)
            {
                Close();
                return;
            }

            //计算流位置
            m_ReceiveState.Stream.Position += bytesReceived;
            if (m_ReceiveState.Stream.Position < m_ReceiveState.Stream.Length)
            {
                //没接收完，继续接收
                Receive();
                return;
            }

            //重置流位置
            m_ReceiveState.Stream.Position = 0L;

            //处理接收到的消息包
            bool processSuccess = false;
            if (m_ReceiveState.PacketHeader != null)
            {
                processSuccess = ProcessPacket();
            }
            else
            {
                processSuccess = ProcessPacketHeader();
            }

            //继续接收消息
            if (processSuccess)
            {
                Receive();
                return;
            }
        }

        /// <summary>
        /// 处理网络消息包头
        /// </summary>
        /// <returns></returns>
        private bool ProcessPacketHeader()
        {
            try
            {
                //反序列化消息包头
                object customErrorData = null;
                PacketHeaderBase packetHeader = m_NetworkChannelHelper.DeserializePacketHeader(m_ReceiveState.Stream, out customErrorData);

                if (customErrorData != null && NetworkChannelCustomError != null)
                {
                    NetworkChannelCustomError(this, customErrorData);
                }

                if (packetHeader == null)
                {
                    string errorMessage = "网络消息包头为空";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, errorMessage);
                        Debug.LogError(errorMessage);
                        return false;
                    }


                }

                //为接收网络消息包作准备
                m_ReceiveState.PrepareForPacket(packetHeader);
                if (packetHeader.PacketLength <= 0)
                {
                    ProcessPacket();
                }
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, exception.ToString());
                    return false;
                }

                throw;
            }

            return true;
        }

        /// <summary>
        /// 处理网络消息包
        /// </summary>
        /// <returns></returns>
        private bool ProcessPacket()
        {
            //重置心跳包状态
            lock (m_HeartBeatState)
            {
                m_HeartBeatState.Reset(ResetHeartBeatElapseSecondsWhenReceivePacket);
            }

            try
            {
                //反序列化消息包
                object customErrorData = null;
                PacketBase packet = m_NetworkChannelHelper.DeserializePacket(m_ReceiveState.PacketHeader, m_ReceiveState.Stream, out customErrorData);

                if (customErrorData != null && NetworkChannelCustomError != null)
                {
                    NetworkChannelCustomError(this, customErrorData);
                }

                //派发消息包
                if (packet != null)
                {
                    m_ReceivePacketPool.Fire(this, packet);
                }

                //为接收消息包头作准备
                m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.DeserializePacketError, exception.ToString());
                    return false;
                }

                throw;
            }

            return true;
        }


        #endregion

        #region 发送网络消息
        /// <summary>
        /// 向远程主机发送消息包
        /// </summary>
        /// <typeparam name="T">消息包类型</typeparam>
        /// <param name="packet">要发送的消息包</param>
        public void Send<T>(T packet) where T : PacketBase
        {
            if (m_Socket == null)
            {
                string errorMessage = "Socket未连接，无法发送网络消息包";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SocketError, errorMessage);
                    Debug.LogError(errorMessage);
                    return;
                }
               
            }

            if (packet == null)
            {
                string errorMessage = "要发送的网络消息包为空";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SendError, errorMessage);
                    Debug.LogError(errorMessage);
                    return;
                }
            }

            //将要发送的网络消息包加入队列
            lock (m_SendPacketPool)
            {
                m_SendPacketPool.Enqueue(packet);
            }
        }

        /// <summary>
        /// 消息包发送处理
        /// </summary>
        private void ProcessSend()
        {
            if (m_SendPacketPool.Count <= 0)
            {
                return;
            }

            if (!m_SendState.IsFree)
            {
                return;
            }

            //从队列里获取要发送的消息包对象
            PacketBase packet = null;
            lock (m_SendPacketPool)
            {
                packet = m_SendPacketPool.Dequeue();
            }

            //序列化消息包
            byte[] packetBytes = null;
            try
            {
                packetBytes = m_NetworkChannelHelper.Serialize(packet);
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SerializeError, exception.ToString());
                    return;
                }

                throw;
            }

            if (packetBytes == null || packetBytes.Length <= 0)
            {
                string errorMessage = "序列化网络消息包失败";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SerializeError, errorMessage);
                    Debug.LogError(errorMessage);
                    return;
                }

            }

            //发送消息
            m_SendState.SetPacket(packetBytes);
            Send();
        }

        /// <summary>
        /// 发送网络消息
        /// </summary>
        private void Send()
        {
            try
            {
                m_Socket.BeginSend(m_SendState.GetPacketBytes(), m_SendState.Offset, m_SendState.Length, SocketFlags.None, SendCallback, m_Socket);
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SendError, exception.Message);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// 发送网络消息的回调
        /// </summary>
        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                //计算偏移
                m_SendState.Offset += socket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                m_Active = false;
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SendError, exception.Message);
                    return;
                }

                throw;
            }

            //偏移<长度，表示没发送完，就继续发送
            if (m_SendState.Offset < m_SendState.Length)
            {
                Send();
                return;
            }

            //重置发送状态
            m_SendState.Reset();
        }
        #endregion
    }
}

