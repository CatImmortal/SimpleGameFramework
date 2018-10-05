using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// Web 请求任务的状态
    /// </summary>
    public enum WebRequestTaskStatus
    {
        /// <summary>
        /// 准备请求
        /// </summary>
        Todo,

        /// <summary>
        /// 请求中
        /// </summary>
        Doing,

        /// <summary>
        /// 请求完成
        /// </summary>
        Done,

        /// <summary>
        /// 请求错误
        /// </summary>
        Error
    }

    public class WebRequestTask : ITask
    {
        private static int s_Serial = 0;
        private readonly byte[] m_PostData;

        /// <summary>
        ///  Web 请求任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        ///  Web 请求任务是否完成。
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        ///  Web 请求任务的状态
        /// </summary>
        public WebRequestTaskStatus Status { get; set; }

        /// <summary>
        /// 要发送的远程地址
        /// </summary>
        public string WebRequestUri { get; private set; }

        /// <summary>
        ///  Web 请求超时时长，以秒为单位
        /// </summary>
        public float Timeout { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 初始化 Web 请求任务的新实例
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="timeout">下载超时时长，以秒为单位</param>
        /// <param name="userData">用户自定义数据</param>
        public WebRequestTask(string webRequestUri, byte[] postData, float timeout, object userData)
        {
            SerialId = s_Serial++;
            Done = false;
            Status = WebRequestTaskStatus.Todo;
            WebRequestUri = webRequestUri;
            m_PostData = postData;
            Timeout = timeout;
            UserData = userData;
        }

        /// <summary>
        /// 获取要发送的数据流。
        /// </summary>
        public byte[] GetPostData()
        {
            return m_PostData;
        }
    }
}

