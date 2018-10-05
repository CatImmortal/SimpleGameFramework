using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  下载代理辅助器错误事件
/// </summary>
public class DownloadAgentHelperErrorEventArgs : EventArgs {

    public DownloadAgentHelperErrorEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// 获取错误信息。
    /// </summary>
    public string ErrorMessage { get; private set; }


}
