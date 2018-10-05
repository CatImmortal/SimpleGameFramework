using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的下载代理辅助器
    /// </summary>
    public class UnityWebRequestDownloadAgentHelper : DownloadAgentHelperBase,IDisposable
    {
        private UnityWebRequest m_UnityWebRequest = null;

        /// <summary>
        /// 已下载的大小
        /// </summary>
        private int m_DownloadedSize = 0;

        /// <summary>
        /// 是否已释放资源的标记
        /// </summary>
        private bool m_Disposed = false;

        //三种下载事件
        public override event EventHandler<DownloadAgentHelperUpdateEventArgs> DownloadAgentHelperUpdateEvent;
        public override event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperCompleteEvent;
        public override event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperErrorEvent;


        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, object userData)
        {
            m_UnityWebRequest = UnityWebRequest.Get(downloadUri);
            m_UnityWebRequest.SendWebRequest();
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, object userData)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Range", string.Format("bytes={0}-", fromPosition.ToString()));
            m_UnityWebRequest = UnityWebRequest.Post(downloadUri, header);
            m_UnityWebRequest.SendWebRequest();
        }
        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="toPosition">下载数据结束位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, int toPosition, object userData)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Range", string.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()));
            m_UnityWebRequest = UnityWebRequest.Post(downloadUri, header);
            m_UnityWebRequest.SendWebRequest();
        }

        /// <summary>
        /// 轮询下载代理辅助器
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {

            if (m_UnityWebRequest == null)
            {
                return;
            }

            if (!m_UnityWebRequest.isDone)
            {
                if (m_DownloadedSize < (int)m_UnityWebRequest.downloadedBytes)
                {
                    m_DownloadedSize = (int)m_UnityWebRequest.downloadedBytes;
                    DownloadAgentHelperUpdateEvent(this, new DownloadAgentHelperUpdateEventArgs((int)m_UnityWebRequest.downloadedBytes, null));
                }
                return;
            }

            if (m_UnityWebRequest.isNetworkError)
            {
                DownloadAgentHelperErrorEvent(this, new DownloadAgentHelperErrorEventArgs(m_UnityWebRequest.error));
            }
            else if (m_UnityWebRequest.downloadHandler.isDone)
            {
                DownloadAgentHelperCompleteEvent(this, new DownloadAgentHelperCompleteEventArgs((int)m_UnityWebRequest.downloadedBytes, m_UnityWebRequest.downloadHandler.data));
            }
        }

        /// <summary>
        /// 重置下载代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }
            m_DownloadedSize = 0;
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
        /// 释放资源
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

