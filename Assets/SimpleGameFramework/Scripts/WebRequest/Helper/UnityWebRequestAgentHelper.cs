using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的 Web 请求代理辅助器。
    /// </summary>
    public class UnityWebRequestAgentHelper : WebRequestAgentHelperBase
    {
        private UnityWebRequest m_UnityWebRequest = null;
        private bool m_Disposed = false;

        public override event EventHandler<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperCompleteEvent;
        public override event EventHandler<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperErrorEvent;

        public override void Request(string webRequestUri, object userData)
        {
            WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
            if (wwwFormInfo.WWWForm == null)
            {
                m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
            }
            else
            {
                m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, wwwFormInfo.WWWForm);
            }

            m_UnityWebRequest.SendWebRequest();
        }

        public override void Request(string webRequestUri, byte[] postData, object userData)
        {
            m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, Encoding.UTF8.GetString(postData));
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_UnityWebRequest == null || !m_UnityWebRequest.isDone)
            {
                return;
            }

            if (m_UnityWebRequest.isNetworkError)
            {
                WebRequestAgentHelperErrorEvent(this, new WebRequestAgentHelperErrorEventArgs(m_UnityWebRequest.error));
            }
            else if (m_UnityWebRequest.downloadHandler.isDone)
            {
                WebRequestAgentHelperCompleteEvent(this, new WebRequestAgentHelperCompleteEventArgs(m_UnityWebRequest.downloadHandler.data));
            }
        }

        /// <summary>
        /// 重置 Web 请求代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
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
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }
            }

            m_Disposed = true;
        }

    }
}

