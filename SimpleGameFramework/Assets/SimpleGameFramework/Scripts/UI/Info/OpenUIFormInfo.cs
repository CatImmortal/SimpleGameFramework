using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 打开界面的信息
    /// </summary>
    public class OpenUIFormInfo
    {

        /// <summary>
        /// 界面序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 界面组
        /// </summary>
        public UIGroup UIGroup { get; private set; }

        /// <summary>
        /// 界面是否暂停或遮挡
        /// </summary>
        public bool PauseCoveredUIForm { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public OpenUIFormInfo(int serialId, UIGroup uIGroup, bool pauseCoveredUIForm, object userData)
        {
            SerialId = serialId;
            UIGroup = uIGroup;
            PauseCoveredUIForm = pauseCoveredUIForm;
            UserData = userData;
        }
    }

}
