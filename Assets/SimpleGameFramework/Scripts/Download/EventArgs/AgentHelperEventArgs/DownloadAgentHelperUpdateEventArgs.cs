using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Download
{
    /// <summary>
    ///  下载代理辅助器更新事件。
    /// </summary>
    public class DownloadAgentHelperUpdateEventArgs : EventArgs
    {
        public DownloadAgentHelperUpdateEventArgs(int size, byte[] bytes)
        {
            Size = size;
            Bytes = bytes;
        }

        /// <summary>
        /// 下载的数据大小。
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// 获取下载的数据流。
        /// </summary>
        public byte[] Bytes { get; private set; }
}
}


