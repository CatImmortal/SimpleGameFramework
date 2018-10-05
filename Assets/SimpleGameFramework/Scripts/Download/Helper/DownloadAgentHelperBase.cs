using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 下载代理辅助器基类
    /// </summary>
    public abstract class DownloadAgentHelperBase
    {
        /// <summary>
        /// 下载代理辅助器更新事件
        /// </summary>
        public abstract event EventHandler<DownloadAgentHelperUpdateEventArgs> DownloadAgentHelperUpdateEvent;

        /// <summary>
        /// 下载代理辅助器完成事件
        /// </summary>
        public abstract event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperCompleteEvent;

        /// <summary>
        /// 下载代理辅助器错误事件
        /// </summary>
        public abstract event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperErrorEvent;

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, object userData);

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, int fromPosition, object userData);

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="toPosition">下载数据结束位置</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, int fromPosition, int toPosition , object userData);

        /// <summary>
        /// 轮询下载代理辅助器
        /// </summary>
        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 重置下载代理辅助器
        /// </summary>
        public abstract void Reset();

      
    }
}

