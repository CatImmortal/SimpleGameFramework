using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面信息
    /// </summary>
    public class UIFormInfo
    {

        /// <summary>
        /// 界面
        /// </summary>
        public UIForm UIForm { get; private set; }

        /// <summary>
        /// 界面是否暂停
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// 界面是否遮挡
        /// </summary>
        public bool Covered { get; set; }

        public UIFormInfo(UIForm uIForm)
        {
            UIForm = uIForm;
            Paused = true;
            Covered = true;
        }
    }
}

