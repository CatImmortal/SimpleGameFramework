using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.WebRequest
{
    /// <summary>
    /// WWW表单信息
    /// </summary>
    public class WWWFormInfo
    {
        /// <summary>
        /// WWW表单
        /// </summary>
        public WWWForm WWWForm { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public WWWFormInfo(WWWForm wwwForm, object userData)
        {
            WWWForm = wwwForm;
            UserData = userData;
        }

    }

}
