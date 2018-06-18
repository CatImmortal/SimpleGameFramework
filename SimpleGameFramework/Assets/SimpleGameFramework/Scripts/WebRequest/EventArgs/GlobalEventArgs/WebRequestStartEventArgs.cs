using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// Web请求开始事件
    /// </summary>
    public class WebRequestStartEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// Web 请求开始事件编号
        /// </summary>
        public static readonly int EventId = typeof(WebRequestStartEventArgs).GetHashCode();

        /// <summary>
        /// Web 请求开始事件编号
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
        /// 清理 Web 请求开始事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            WebRequestUri = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充 Web 请求开始事件
        /// </summary>
        /// <returns>Web 请求开始事件。</returns>
        public WebRequestStartEventArgs Fill(object userData,int serialId,string webRequestUri)
        {
            WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
            SerialId = serialId;
            WebRequestUri = webRequestUri;
            UserData = wwwFormInfo.UserData;

            return this;
        }
    }

}
