using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面组
    /// </summary>
    public class UIGroup
    {
        #region 字段与属性
        /// <summary>
        /// 界面组名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 界面组辅助器
        /// </summary>
        public UIGroupHelperBase UIGroupHelper { get; private set; }

        /// <summary>
        /// 界面组深度
        /// </summary>
        private int m_Depth;

        /// <summary>
        /// 界面组深度
        /// </summary>
        public int Depth
        {
            get
            {
                return m_Depth;
            }
            set
            {
                if (m_Depth == value)
                {
                    return;
                }

                m_Depth = value;
                UIGroupHelper.SetDepth(m_Depth);
            }
        }


        /// <summary>
        /// 界面组是否暂停
        /// </summary>
        public bool Pause { get; set; }


        /// <summary>
        /// 界面信息链表
        /// </summary>
        private LinkedList<UIFormInfo> m_UIFormInfos;

        /// <summary>
        /// 界面组中界面数量
        /// </summary>
        public int UIFormCount
        {
            get
            {
                return m_UIFormInfos.Count;
            }
        }

        /// <summary>
        /// 当前界面
        /// </summary>
        public UIForm CurrentUIForm
        {
            get
            {
                return m_UIFormInfos.First != null ? m_UIFormInfos.First.Value.UIForm : null;
            }
        }
        #endregion

        #region 构造方法
        public UIGroup(string name, int depth, UIGroupHelperBase uiGroupHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("用来构造界面组的名称为空");
            }

            if (uiGroupHelper == null)
            {
                Debug.LogError("用来构造界面组的辅助器为空");
            }

            Name = name;
            Pause = false;
            UIGroupHelper = uiGroupHelper;
            m_UIFormInfos = new LinkedList<UIFormInfo>();
            Depth = depth;
        }
        #endregion

        /// <summary>
        /// 界面组轮询
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                LinkedListNode<UIFormInfo> next = current.Next;
                current.Value.UIForm.OnUpdate(elapseSeconds, realElapseSeconds);
                current = next;
            }
        }

        #region 检查界面
        /// <summary>
        /// 界面组中是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>界面组中是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm.SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 界面组中是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>界面组中是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("界面资源名称为空，无法检查界面是否存在");
                return false;
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm.UIFormAssetName == uiFormAssetName)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 获取界面
        /// <summary>
        /// 从界面组中获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(int serialId)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm.SerialId == serialId)
                {
                    return uiFormInfo.UIForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 从界面组中获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("界面资源名称为空，获取界面");
                return null;
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm.UIFormAssetName == uiFormAssetName)
                {
                    return uiFormInfo.UIForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public UIForm[] GetUIForms(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("界面资源名称为空，获取界面");
                return null;
            }

            List<UIForm> uiForms = new List<UIForm>();
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm.UIFormAssetName == uiFormAssetName)
                {
                    uiForms.Add(uiFormInfo.UIForm);
                }
            }

            return uiForms.ToArray();
        }

        /// <summary>
        /// 从界面组中获取所有界面
        /// </summary>
        /// <returns>界面组中的所有界面</returns>
        public UIForm[] GetAllUIForms()
        {
            List<UIForm> uiForms = new List<UIForm>();
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                uiForms.Add(uiFormInfo.UIForm);
            }

            return uiForms.ToArray();
        }
        #endregion

        #region 增加与移除界面

        /// <summary>
        /// 获取界面信息
        /// </summary>
        /// <param name="uiForm">界面</param>
        /// <returns>要获取的界面信息</returns>
        private UIFormInfo GetUIFormInfo(UIForm uiForm)
        {
            if (uiForm == null)
            {
                Debug.LogError("界面为空，无法获取界面信息");
                return null;
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIForm == uiForm)
                {
                    return uiFormInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 往界面组增加界面
        /// </summary>
        /// <param name="uiForm">要增加的界面</param>
        public void AddUIForm(UIForm uiForm)
        {
            UIFormInfo uiFormInfo = new UIFormInfo(uiForm);
            m_UIFormInfos.AddFirst(uiFormInfo);
        }

        /// <summary>
        /// 从界面组移除界面。
        /// </summary>
        /// <param name="uiForm">要移除的界面。</param>
        public void RemoveUIForm(UIForm uiForm)
        {
            UIFormInfo uiFormInfo = GetUIFormInfo(uiForm);
            if (uiFormInfo == null)
            {
                Debug.LogError("界面信息为空，无法移除界面");
            }

            //遮挡界面
            if (!uiFormInfo.Covered)
            {
                uiFormInfo.Covered = true;
                uiForm.OnCover();
            }

            //暂停界面
            if (!uiFormInfo.Paused)
            {
                uiFormInfo.Paused = true;
                uiForm.OnPause();
            }

            m_UIFormInfos.Remove(uiFormInfo);
        }

        #endregion

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIForm uiForm, object userData)
        {
            UIFormInfo uiFormInfo = GetUIFormInfo(uiForm);
            if (uiFormInfo == null)
            {
                Debug.LogError("要激活的界面的信息为空");
            }

            //将激活的界面放到链表最前
            m_UIFormInfos.Remove(uiFormInfo);
            m_UIFormInfos.AddFirst(uiFormInfo);
        }

        /// <summary>
        /// 刷新界面组
        /// </summary>
        public void Refresh()
        {
            LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
            bool pause = Pause;
            bool cover = false;
            int depth = UIFormCount;
            while (current != null)
            {
                LinkedListNode<UIFormInfo> next = current.Next;

                //改变界面深度
                current.Value.UIForm.OnDepthChanged(Depth, depth--);

                //界面组暂停时
                if (pause)
                {
                    //遮挡未被遮挡的界面
                    if (!current.Value.Covered)
                    {
                        current.Value.Covered = true;
                        current.Value.UIForm.OnCover();
                    }

                    //暂停未被暂停的界面
                    if (!current.Value.Paused)
                    {
                        current.Value.Paused = true;
                        current.Value.UIForm.OnPause();
                    }
                }
                else
                {
                    //恢复暂停的界面
                    if (current.Value.Paused)
                    {
                        current.Value.Paused = false;
                        current.Value.UIForm.OnResume();
                    }

                    //当前界面需要暂停被遮挡的界面时，暂停后面的界面
                    if (current.Value.UIForm.PauseCoveredUIForm)
                    {
                        pause = true;
                    }

                    
                    if (cover)
                    {
                        //遮挡未被遮挡的界面
                        if (!current.Value.Covered)
                        {
                            current.Value.Covered = true;
                            current.Value.UIForm.OnCover();
                        }
                    }
                    else
                    {
                        //恢复遮挡的界面
                        if (current.Value.Covered)
                        {
                            current.Value.Covered = false;
                            current.Value.UIForm.OnReveal();
                        }

                        cover = true;
                    }
                }

                current = next;
            }
        }
    }
}

