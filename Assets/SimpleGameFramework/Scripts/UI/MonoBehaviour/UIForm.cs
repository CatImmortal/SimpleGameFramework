using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面
    /// </summary>
    public class UIForm : MonoBehaviour
    {
        /// <summary>
        /// 界面序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 界面资源名称
        /// </summary>
        public string UIFormAssetName { get; private set; }


        /// <summary>
        /// 获取界面实例
        /// </summary>
        public object Handle
        {
            get
            {
                return gameObject;
            }
        }


        /// <summary>
        /// 界面所属的界面组
        /// </summary>
        public UIGroup UIGroup { get; private set; }


        /// <summary>
        /// 获取界面深度
        /// </summary>
        public int DepthInUIGroup { get; private set; }


        /// <summary>
        /// 是否暂停被覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm { get; private set; }


        /// <summary>
        /// 界面逻辑
        /// </summary>
        public UIFormLogic Logic { get; private set; }

        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroup">界面所处的界面组</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int serialId, string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, bool isNewInstance, object userData)
        {
            //界面每次打开时都刷新一次序列编号
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            if (isNewInstance)
            {
                UIGroup = uiGroup;
            }
            else if (UIGroup != uiGroup)
            {
                Debug.LogError("非新实例对象的界面初始化时界面组不一致");
                return;
            }

            DepthInUIGroup = 0;
            PauseCoveredUIForm = pauseCoveredUIForm;

            if (!isNewInstance)
            {
                return;
            }

            //新界面实例才调用界面逻辑的OnInit
            Logic = GetComponent<UIFormLogic>();
            if (Logic == null)
            {
                Debug.LogError("获取的界面逻辑脚本为空");
                return;
            }

            //进行界面逻辑的初始化
            Logic.OnInit(userData);
        }

        #region 界面逻辑代理
        /// <summary>
        /// 界面回收
        /// </summary>
        public void OnRecycle()
        {
            SerialId = 0;
            DepthInUIGroup = 0;
            PauseCoveredUIForm = true;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnOpen(object userData)
        {
            Logic.OnOpen(userData);
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnClose(object userData)
        {
            Logic.OnClose(userData);
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public void OnPause()
        {
            Logic.OnPause();
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        public void OnResume()
        {
            Logic.OnResume();
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        public void OnCover()
        {
            Logic.OnCover();
        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        public void OnReveal()
        {
            Logic.OnReveal();
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnRefocus(object userData)
        {
            Logic.OnRefocus(userData);
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Logic.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
            Logic.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }


        #endregion
    }
}

