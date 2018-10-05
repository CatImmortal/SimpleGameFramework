using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using ProtoBuf;
namespace SimpleGameFramework.Network
{
    /// <summary>
    /// 网络消息包基类
    /// </summary>
    public abstract class PacketBase : GlobalEventArgs, IExtensible
    {
        /// <summary>
        /// 网络消息包类型
        /// </summary>
        public abstract PacketType PacketType { get; }

        private IExtension m_ExtensionObject;

        public PacketBase()
        {
            m_ExtensionObject = null;
        }

        public IExtension GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref m_ExtensionObject, createIfMissing);
        }
    }

}
