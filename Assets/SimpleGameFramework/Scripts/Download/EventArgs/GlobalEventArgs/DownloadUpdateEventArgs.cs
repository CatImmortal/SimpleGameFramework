using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 下载更新事件
    /// </summary>
    public class DownloadUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 下载更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(DownloadUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 下载更新事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 下载任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 下载后存放路径
        /// </summary>
        public string DownloadPath { get; private set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadUri { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 当前已下载大小
        /// </summary>
        public int CurrentSize { get; private set; }

        /// <summary>
        /// 清理下载更新事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            DownloadPath = default(string);
            DownloadUri = default(string);
            CurrentSize = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充下载更新事件
        /// </summary>
        /// <returns>下载更新事件</returns>
        public DownloadUpdateEventArgs Fill(DownloadTask task, int currentSize)
        {
            SerialId = task.SerialId;
            DownloadPath = task.DownloadPath;
            DownloadUri = task.DownloadUri;
            CurrentSize = currentSize;
            UserData = task.UserData;

            return this;
        }
    }
}

