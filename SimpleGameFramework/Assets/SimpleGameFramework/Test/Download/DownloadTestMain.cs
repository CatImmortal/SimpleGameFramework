using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Download;
using SimpleGameFramework.Event;
using System;

public class DownloadTestMain : MonoBehaviour
{

    DownloadManager m_downloadManager;
    EventManager m_eventManager;

    

    void Awake()
    {
        m_eventManager = FrameworkEntry.Instance.GetManager<EventManager>();
        m_downloadManager = FrameworkEntry.Instance.GetManager<DownloadManager>();
    }

    void Start()
    {
        m_eventManager.Subscribe(DownloadStartEventArgs.EventId, OnDownloadStart);
        m_eventManager.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        m_eventManager.Subscribe(DownloadUpdateEventArgs.EventId, OnDownUpdate);
        m_eventManager.Subscribe(DownloadFailureEventArgs.EventId, OnDownFailure);
        m_downloadManager.AddDownloadTask(Application.dataPath + "/SimpleGameFramework/Test/Download/Test.jpg", "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1527358407150&di=c0da7d605f3d44bca1314bd9c918da1c&imgtype=0&src=http%3A%2F%2Fimg.zcool.cn%2Fcommunity%2F038c0ee5744f9a500000025ae5acd2a.jpg");

    }

    private void OnDownloadStart(object sender,GlobalEventArgs e)
    {
        DownloadStartEventArgs de = (DownloadStartEventArgs)e;
        Debug.Log("下载开始了：" + de.DownloadUri);
    }

    private void OnDownloadSuccess(object sender,GlobalEventArgs e)
    {
        DownloadSuccessEventArgs de = (DownloadSuccessEventArgs)e;
        Debug.Log("下载成功了："+de.CurrentSize+"-" + de.DownloadPath);
        
    }

    private void OnDownUpdate(object sender,GlobalEventArgs e)
    {
        DownloadUpdateEventArgs de = (DownloadUpdateEventArgs)e;
        Debug.Log("下载更新了：" + de.CurrentSize);
    }

    private void OnDownFailure(object sender,GlobalEventArgs e)
    {
        DownloadFailureEventArgs de = (DownloadFailureEventArgs)e;
        Debug.Log("下载失败了：" + de.ErrorMessage);
    }

}
