using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.ObjectPool;
using SimpleGameFramework.Event;
using SimpleGameFramework.Resource;
using System;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// 界面管理器
    /// </summary>
    public class UIManager : ManagerBase
    {
        #region 字段
        /// <summary>
        /// 界面组的字典
        /// </summary>
        private Dictionary<string, UIGroup> m_UIGroups;

        /// <summary>
        /// 正在加载的界面的列表
        /// </summary>
        private List<int> m_UIFormsBeingLoaded;

        /// <summary>
        /// 正在加载的界面资源名称的列表
        /// </summary>
        private List<string> m_UIFormAssetNamesBeingLoaded;

        /// <summary>
        /// 要释放的界面的哈希集
        /// </summary>
        private HashSet<int> m_UIFormsToReleaseOnLoad;

        /// <summary>
        /// 等待回收的界面的队列
        /// </summary>
        private LinkedList<UIForm> m_RecycleQueue;

        /// <summary>
        /// 对象池管理器
        /// </summary>
        private ObjectPoolManager m_ObjectPoolManager;

        /// <summary>
        /// 界面实例对象池
        /// </summary>
        private ObjectPool<UIFormInstanceObject> m_InstancePool;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 资源加载回调方法集
        /// </summary>
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 界面辅助器
        /// </summary>
        private UIFormHelperBase m_UIFormHelper;

        /// <summary>
        /// 序列编号
        /// </summary>
        private int m_Serial;

        /// <summary>
        /// 界面组数量
        /// </summary>
        public int UIGroupCount
        {
            get
            {
                return m_UIGroups.Count;
            }
        }

        /// <summary>
        /// 界面实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get
            {
                return m_InstancePool.AutoReleaseInterval;
            }
            set
            {
                m_InstancePool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 界面实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get
            {
                return m_InstancePool.Capacity;
            }
            set
            {
                m_InstancePool.Capacity = value;
            }
        }

        /// <summary>
        /// 界面实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get
            {
                return m_InstancePool.ExpireTime;
            }
            set
            {
                m_InstancePool.ExpireTime = value;
            }
        }


        #endregion

        public UIManager()
        {
            m_UIGroups = new Dictionary<string, UIGroup>();
            m_UIFormsBeingLoaded = new List<int>();
            m_UIFormAssetNamesBeingLoaded = new List<string>();
            m_UIFormsToReleaseOnLoad = new HashSet<int>();
            m_RecycleQueue = new LinkedList<UIForm>();

            m_ObjectPoolManager = FrameworkEntry.Instance.GetManager<ObjectPoolManager>();
            m_InstancePool = m_ObjectPoolManager.CreateObjectPool<UIFormInstanceObject>(16, 60f);
            InstanceAutoReleaseInterval = 60f;

            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadUIFormSuccessCallback, LoadUIFormDependencyAssetCallback, LoadUIFormFailureCallback, LoadUIFormUpdateCallback);

            m_UIFormHelper = null;
            m_Serial = 0;

        }

        #region 生命周期
        public override void Init()
        {
            SetUIFormHelper(new DefaultUIFormHelper());
        }

        /// <summary>
        /// 界面管理器轮询
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //回收需要回收的界面
            while (m_RecycleQueue.Count > 0)
            {
                UIForm uiForm = m_RecycleQueue.First.Value;
                m_RecycleQueue.RemoveFirst();
                uiForm.OnRecycle();
                m_InstancePool.Unspawn(uiForm.Handle);
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理界面管理器
        /// </summary>
        public override void Shutdown()
        {
            CloseAllLoadedUIForms();
            m_UIGroups.Clear();
            m_UIFormsBeingLoaded.Clear();
            m_UIFormAssetNamesBeingLoaded.Clear();
            m_UIFormsToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }
        #endregion

        /// <summary>
        /// 设置界面辅助器
        /// </summary>
        /// <param name="uiFormHelper">界面辅助器</param>
        public void SetUIFormHelper(UIFormHelperBase uiFormHelper)
        {
            if (uiFormHelper == null)
            {
                Debug.LogError("要设置的界面辅助器为空");
            }

            m_UIFormHelper = uiFormHelper;
        }

        #region 界面组相关的方法
        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>是否存在界面组</returns>
        public bool HasUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                Debug.LogError("要检查是存在的界面组名称为空");
                return false;
            }

            return m_UIGroups.ContainsKey(uiGroupName);
        }

        #region 获取界面组
        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>要获取的界面组</returns>
        public UIGroup GetUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                Debug.LogError("要获取的界面组名称为空");
                return null;
            }

            UIGroup uiGroup = null;
            if (m_UIGroups.TryGetValue(uiGroupName, out uiGroup))
            {
                return uiGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <returns>所有界面组</returns>
        public UIGroup[] GetAllUIGroups()
        {
            int index = 0;
            UIGroup[] uiGroups = new UIGroup[m_UIGroups.Count];
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroups[index++] = uiGroup.Value;
            }

            return uiGroups;
        }
        #endregion

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <returns>是否增加界面组成功</returns>
        public bool AddUIGroup(string uiGroupName, int uiGroupDepth = 0)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                Debug.LogError("要增加的界面组名称为空");
                return false;
            }

            if (HasUIGroup(uiGroupName))
            {
                Debug.LogError("要增加界面组已存在");
                return false;
            }

            //创建界面组辅助器
            UIGroupHelperBase uiGroupHelper = new GameObject().AddComponent<UGuiGroupHelper>();
            uiGroupHelper.name = string.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            uiGroupHelper.transform.SetParent(FrameworkEntry.Instance.transform);

            //将界面组放入字典
            m_UIGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, uiGroupHelper));

            return true;
        }
        #endregion

        #region 界面相关的方法

        #region 检查界面
        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUIForm(serialId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("要检查是否存在的界面的资源名称为空");
                return false;
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUIForm(uiFormAssetName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(int serialId)
        {
            return m_UIFormsBeingLoaded.Contains(serialId);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("要检查是否正在加载的界面的资源名称为空");
                return false;
            }

            return m_UIFormAssetNamesBeingLoaded.Contains(uiFormAssetName);
        }

        /// <summary>
        /// 是否是合法的界面
        /// </summary>
        /// <param name="uiForm">界面</param>
        /// <returns>界面是否合法</returns>
        public bool IsValidUIForm(UIForm uiForm)
        {
            if (uiForm == null)
            {
                return false;
            }

            return HasUIForm(uiForm.SerialId);
        }
        #endregion

        #region 获取界面
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(int serialId)
        {
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                UIForm uiForm = uiGroup.Value.GetUIForm(serialId);
                if (uiForm != null)
                {
                    return uiForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("要获取的界面的界面资源名称为空");
                return null;
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                UIForm uiForm = uiGroup.Value.GetUIForm(uiFormAssetName);
                if (uiForm != null)
                {
                    return uiForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIForm[] GetUIForms(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("要获取的界面的界面资源名称为空");
                return null;
            }

            List<UIForm> uiForms = new List<UIForm>();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiForms.AddRange(uiGroup.Value.GetUIForms(uiFormAssetName));
            }

            return uiForms.ToArray();
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <returns>所有已加载的界面。</returns>
        public UIForm[] GetAllLoadedUIForms()
        {
            List<UIForm> uiForms = new List<UIForm>();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiForms.AddRange(uiGroup.Value.GetAllUIForms());
            }

            return uiForms.ToArray();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <returns>所有正在加载界面的序列编号。</returns>
        public int[] GetAllLoadingUIFormSerialIds()
        {
            return m_UIFormsBeingLoaded.ToArray();
        }
        #endregion

        #region 打开界面
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm = false, object userData = null)
        {
            if (m_ResourceManager == null)
            {
                Debug.LogError("打开界面时资源管理器为空");
                return -1;
            }

            if (m_UIFormHelper == null)
            {
                Debug.LogError("打开界面时界面辅助器为空");
                return -1;
            }

            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                Debug.LogError("要打开的界面的资源名称为空");
                return -1;
            }

            if (string.IsNullOrEmpty(uiGroupName))
            {
                Debug.LogError("要打开的界面的界面组名称为空");
                return -1;
            }

            //界面组检查
            UIGroup uiGroup = GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                Debug.LogError(string.Format("要打开的界面的界面组：{0} 不存在", uiGroupName));
            }
            //尝试从对象池获取界面实例
            int serialId = m_Serial++;
            UIFormInstanceObject uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);
            if (uiFormInstanceObject == null)
            {
                //没获取到就加载该界面
                m_UIFormsBeingLoaded.Add(serialId);
                m_UIFormAssetNamesBeingLoaded.Add(uiFormAssetName);
                m_ResourceManager.LoadAsset(uiFormAssetName, m_LoadAssetCallbacks, new OpenUIFormInfo(serialId, uiGroup, pauseCoveredUIForm, userData));
            }
            else
            {
                OpenUIForm(serialId, uiFormAssetName, uiGroup, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData);
            }

            return serialId;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroup">界面组</param>
        /// <param name="uiFormInstance">界面实例</param>
        /// <param name="pauseCoveredUIForm">界面是否暂停或遮挡</param>
        /// <param name="isNewInstance">界面是否是新实例</param>
        /// <param name="duration">加载持续时间</param>
        /// <param name="userData">用户自定义数据</param>
        private void OpenUIForm(int serialId, string uiFormAssetName, UIGroup uiGroup, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)
        {
            try
            {
                //使用辅助器创建界面
                UIForm uiForm = m_UIFormHelper.CreateUIForm(uiFormInstance, uiGroup, userData);
                if (uiForm == null)
                {
                    throw new Exception("使用辅助器创建界面失败");
                }

                uiForm.OnInit(serialId, uiFormAssetName, uiGroup, pauseCoveredUIForm, isNewInstance, userData);
                uiGroup.AddUIForm(uiForm);
                uiForm.OnOpen(userData);
                uiGroup.Refresh();

                //派发打开界面成功事件
                OpenUIFormSuccessEventArgs se = ReferencePool.Acquire<OpenUIFormSuccessEventArgs>();
                m_EventManager.Fire(this, se.Fill(uiForm, duration, userData));
            }
            catch (Exception exception)
            {
                //派发打开界面失败事件
                OpenUIFormFailureEventArgs fe = ReferencePool.Acquire<OpenUIFormFailureEventArgs>();
                m_EventManager.Fire(this, fe.Fill(serialId, uiFormAssetName, uiGroup.Name, pauseCoveredUIForm, userData, exception.Message));
            }
        }
        #endregion

        #region 关闭界面
        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseUIForm(int serialId, object userData = null)
        {
            UIForm uiForm = GetUIForm(serialId);
            if (uiForm == null)
            {
                Debug.LogError("要关闭的界面为空");
                return;    
            }

            UIGroup uiGroup = uiForm.UIGroup;
            if (uiGroup == null)
            {
                Debug.LogError("要关闭的界面的界面组为空");
                return;
            }

            uiGroup.RemoveUIForm(uiForm);
            uiForm.OnClose(userData);
            uiGroup.Refresh();

            //派发关闭界面完成事件
            CloseUIFormCompleteEventArgs e = ReferencePool.Acquire<CloseUIFormCompleteEventArgs>();
            m_EventManager.Fire(this, e.Fill(serialId, uiForm.UIFormAssetName, userData, uiGroup));

            m_RecycleQueue.AddLast(uiForm);
        }

        /// <summary>
        /// 关闭所有已加载的界面。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseAllLoadedUIForms(object userData = null)
        {
            UIForm[] uiForms = GetAllLoadedUIForms();
            foreach (UIForm uiForm in uiForms)
            {
                CloseUIForm(uiForm.SerialId, userData);
            }
        }

        /// <summary>
        /// 关闭所有正在加载的界面。
        /// </summary>
        public void CloseAllLoadingUIForms()
        {
            foreach (int serialId in m_UIFormsBeingLoaded)
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
            }
        }
        #endregion

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIForm uiForm, object userData = null)
        {
            if (uiForm == null)
            {
                Debug.LogError("要激活的界面为空");
                return;
            }

            UIGroup uiGroup = uiForm.UIGroup;
            if (uiGroup == null)
            {
                Debug.LogError("要激活的界面的界面组为空");
                return;
            }

            uiGroup.RefocusUIForm(uiForm, userData);
            uiGroup.Refresh();
            uiForm.OnRefocus(userData);
        }


        #endregion

        #region 加载资源的4个回调方法
        private void LoadUIFormSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                Debug.LogError("打开界面的信息为空");
                return;
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);

            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                Debug.LogError(string.Format("需要释放的界面：{0} 加载成功",openUIFormInfo.SerialId.ToString()));
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                m_UIFormHelper.ReleaseUIForm(uiFormAsset, null);
                return;
            }

            //实例化界面，并将界面实例对象放入对象池
            UIFormInstanceObject uiFormInstanceObject = new UIFormInstanceObject(uiFormAssetName, uiFormAsset, m_UIFormHelper.InstantiateUIForm(uiFormAsset), m_UIFormHelper);
            m_InstancePool.Register(uiFormInstanceObject, true);

            //打开界面
            OpenUIForm(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration, openUIFormInfo.UserData);
        }

        private void LoadUIFormFailureCallback(string uiFormAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                Debug.LogError("打开界面的信息为空");
                return;
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);
            m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);

            string message = string.Format("打开界面：{0} 失败，错误信息：{1}",uiFormAssetName, errorMessage);

            //派发打开界面失败事件
            OpenUIFormFailureEventArgs e = ReferencePool.Acquire<OpenUIFormFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(openUIFormInfo.SerialId, uiFormAssetName,openUIFormInfo.UIGroup.Name,openUIFormInfo.PauseCoveredUIForm, userData, message));
        }

        private void LoadUIFormUpdateCallback(string uiFormAssetName, float progress, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                Debug.LogError("打开界面的信息为空");
                return;
            }

            //派发打开界面更新事件
            OpenUIFormUpdateEventArgs e = ReferencePool.Acquire<OpenUIFormUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, userData, progress));
        }

        private void LoadUIFormDependencyAssetCallback(string uiFormAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                Debug.LogError("打开界面的信息为空");
                return;
            }

            //派发打开界面时加载依赖资源事件
            OpenUIFormDependencyAssetEventArgs e = ReferencePool.Acquire<OpenUIFormDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, userData, dependencyAssetName, loadedCount, totalCount));
        }
        #endregion
    }
}

