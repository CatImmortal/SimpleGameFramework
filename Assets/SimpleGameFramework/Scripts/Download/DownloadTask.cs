using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 下载任务的状态
    /// </summary>
    public enum DownloadTaskStatus
    {
        /// <summary>
        /// 准备下载
        /// </summary>
        Todo,
        /// <summary>
        /// 下载中
        /// </summary>
        Doing,
        /// <summary>
        /// 下载完成
        /// </summary>
        Done,
        /// <summary>
        /// 下载错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 下载任务
    /// </summary>
    public class DownloadTask : ITask
    {

        private static int s_Serial = 0;


        /// <summary>
        /// 下载任务的序列ID
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool Done { get; set; }
        
        /// <summary>
        /// 下载任务的状态
        /// </summary>
        public DownloadTaskStatus Status { get; set; }

        /// <summary>
        /// 获取原始下载地址。
        /// </summary>
        public string DownloadUri { get; private set; }

        /// <summary>
        /// 下载后存在的路径
        /// </summary>
        public string DownloadPath { get; private set; }

        /// <summary>
        /// 将缓冲区写入磁盘的临界大小
        /// </summary>
        public long FlushSize { get; private set; }

        /// <summary>
        /// 下载超时时长，以秒为单位
        /// </summary>
        public float Timeout { get; private set; }

        /// <summary>
        /// 用户自定义数据。
        /// </summary>
        public object UserData { get; private set; }

        public DownloadTask(string downloadUri, string downloadPath, long flushSize, float timeout, object userData)
        {
            s_Serial++;
            Done = false;
            Status = DownloadTaskStatus.Todo;
            DownloadUri = downloadUri;
            DownloadPath = downloadPath;
            FlushSize = flushSize;
            Timeout = timeout;
            UserData = userData;
        }

    }
}

