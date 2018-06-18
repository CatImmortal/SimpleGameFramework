using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.ObjectPool;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面实例对象
    /// </summary>
    public class UIFormInstanceObject : ObjectBase
    {
        /// <summary>
        /// 界面资源
        /// </summary>
        private object m_UIFormAsset;

        private UIFormHelperBase m_UIFormHelper;

        public UIFormInstanceObject(string name, object uiFormAsset, object uiFormInstance, UIFormHelperBase uiFormHelper) : base(uiFormAsset, name)
        {
            if (uiFormAsset == null)
            {
                Debug.LogError("用来构造界面实例对象的资源名称为空");
            }

            if (uiFormHelper == null)
            {
                Debug.LogError("用来构造界面实例对象的辅助器为空");
            }

            m_UIFormAsset = uiFormAsset;
            m_UIFormHelper = uiFormHelper;
        }

        public override void Release()
        {
            m_UIFormHelper.ReleaseUIForm(m_UIFormAsset, Target);

        }
    }
}

