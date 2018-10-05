using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using UnityEngine.Events;

namespace SimpleGameFramework.WebRequest
{
    public class WebRequestAgent : ITaskAgent<WebRequestTask>
    {
        /// <summary>
        /// Web请求代理辅助器
        /// </summary>
        private WebRequestAgentHelperBase m_Helper;

        /// <summary>
        ///  Web 请求任务
        /// </summary>
        public WebRequestTask Task { get; private set; }

        /// <summary>
        /// 获取已经等待时间
        /// </summary>
        public float WaitTime { get; private set; }

        //下载过程中用来进行对外通知的委托（由WebRequestManager进行注册）
        public UnityAction<WebRequestAgent> WebRequestAgentStart;
        public UnityAction<WebRequestAgent, byte[]> WebRequestAgentSuccess;
        public UnityAction<WebRequestAgent, string> WebRequestAgentFailure;


        public WebRequestAgent(WebRequestAgentHelperBase webRequestAgentHelper)
        {
            if (webRequestAgentHelper == null)
            {
                Debug.LogError("用来构造Web请求代理的辅助器为空");
            }

            m_Helper = webRequestAgentHelper;
            Task = null;
            WaitTime = 0f;

            WebRequestAgentStart = null;
            WebRequestAgentSuccess = null;
            WebRequestAgentFailure = null;
        }

        /// <summary>
        /// 初始化 Web 请求代理
        /// </summary>
        public void Initialize()
        {
            m_Helper.WebRequestAgentHelperCompleteEvent += OnWebRequestAgentHelperComplete;
            m_Helper.WebRequestAgentHelperErrorEvent += OnWebRequestAgentHelperError;
        }

        /// <summary>
        /// Web 请求代理轮询
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Task.Status == WebRequestTaskStatus.Doing)
            {
                WaitTime += realElapseSeconds;
                if (WaitTime >= Task.Timeout)
                {
                    OnWebRequestAgentHelperError(this, new WebRequestAgentHelperErrorEventArgs("Web请求超时"));
                }
            }

            m_Helper.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理 Web 请求代理。
        /// </summary>
        public void Shutdown()
        {
            Reset();
            m_Helper.WebRequestAgentHelperCompleteEvent -= OnWebRequestAgentHelperComplete;
            m_Helper.WebRequestAgentHelperErrorEvent -= OnWebRequestAgentHelperError;
        }

        /// <summary>
        /// 开始处理 Web 请求任务
        /// </summary>
        /// <param name="task">要处理的 Web 请求任务</param>
        public void Start(WebRequestTask task)
        {
            if (task == null)
            {
                Debug.LogError("要开始处理的Web请求任务为空");
                return;
            }

            Task = task;
            Task.Status = WebRequestTaskStatus.Doing;

            if (WebRequestAgentStart != null)
            {
                WebRequestAgentStart(this);
            }

            //获取要发送的数据流
            byte[] postData = Task.GetPostData();
            if (postData == null)
            {
                m_Helper.Request(Task.WebRequestUri, Task.UserData);
            }
            else
            {
                m_Helper.Request(Task.WebRequestUri, postData, Task.UserData);
            }

            WaitTime = 0f;
        }

        /// <summary>
        /// 重置 Web 请求代理
        /// </summary>
        public void Reset()
        {
            m_Helper.Reset();
            Task = null;
            WaitTime = 0f;
        }

        #region 往辅助器注册的2个方法
        /// <summary>
        /// 辅助器请求完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebRequestAgentHelperComplete(object sender, WebRequestAgentHelperCompleteEventArgs e)
        {
            m_Helper.Reset();
            Task.Status = WebRequestTaskStatus.Done;

            if (WebRequestAgentSuccess != null)
            {
                WebRequestAgentSuccess(this, e.GetWebResponseBytes());
            }

            Task.Done = true;
        }

        /// <summary>
        /// 辅助器请求错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebRequestAgentHelperError(object sender, WebRequestAgentHelperErrorEventArgs e)
        {
            m_Helper.Reset();
            Task.Status = WebRequestTaskStatus.Error;

            if (WebRequestAgentFailure != null)
            {
                WebRequestAgentFailure(this, e.ErrorMessage);
            }

            Task.Done = true;
        }
        #endregion
    }
}

