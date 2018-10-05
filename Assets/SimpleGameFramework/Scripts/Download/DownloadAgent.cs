using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace SimpleGameFramework.Download
{
    public class DownloadAgent : ITaskAgent<DownloadTask>, IDisposable
    {
        #region 字段与属性
        /// <summary>
        /// 下载代理辅助器
        /// </summary>
        private DownloadAgentHelperBase m_Helper;

        /// <summary>
        /// 下载任务
        /// </summary>
        public DownloadTask Task { get; private set; }

        /// <summary>
        /// 文件IO流
        /// </summary>
        private FileStream m_FileStream;

        /// <summary>
        /// 已缓冲数据的大小
        /// </summary>
        private int m_WaitFlushSize;

        /// <summary>
        /// 获取已经等待时间
        /// </summary>
        public float WaitTime { get; private set; }

        /// <summary>
        /// 开始下载时已经存在的大小
        /// </summary>
        public int StartLength { get; private set; }

        /// <summary>
        /// 已经下载的大小
        /// </summary>
        public int DownloadedSize { get; private set; }

        /// <summary>
        /// 总下载的大小
        /// </summary>
        public int CurrentLength
        {
            get
            {
                return StartLength + DownloadedSize;
            }
        }
        /// <summary>
        /// 获取已经存盘的大小
        /// </summary>
        public long SavedLength { get; private set; }

        /// <summary>
        /// 是否已释放资源的标记
        /// </summary>
        private bool m_Disposed;


        //下载过程中用来进行对外通知的委托（由DownloadManager进行注册）
        public UnityAction<DownloadAgent> DownloadAgentStart;
        public UnityAction<DownloadAgent, int> DownloadAgentUpdate;
        public UnityAction<DownloadAgent, int> DownloadAgentSuccess;
        public UnityAction<DownloadAgent, string> DownloadAgentFailure;
        #endregion

        #region 构造方法
        public DownloadAgent(DownloadAgentHelperBase downloadAgentHelper)
        {
            if (downloadAgentHelper == null)
            {
                Debug.LogError("用来构造下载任务代理的辅助器为空");
            }

            m_Helper = downloadAgentHelper;
            Task = null;
            m_FileStream = null;
            m_WaitFlushSize = 0;
            WaitTime = 0f;
            StartLength = 0;
            DownloadedSize = 0;
            SavedLength = 0;
            m_Disposed = false;

            DownloadAgentStart = null;
            DownloadAgentUpdate = null;
            DownloadAgentSuccess = null;
            DownloadAgentFailure = null;
        }
        #endregion


        /// <summary>
        /// 初始化下载代理（注册方法）
        /// </summary>
        public void Initialize()
        {
            m_Helper.DownloadAgentHelperUpdateEvent += OnDownloadAgentHelperUpdate;
            m_Helper.DownloadAgentHelperCompleteEvent += OnDownloadAgentHelperComplete;
            m_Helper.DownloadAgentHelperErrorEvent += OnDownloadAgentHelperError;
        }

        #region 下载代理的操作
        /// <summary>
        /// 开始下载任务
        /// </summary>
        public void Start(DownloadTask task)
        {
            if (task == null)
            {
                Debug.LogError("要开始的任务为空");
                return;
            }

            Task = task;
            Task.Status = DownloadTaskStatus.Doing;
            //设置文件要下载到的的路径
            string downloadFile = string.Format("{0}.download", Task.DownloadPath);

            try
            {
                if (File.Exists(downloadFile))
                {
                    //路径已存在，用流打开
                    m_FileStream = File.OpenWrite(downloadFile);
                    //重新设置流的位置到文件最后
                    m_FileStream.Seek(0, SeekOrigin.End);
                    SavedLength = m_FileStream.Length;
                    StartLength = (int)SavedLength;
                    DownloadedSize = 0;
                }
                else
                {
                    //路径不存在就创建路径
                    string directory = Path.GetDirectoryName(Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    //创建流与文件路径
                    m_FileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    StartLength  = DownloadedSize = 0;
                    SavedLength = 0;
                }

                //开始下载
                if (DownloadAgentStart != null)
                {
                    //通知DownloadManager，开始了下载
                    DownloadAgentStart(this);
                }
                //通过下载代理辅助器进行下载
                if (StartLength > 0)
                {
                    m_Helper.Download(Task.DownloadUri,StartLength,Task.UserData);
                }
                else
                {
                    m_Helper.Download(Task.DownloadUri, Task.UserData);
                }
            }
            catch (Exception e)
            {
                OnDownloadAgentHelperError(this, new DownloadAgentHelperErrorEventArgs(e.Message));
            }
        }

        /// <summary>
        /// 轮询下载任务
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Task.Status == DownloadTaskStatus.Doing)
            {
                WaitTime += realElapseSeconds;
                //下载超时
                if (WaitTime >= Task.Timeout)
                {
                    OnDownloadAgentHelperError(this, new DownloadAgentHelperErrorEventArgs("Timeout"));
                }
            }

            m_Helper.Update(elapseSeconds,realElapseSeconds);
        }

        /// <summary>
        /// 重置下载代理
        /// </summary>
        public void Reset()
        {
            m_Helper.Reset();

            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream = null;
            }

            Task = null;
            m_WaitFlushSize = 0;
            WaitTime = 0f;
            StartLength = 0;
            DownloadedSize = 0;
            SavedLength = 0;
        }

        /// <summary>
        /// 关闭并清理下载代理
        /// </summary>
        public void Shutdown()
        {
            Dispose();

            m_Helper.DownloadAgentHelperUpdateEvent -= OnDownloadAgentHelperUpdate;
            m_Helper.DownloadAgentHelperCompleteEvent -= OnDownloadAgentHelperComplete;
            m_Helper.DownloadAgentHelperErrorEvent -= OnDownloadAgentHelperError;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
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
                if (m_FileStream != null)
                {
                    m_FileStream.Dispose();
                    m_FileStream = null;
                }
            }

            m_Disposed = true;
        }
        #endregion

        #region 保存下载的字节
        /// <summary>
        /// 保存下载的字节
        /// </summary>
        private void SaveBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            try
            {
                //写入数据
                int length = bytes.Length;
                m_FileStream.Write(bytes, 0, length);

                //更新记录
                m_WaitFlushSize += length;
                SavedLength += length;

                if (m_WaitFlushSize >= Task.FlushSize)
                {
                    //清空缓冲区
                    m_FileStream.Flush();
                    m_WaitFlushSize = 0;
                }
            }
            catch (Exception e)
            {
                OnDownloadAgentHelperError(this, new DownloadAgentHelperErrorEventArgs(e.Message));
            }
        }
        #endregion

        #region 往辅助器注册的三个方法
        /// <summary>
        /// 辅助器下载更新时
        /// </summary>
        private void OnDownloadAgentHelperUpdate(object sender, DownloadAgentHelperUpdateEventArgs e)
        {
            //重置等待时间
            WaitTime = 0;

            //保存已下载的数据
            byte[] bytes = e.Bytes;
            SaveBytes(bytes);

            //更新已下载的数据大小
            DownloadedSize = e.Size;

            if (DownloadAgentUpdate != null)
            {
                //调用委托，通知下载更新
                DownloadAgentUpdate(this,DownloadedSize);
            }
        }

        /// <summary>
        /// 辅助器下载完成时
        /// </summary>
        private void OnDownloadAgentHelperComplete(object sender, DownloadAgentHelperCompleteEventArgs e)
        {
            //重置等待时间
            WaitTime = 0;

            //保存下载好的内容
            byte[] bytes = e.Bytes;
            SaveBytes(bytes);

            DownloadedSize = e.Size;
            if (SavedLength != CurrentLength)
            {
                Debug.LogError("保存的数据长度与下载的数据长度不同");
            }

            m_Helper.Reset();
            m_FileStream.Close();
            m_FileStream = null;

            //把原来已存在的文件删掉
            if (File.Exists(Task.DownloadPath))
            {
                File.Delete(Task.DownloadPath);
            }

            //修改下载完成的文件的后缀（去掉.download）
            File.Move(string.Format("{0}.download", Task.DownloadPath), Task.DownloadPath);

            Task.Status = DownloadTaskStatus.Done;

            if (DownloadAgentSuccess != null)
            {
                //调用委托，通知下载成功
                DownloadAgentSuccess(this, bytes != null ? bytes.Length : 0);
            }

            Task.Done = true;
        }

        /// <summary>
        /// 辅助器下载失败时
        /// </summary>
        private void OnDownloadAgentHelperError(object sender, DownloadAgentHelperErrorEventArgs e)
        {
            m_Helper.Reset();
            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream = null;
            }

            Task.Status = DownloadTaskStatus.Error;

            if (DownloadAgentFailure != null)
            {
                //调用委托，通知下载失败
                DownloadAgentFailure(this, e.ErrorMessage);
            }

            Task.Done = true;
        }
        #endregion
    }
}

