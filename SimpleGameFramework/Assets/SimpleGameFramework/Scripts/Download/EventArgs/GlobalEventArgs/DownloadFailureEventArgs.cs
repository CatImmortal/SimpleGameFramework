using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 下载失败事件
    /// </summary>
    public class DownloadFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 下载失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(DownloadFailureEventArgs).GetHashCode();

        /// <summary>
        /// 下载失败事件编号
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
        /// 用户自定义数据。
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 错误信息。
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理下载失败事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            DownloadPath = default(string);
            DownloadUri = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充下载失败事件
        /// </summary>
        /// <returns>下载失败事件</returns>
        public DownloadFailureEventArgs Fill(DownloadTask task, string errorMessage)
        {
            SerialId = task.SerialId;
            DownloadPath = task.DownloadPath;
            DownloadUri = task.DownloadUri;
            ErrorMessage = errorMessage;
            UserData = task.UserData;

            return this;
        }
    }
}

