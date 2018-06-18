using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
namespace SimpleGameFramework.Download
{
    /// <summary>
    /// 下载管理器
    /// </summary>
    public class DownloadManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_eventManager;

        /// <summary>
        /// 下载任务池
        /// </summary>
        private TaskPool<DownloadTask> m_TaskPool;

        /// <summary>
        /// 下载代理辅助器数量
        /// </summary>
        private int m_DownloadAgentHelperCount = 3;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int FlushSize { get; set; }

        /// <summary>
        /// 超时秒数
        /// </summary>
        public float Timeout { get; set; }

        public override int Priority
        {
            get
            {
                return 80;
            }
        }

        /// <summary>
        /// 下载代理总数量。
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return m_TaskPool.TotalAgentCount;
            }
        }

        /// <summary>
        /// 工作中下载代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_TaskPool.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 等待下载任务数量
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_TaskPool.WaitingTaskCount;
            }
        }
        #endregion

        public DownloadManager()
        {
            m_eventManager = FrameworkEntry.Instance.GetManager<EventManager>();
            m_TaskPool = new TaskPool<DownloadTask>();
            FlushSize = 1024 * 1024;
            Timeout = 30f;
        }

        public override void Init()
        {
            for (int i = 0; i < m_DownloadAgentHelperCount; i++)
            {
                AddDownloadAgentHelper(new UnityWebRequestDownloadAgentHelper());
            }
        }

        /// <summary>
        /// 关闭并清理下载管理器
        /// </summary>
        public override void Shutdown()
        {
            m_TaskPool.Shutdown();
        }

        /// <summary>
        /// 下载管理器轮询
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_TaskPool.Update(elapseSeconds, realElapseSeconds);
        }

        #region 增加下载代理辅助器
        /// <summary>
        /// 增加下载代理辅助器
        /// </summary>
        /// <param name="downloadAgentHelper">要增加的下载代理辅助器</param>
        public void AddDownloadAgentHelper(DownloadAgentHelperBase downloadAgentHelper)
        {
            //使用辅助器来创建下载代理
            DownloadAgent agent = new DownloadAgent(downloadAgentHelper);

            //往下载代理的委托里注册方法
            agent.DownloadAgentStart += OnDownloadStart;
            agent.DownloadAgentUpdate += OnDownloadUpdate;
            agent.DownloadAgentSuccess += OnDownloadSuccess;
            agent.DownloadAgentFailure += OnDownloadFailure;

            //往任务池里增加下载代理
            m_TaskPool.AddAgent(agent);
        }

        #endregion



        #region 下载任务的增加与移除
        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务编号</returns>
        public int AddDownloadTask(string downloadPath, string downloadUri, object userData = null)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                Debug.LogError("保存路径为空");
                return -1;
            }

            if (string.IsNullOrEmpty(downloadUri))
            {
                Debug.LogError("下载路径为空");
                return -1;
            }

            if (TotalAgentCount <= 0)
            {
                Debug.LogError("总下载代理数量为0");
                return -1;
            }

            DownloadTask downloadTask = new DownloadTask(downloadUri, downloadPath, FlushSize, Timeout, userData);
            m_TaskPool.AddTask(downloadTask);

            return downloadTask.SerialId;
        }

        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号</param>
        /// <returns>是否移除下载任务成功</returns>
        public bool RemoveDownload(int serialId)
        {
            return m_TaskPool.RemoveTask(serialId) != null;
        }

        /// <summary>
        /// 移除所有下载任务
        /// </summary>
        public void RemoveAllDownload()
        {
            m_TaskPool.RemoveAllTasks();
        }
        #endregion




        #region 往下载代理注册的4个方法（派发全局事件）

        private void OnDownloadStart(DownloadAgent sender)
        {
            DownloadStartEventArgs e = ReferencePool.Acquire<DownloadStartEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task, sender.CurrentLength));
            Debug.Log("派发了下载开始的事件");
        }

        private void OnDownloadUpdate(DownloadAgent sender, int DownloadedLength)
        {
            DownloadUpdateEventArgs e = ReferencePool.Acquire<DownloadUpdateEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task, DownloadedLength));
            Debug.Log("派发了下载更新的事件");
        }

        private void OnDownloadSuccess(DownloadAgent sender, int DownloadedLength)
        {
            DownloadSuccessEventArgs e = ReferencePool.Acquire<DownloadSuccessEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task, DownloadedLength));
            Debug.Log("派发了下载成功的事件");
        }

        private void OnDownloadFailure(DownloadAgent sender, string errorMessage)
        {
            DownloadFailureEventArgs e = ReferencePool.Acquire<DownloadFailureEventArgs>();
            m_eventManager.Fire(this, e.Fill(sender.Task, errorMessage));
            Debug.Log("派发了下载失败的事件");
        }

        #endregion
    }
}

