using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.UI 
{
    /// <summary>
    /// 关闭界面完成事件
    /// </summary>
    public class CloseUIFormCompleteEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 关闭界面完成事件编号
        /// </summary>
        public static readonly int EventId = typeof(CloseUIFormCompleteEventArgs).GetHashCode();

        /// <summary>
        /// 关闭界面完成事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 界面序列编号
        /// </summary>
        public int SerialId { get; private set; }


        /// <summary>
        /// 界面资源名称
        /// </summary>
        public string UIFormAssetName { get; private set; }


        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 界面所属的界面组
        /// </summary>
        public UIGroup UIGroup { get; private set; }

        /// <summary>
        /// 清理关闭界面完成事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            UIFormAssetName = default(string);
            UIGroup = default(UIGroup);
            UserData = default(object);
        }

        /// <summary>
        /// 填充关闭界面完成事件
        /// </summary>
        /// <returns>关闭界面完成事件</returns>
        public CloseUIFormCompleteEventArgs Fill(int serialId,string uiFormAssetName,object userData,UIGroup uiGroup)
        {
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            UserData = userData;

            UIGroup = uiGroup;

            return this;
        }
    }
}

