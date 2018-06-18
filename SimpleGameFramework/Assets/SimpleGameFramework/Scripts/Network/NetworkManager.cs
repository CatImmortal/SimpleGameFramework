using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络管理器
    /// </summary>
    public class NetworkManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 网络频道的字典
        /// </summary>
        private Dictionary<string, NetworkChannel> m_NetworkChannels;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 获取网络频道数量
        /// </summary>
        public int NetworkChannelCount
        {
            get
            {
                return m_NetworkChannels.Count;
            }
        }

        public NetworkManager()
        {
            m_NetworkChannels = new Dictionary<string, NetworkChannel>();
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();
        }
        #endregion

        #region 生命周期
        public override void Init()
        {

        }

        /// <summary>
        /// 网络管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (KeyValuePair<string, NetworkChannel> networkChannel in m_NetworkChannels)
            {
                networkChannel.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理网络管理器
        /// </summary>
        public override void Shutdown()
        {
            foreach (KeyValuePair<string, NetworkChannel> networkChannel in m_NetworkChannels)
            {
                NetworkChannel nc = networkChannel.Value;
                nc.NetworkChannelConnected -= OnNetworkChannelConnected;
                nc.NetworkChannelClosed -= OnNetworkChannelClosed;
                nc.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
                nc.NetworkChannelError -= OnNetworkChannelError;
                nc.NetworkChannelCustomError -= OnNetworkChannelCustomError;
                nc.Shutdown();
            }

            m_NetworkChannels.Clear();
        }
        #endregion

        /// <summary>
        /// 检查是否存在网络频道
        /// </summary>
        /// <param name="name">网络频道名称</param>
        /// <returns>是否存在网络频道</returns>
        public bool HasNetworkChannel(string name)
        {
            return m_NetworkChannels.ContainsKey(name ?? string.Empty);
        }

        /// <summary>
        /// 获取网络频道
        /// </summary>
        /// <param name="name">网络频道名称</param>
        /// <returns>要获取的网络频道</returns>
        public NetworkChannel GetNetworkChannel(string name)
        {
            NetworkChannel networkChannel = null;
            if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out networkChannel))
            {
                return networkChannel;
            }

            return null;
        }

        /// <summary>
        /// 获取所有网络频道
        /// </summary>
        /// <returns>所有网络频道</returns>
        public NetworkChannel[] GetAllNetworkChannels()
        {
            int index = 0;
            NetworkChannel[] networkChannels = new NetworkChannel[m_NetworkChannels.Count];
            foreach (KeyValuePair<string, NetworkChannel> networkChannel in m_NetworkChannels)
            {
                networkChannels[index++] = networkChannel.Value;
            }

            return networkChannels;
        }

        /// <summary>
        /// 创建网络频道
        /// </summary>
        /// <param name="name">网络频道名称</param>
        /// <param name="networkChannelHelper">网络频道辅助器</param>
        /// <returns>要创建的网络频道</returns>
        public NetworkChannel CreateNetworkChannel(string name, NetworkChannelHelperBase networkChannelHelper)
        {
            if (networkChannelHelper == null)
            {
                Debug.LogError("用来创建网络频道的辅助器为空，无法创建");
                return null;
            }

            if (networkChannelHelper.PacketHeaderLength <= 0)
            {
                Debug.LogError("用来创建网络频道的辅助器为空，无法创建");
                return null;
            }

            if (HasNetworkChannel(name))
            {
                Debug.LogError("用来创建网络频道的辅助器为空，无法创建");
                return null;
            }

            NetworkChannel networkChannel = new NetworkChannel(name, networkChannelHelper);
            networkChannel.NetworkChannelConnected += OnNetworkChannelConnected;
            networkChannel.NetworkChannelClosed += OnNetworkChannelClosed;
            networkChannel.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
            networkChannel.NetworkChannelError += OnNetworkChannelError;
            networkChannel.NetworkChannelCustomError += OnNetworkChannelCustomError;
            m_NetworkChannels.Add(name, networkChannel);
            return networkChannel;
        }

        /// <summary>
        /// 销毁网络频道
        /// </summary>
        /// <param name="name">网络频道名称</param>
        /// <returns>是否销毁网络频道成功</returns>
        public bool DestroyNetworkChannel(string name)
        {
            NetworkChannel networkChannel = null;
            if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out networkChannel))
            {
                networkChannel.NetworkChannelConnected -= OnNetworkChannelConnected;
                networkChannel.NetworkChannelClosed -= OnNetworkChannelClosed;
                networkChannel.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
                networkChannel.NetworkChannelError -= OnNetworkChannelError;
                networkChannel.NetworkChannelCustomError -= OnNetworkChannelCustomError;
                networkChannel.Shutdown();
                return m_NetworkChannels.Remove(name);
            }

            return false;
        }

        #region 向网络频道的委托注册的5个方法
        private void OnNetworkChannelConnected(NetworkChannel networkChannel, object userData)
        {
            //派发网络连接成功事件
            NetworkConnectedEventArgs e = ReferencePool.Acquire<NetworkConnectedEventArgs>();
            m_EventManager.Fire(this, e.Fill(networkChannel, userData));
        }

        private void OnNetworkChannelClosed(NetworkChannel networkChannel)
        {
            //派发网络连接关闭事件
            NetworkClosedEventArgs e = ReferencePool.Acquire<NetworkClosedEventArgs>();
            m_EventManager.Fire(this, e.Fill(networkChannel));
        }

        private void OnNetworkChannelMissHeartBeat(NetworkChannel networkChannel, int missHeartBeatCount)
        {
            //派发网络心跳包丢失事件
            NetworkMissHeartBeatEventArgs e = ReferencePool.Acquire<NetworkMissHeartBeatEventArgs>();
            m_EventManager.Fire(this, e.Fill(networkChannel, missHeartBeatCount));
        }

        private void OnNetworkChannelError(NetworkChannel networkChannel, NetworkErrorCode errorCode, string errorMessage)
        {
            //派发网络错误事件
            NetworkErrorEventArgs e = ReferencePool.Acquire<NetworkErrorEventArgs>();
            m_EventManager.Fire(this, e.Fill(networkChannel,errorCode,errorMessage));
        }

        private void OnNetworkChannelCustomError(NetworkChannel networkChannel, object customErrorData)
        {
            //派发用户自定义网络错误事件
            NetworkCustomErrorEventArgs e = ReferencePool.Acquire<NetworkCustomErrorEventArgs>();
            m_EventManager.Fire(this, e.Fill(networkChannel,customErrorData));
        }
        #endregion
    }
}

