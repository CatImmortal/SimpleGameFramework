using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 打开界面更新事件
    /// </summary>
    public class OpenUIFormUpdateEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 打开界面更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 打开界面更新事件编号
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
        /// 打开界面进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 清理打开界面更新事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = default(int);
            UIFormAssetName = default(string);
            UIGroupName = default(string);
            PauseCoveredUIForm = default(bool);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充打开界面更新事件
        /// </summary>
        /// <returns>打开界面更新事件。</returns>
        public OpenUIFormUpdateEventArgs Fill(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData,float progress)
        {
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            UIGroupName = uiGroupName;
            PauseCoveredUIForm = pauseCoveredUIForm;
            UserData = userData;

            Progress = Progress;

            return this;
        }
    }
}

