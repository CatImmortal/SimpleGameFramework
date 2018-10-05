using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 打开界面失败事件
    /// </summary>
    public class OpenUIFormFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 打开界面失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormFailureEventArgs).GetHashCode();

        /// <summary>
        /// 打开界面失败事件编号
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
        /// 界面组名称
        /// </summary>
        public string UIGroupName { get; private set; }

        /// <summary>
        /// 是否暂停被覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 清理打开界面失败事件
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            UIFormAssetName = default(string);
            UIGroupName = default(string);
            PauseCoveredUIForm = default(bool);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充打开界面失败事件
        /// </summary>
        /// <returns>打开界面失败事件</returns>
        public OpenUIFormFailureEventArgs Fill(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData,string errorMessage)
        {
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            UIGroupName = uiGroupName;
            PauseCoveredUIForm = pauseCoveredUIForm;
            UserData = userData;

            ErrorMessage = errorMessage;

            return this;
        }
    }

}
