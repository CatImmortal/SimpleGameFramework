using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// 默认 Web 请求代理辅助器。
    /// </summary>
    public class DefaultWebRequestAgentHelper : WebRequestAgentHelperBase,IDisposable
    {
        private WWW m_WWW = null;
        private bool m_Disposed = false;

        public override event EventHandler<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperCompleteEvent;
        public override event EventHandler<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperErrorEvent;

       
        /// <summary>
        /// 通过 Web 请求代理辅助器发送请求
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Request(string webRequestUri, object userData)
        {

            WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
            if (wwwFormInfo.WWWForm == null)
            {
                m_WWW = new WWW(webRequestUri);
            }
            else
            {
                m_WWW = new WWW(webRequestUri, wwwFormInfo.WWWForm);
            }
        }

        /// <summary>
        /// 通过 Web 请求代理辅助器发送请求
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Request(string webRequestUri, byte[] postData, object userData)
        {
            m_WWW = new WWW(webRequestUri, postData);
        }

        /// <summary>
        /// 轮询Web请求代理辅助器
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_WWW == null || !m_WWW.isDone)
            {
                return;
            }

            if (!string.IsNullOrEmpty(m_WWW.error))
            {
                WebRequestAgentHelperErrorEvent(this, new WebRequestAgentHelperErrorEventArgs(m_WWW.error));
            }
            else
            {
                WebRequestAgentHelperCompleteEvent(this, new WebRequestAgentHelperCompleteEventArgs(m_WWW.bytes));
            }
        }

        /// <summary>
        /// 重置 Web 请求代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_WWW != null)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
            }

            m_Disposed = true;
        }

       
    }
}

