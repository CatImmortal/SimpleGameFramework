using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SimpleGameFramework.Event;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 默认网络频道辅助器
    /// </summary>
    public class DefaultNetworkChannerHelper : NetworkChannelHelperBase
    {


        /// <summary>
        /// 服务器发往客户端的消息包的Type字典
        /// </summary>
        private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();

        /// <summary>
        /// 网络频道
        /// </summary>
        private NetworkChannel m_NetworkChannel;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 获取消息包头长度。
        /// </summary>
        public override int PacketHeaderLength
        {
            get
            {
                return sizeof(int);
            }
        }

        /// <summary>
        /// 获取服务器发往客户端的消息包的Type
        /// </summary>
        private Type GetServerToClientPacketType(int id)
        {
            Type type = null;
            if (m_ServerToClientPacketTypes.TryGetValue(id, out type))
            {
                return type;
            }

            return null;
        }

        /// <summary>
        /// 初始化网络频道辅助器
        /// </summary>
        /// <param name="networkChannel">网络频道</param>
        public override void Initialize(NetworkChannel networkChannel)
        {
            m_NetworkChannel = networkChannel;
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();

            // 反射注册包和包处理器
            Type packetBaseType = typeof(SCPacketBase);
            Type packetHandlerBaseType = typeof(PacketHandlerBase);

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }

                //注册消息包
                if (types[i].BaseType == packetBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetServerToClientPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Debug.LogError(string.Format("消息包Type： {0} 已经存在, 检查 '{1}' 或 '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name));
                        continue;
                    }

                    m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);
                }
                //注册消息包处理器
                else if (types[i].BaseType == packetHandlerBaseType)
                {
                    PacketHandlerBase packetHandler = (PacketHandlerBase)Activator.CreateInstance(types[i]);
                    m_NetworkChannel.RegisterHandler(packetHandler);
                }
            }

            //注册全局事件监听
            m_EventManager.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            m_EventManager.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            m_EventManager.Subscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            m_EventManager.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            m_EventManager.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }



        /// <summary>
        /// 关闭并清理网络频道辅助器。
        /// </summary>
        public override void Shutdown()
        {
            //取消对全局事件监听
            m_EventManager.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            m_EventManager.Unsubscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            m_EventManager.Unsubscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            m_EventManager.Unsubscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            m_EventManager.Unsubscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

            m_NetworkChannel = null;
        }

        /// <summary>
        /// 发送心跳消息包
        /// </summary>
        /// <returns>是否发送心跳消息包成功</returns>
        public override bool SendHeartBeat()
        {
            m_NetworkChannel.Send(ReferencePool.Acquire<CSHeartBeat>());
            return true;
        }

        #region 消息包的序列化与反序列化
        /// <summary>
        /// 序列化消息包
        /// </summary>
        /// <typeparam name="T">消息包类型</typeparam>
        /// <param name="packet">要序列化的消息包</param>
        /// <returns>序列化后的消息包字节流</returns>
        public override byte[] Serialize<T>(T packet)
        {
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null)
            {
                Debug.LogError("要序列化的消息包类型不合法");
                return null;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer)
            {
                Debug.LogError("要序列化的消息包不是客户端发往服务器类型的消息包");
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                //获取包头
                CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();

                //序列化消息包
                Serializer.Serialize(memoryStream, packetHeader);
                Serializer.SerializeWithLengthPrefix(memoryStream, packet, PrefixStyle.Fixed32);

                ReferencePool.Release(packetHeader);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 反序列消息包头
        /// </summary>
        /// <param name="source">要反序列化的来源流</param>
        /// <param name="customErrorData">用户自定义错误数据</param>
        public override PacketHeaderBase DeserializePacketHeader(Stream source, out object customErrorData)
        {
            // 注意：此方法并不在主线程调用！
            customErrorData = null;
            return (PacketHeaderBase)RuntimeTypeModel.Default.Deserialize(source, ReferencePool.Acquire<SCPacketHeader>(), typeof(SCPacketHeader));
        }

        /// <summary>
        /// 反序列化消息包
        /// </summary>
        /// <param name="packetHeader">消息包头</param>
        /// <param name="source">要反序列化的来源流</param>
        /// <param name="customErrorData">用户自定义错误数据</param>
        public override  PacketBase DeserializePacket(PacketHeaderBase packetHeader, Stream source, out object customErrorData)
        {
            // 注意：此方法并不在主线程调用！
            customErrorData = null;

            SCPacketHeader scPacketHeader = packetHeader as SCPacketHeader;
            if (scPacketHeader == null)
            {
                Debug.LogError("要反序列化的消息包的包头不合法");
                return null;
            }

            PacketBase packet = null;
            if (scPacketHeader.IsValid)
            {
                Type packetType = GetServerToClientPacketType(scPacketHeader.Id);
                if (packetType != null)
                {
                    //反序列化消息包
                    packet = (PacketBase)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, ReferencePool.Acquire(packetType), packetType, PrefixStyle.Fixed32, 0);
                }
                else
                {
                    Debug.LogError(string.Format("找不到要反序列化的消息包的Type： {0}", scPacketHeader.Id.ToString()));
                }
            }
            else
            {
                Debug.LogError("要反序列化的消息包不合法");
            }

            ReferencePool.Release(scPacketHeader);
            return packet;
        }
        #endregion

        #region 全局事件的处理方法
        private void OnNetworkConnected(object sender, GlobalEventArgs e)
        {
            NetworkConnectedEventArgs ne = (NetworkConnectedEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel)
            {
                return;
            }

            Debug.Log(string.Format("网络频道： {0} 连接完毕, 本地地址： {1}:{2} , 远程地址 {3}:{4}", ne.NetworkChannel.Name, ne.NetworkChannel.LocalIPAddress, ne.NetworkChannel.LocalPort.ToString(), ne.NetworkChannel.RemoteIPAddress, ne.NetworkChannel.RemotePort.ToString()));
        }

        private void OnNetworkClosed(object sender, GlobalEventArgs e)
        {
           NetworkClosedEventArgs ne = (NetworkClosedEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel)
            {
                return;
            }

            Debug.Log(string.Format("网络频道： {0} 已关闭.", ne.NetworkChannel.Name));
        }

        private void OnNetworkMissHeartBeat(object sender, GlobalEventArgs e)
        {
            NetworkMissHeartBeatEventArgs ne = (NetworkMissHeartBeatEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel)
            {
                return;
            }

           Debug.Log(string.Format("网络频道：{0} 丢失心跳包 {1} ", ne.NetworkChannel.Name, ne.MissCount.ToString()));

            if (ne.MissCount < 2)
            {
                return;
            }

            ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, GlobalEventArgs e)
        {
            NetworkErrorEventArgs ne = (NetworkErrorEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel)
            {
                return;
            }

            Debug.Log(string.Format("网络频道： {0}  错误, 错误码： {1}, 错误信息：{2}", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage));

            ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, GlobalEventArgs e)
        {
            NetworkCustomErrorEventArgs ne = (NetworkCustomErrorEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel)
            {
                return;
            }
        }
        #endregion
    }


}

