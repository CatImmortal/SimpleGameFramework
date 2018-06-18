using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// Web 请求成功事件
    /// </summary>
    public class WebRequestSuccessEventArgs : GlobalEventArgs
    {
        private byte[] m_WebResponseBytes = null;

        /// <summary>
        /// Web 请求成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(WebRequestSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 获取 Web 请求成功事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// Web 请求任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// Web 请求地址
        /// </summary>
        public string WebRequestUri { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 获取 Web 响应的数据流
        /// </summary>
        /// <returns>Web 响应的数据流</returns>
        public byte[] GetWebResponseBytes()
        {
            return m_WebResponseBytes;
        }

        /// <summary>
        /// 清理 Web 请求成功事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            WebRequestUri = default(string);
            m_WebResponseBytes = default(byte[]);
            UserData = default(object);
        }

        /// <summary>
        /// 填充 Web 请求成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>Web 请求成功事件。</returns>
        public WebRequestSuccessEventArgs Fill(object userData, int serialId, string webRequestUri,byte[] webResponseBytes)
        {
            WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
            SerialId = serialId;
            WebRequestUri = webRequestUri;
            m_WebResponseBytes = webResponseBytes;
            UserData = wwwFormInfo.UserData;

            return this;
        }
    }
}

