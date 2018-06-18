using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// Web 请求管理器
    /// </summary>
    public class WebRequestManager : ManagerBase
    {
        private EventManager m_eventManager;

        /// <summary>
        /// Web请求任务池
        /// </summary>
        private TaskPool<WebRequestTask> m_TaskPool;

        /// <summary>
        /// Web 请求超时时长，以秒为单位。
        /// </summary>
        public float Timeout { get; set; }

        /// <summary>
        /// Web请求代理辅助器数量
        /// </summary>
        private int m_WebRequestAgentHelperCount = 1;

        /// <summary>
        /// web 请求代理总数量
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return m_TaskPool.TotalAgentCount;
            }
        }

        /// <summary>
        /// 可用 Web 请求代理数量
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_TaskPool.FreeAgentCount;
            }
        }

        /// <summary>
        /// 工作中 Web 请求代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_TaskPool.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 等待 Web 请求数量
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_TaskPool.WaitingTaskCount;
            }
        }

        public WebRequestManager()
        {
            m_eventManager = FrameworkEntry.Instance.GetManager<EventManager>();
            m_TaskPool = new TaskPool<WebRequestTask>();
            Timeout = 30f;
        }

        public override void Init()
        {
            for (int i = 0; i < m_WebRequestAgentHelperCount; i++)
            {
                AddWebRequestAgentHelper(new DefaultWebRequestAgentHelper());
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_TaskPool.Update(elapseSeconds, realElapseSeconds);
        }

        public override void Shutdown()
        {
            m_TaskPool.Shutdown();
        }

        #region Web请求任务的增加与移除
        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="wwwForm">WWW 表单</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
         public int AddWebRequest(string webRequestUri, byte[] postData = null, WWWForm wwwForm = null, object userData = null)
        {
            if (string.IsNullOrEmpty(webRequestUri))
            {
                Debug.LogError("Web请求地址为空");
                return -1;
            }

            if (TotalAgentCount <= 0)
            {
                Debug.LogError("总Web请求代理数量为0");
                return -1;
            }

            WWWFormInfo info = new WWWFormInfo(wwwForm, userData);
            WebRequestTask webRequestTask = new WebRequestTask(webRequestUri, postData, Timeout, info);
            m_TaskPool.AddTask(webRequestTask);

            return webRequestTask.SerialId;
        }

        /// <summary>
        /// 移除 Web 请求任务
        /// </summary>
        /// <param name="serialId">要移除 Web 请求任务的序列编号</param>
        /// <returns>是否移除 Web 请求任务成功</returns>
        public bool RemoveWebRequest(int serialId)
        {
            return m_TaskPool.RemoveTask(serialId) != null;
        }

        /// <summary>
        /// 移除所有 Web 请求任务
        /// </summary>
        public void RemoveAllWebRequests()
        {
            m_TaskPool.RemoveAllTasks();
        }
        #endregion

        #region 增加Web请求代理辅助器
        /// <summary>
        /// 增加 Web 请求代理辅助器
        /// </summary>
        /// <param name="webRequestAgentHelper">要增加的 Web 请求代理辅助器</param>
        public void AddWebRequestAgentHelper(WebRequestAgentHelperBase webRequestAgentHelper)
        {
            WebRequestAgent agent = new WebRequestAgent(webRequestAgentHelper);
            agent.WebRequestAgentStart += OnWebRequestStart;
            agent.WebRequestAgentSuccess += OnWebRequestSuccess;
            agent.WebRequestAgentFailure += OnWebRequestFailure;

            m_TaskPool.AddAgent(agent);
        }
        #endregion

        #region 往Web请求代理注册的3个方法
        private void OnWebRequestStart(WebRequestAgent sender)
        {
            WebRequestStartEventArgs e = ReferencePool.Acquire<WebRequestStartEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task.UserData, sender.Task.SerialId, sender.Task.WebRequestUri));
            Debug.Log("派发了Web请求开始的事件");
        }

        private void OnWebRequestSuccess(WebRequestAgent sender,byte[] webResponseBytes)
        {
            WebRequestSuccessEventArgs e = ReferencePool.Acquire<WebRequestSuccessEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task.UserData, sender.Task.SerialId, sender.Task.WebRequestUri,webResponseBytes));
            Debug.Log("派发了Web请求成功的事件");
        }

        private void OnWebRequestFailure(WebRequestAgent sender,string errorMessage)
        {
            WebRequestStartEventArgs e = ReferencePool.Acquire<WebRequestStartEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task.UserData, sender.Task.SerialId, sender.Task.WebRequestUri));
            Debug.Log("派发了Web请求失败的事件");
        }
        #endregion
    }
}

