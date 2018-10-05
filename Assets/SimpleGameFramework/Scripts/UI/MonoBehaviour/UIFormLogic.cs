using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面逻辑基类
    /// </summary>
    public abstract class UIFormLogic : MonoBehaviour
    {
        /// <summary>
        /// 界面
        /// </summary>
        public UIForm UIForm
        {
            get
            {
                return GetComponent<UIForm>();
            }
        }

        /// <summary>
        /// 界面名称
        /// </summary>
        public string Name
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        /// <summary>
        /// 界面是否可用
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        /// <summary>
        /// 获取已缓存的 Transform
        /// </summary>
        public Transform CachedTransform { get; private set; }

        #region 界面相关方法
        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnInit(object userData)
        {
            if (CachedTransform == null)
            {
                CachedTransform = transform;
            }
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnOpen(object userData)
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnClose(object userData)
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public virtual void OnPause()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        public virtual void OnResume()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        public virtual void OnCover()
        {

        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        public virtual void OnReveal()
        {

        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnRefocus(object userData)
        {

        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {

        }
        #endregion
    }
}

