using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 打开界面成功事件
    /// </summary>
    public class OpenUIFormSuccessEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 打开界面成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 打开界面成功事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 打开成功的界面
        /// </summary>
        public UIForm UIForm { get; private set; }
 

        /// <summary>
        /// 加载持续时间
        /// </summary>
        public float Duration { get; private set; }


        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理打开界面成功事件。
        /// </summary>
        public override void Clear()
        {
            UIForm = default(UIForm);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充打开界面成功事件
        /// </summary>
        /// <returns>打开界面成功事件</returns>
        public OpenUIFormSuccessEventArgs Fill(UIForm uiForm,float duration,object userData)
        {
            UIForm = uiForm;
            Duration = duration;
            UserData = userData;

            return this;
        }
    }
}

