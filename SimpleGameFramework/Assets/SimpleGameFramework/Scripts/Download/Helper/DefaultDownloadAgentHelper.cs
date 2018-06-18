using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 默认下载代理辅助器（使用WWW实现下载）
    /// </summary>
    public class DefaultDownloadAgentHelper : DownloadAgentHelperBase,IDisposable
    {
        private WWW m_WWW = null;

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
            m_WWW = new WWW(downloadUri);
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
            m_WWW = new WWW(downloadUri, null, header);
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
            m_WWW = new WWW(downloadUri, null, header);
        }


        /// <summary>
        /// 轮询下载代理辅助器
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_WWW == null)
            {
                return;
            }

            if (!m_WWW.isDone)
            {
                if (m_DownloadedSize < m_WWW.bytesDownloaded)
                {
                    //更新已下载的大小
                    m_DownloadedSize = m_WWW.bytesDownloaded;
                    //派发更新事件
                    DownloadAgentHelperUpdateEvent(this, new DownloadAgentHelperUpdateEventArgs(m_WWW.bytesDownloaded, null));
                }

                return;
            }

            if (!string.IsNullOrEmpty(m_WWW.error))
            {
                //派发错误事件
                DownloadAgentHelperErrorEvent(this, new DownloadAgentHelperErrorEventArgs(m_WWW.error));
            }
            else
            {
                //派发完成事件
                DownloadAgentHelperCompleteEvent(this, new DownloadAgentHelperCompleteEventArgs(m_WWW.bytesDownloaded, m_WWW.bytes));
            }
        }

        /// <summary>
        /// 重置下载代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }

            m_DownloadedSize = 0;
        }

        #region 释放资源
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
                if (m_WWW != null)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
            }

            m_Disposed = true;
        }
        #endregion
    }
}

