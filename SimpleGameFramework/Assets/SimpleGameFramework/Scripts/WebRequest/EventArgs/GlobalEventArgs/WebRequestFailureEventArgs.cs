using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// Web请求失败事件
    /// </summary>
    public class WebRequestFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// Web 请求失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(WebRequestFailureEventArgs).GetHashCode();

        /// <summary>
        /// Web 请求失败事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// Web 请求任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }


        /// <summary>
        /// Web 请求地址
        /// </summary>
        public string WebRequestUri { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

       

        /// <summary>
        /// 清理 Web 请求失败事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            WebRequestUri = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充 Web 请求失败事件
        /// </summary>
        /// <returns>Web 请求失败事件</returns>
        public WebRequestFailureEventArgs Fill(object userData, int serialId, string webRequestUri,string errorMessage)
        {
            WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
            SerialId = serialId;
            WebRequestUri = webRequestUri;
            ErrorMessage = errorMessage;
            UserData = wwwFormInfo.UserData;

            return this;
        }
    }
}

