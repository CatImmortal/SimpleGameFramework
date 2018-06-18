using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Resource;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.DataTable
{
    public class DataTableManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 数据表的字典
        /// </summary>
        private Dictionary<string, IDataTable> m_DataTables;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 加载资源回调方法集
        /// </summary>
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 数据表辅助器
        /// </summary>
        private DataTableHelperBase m_DataTableHelper;

        /// <summary>
        /// 获取数据表数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_DataTables.Count;
            }
        }
        #endregion

        #region 构造方法
        public DataTableManager()
        {
            m_DataTables = new Dictionary<string, IDataTable>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadDataTableSuccessCallback, LoadDataTableDependencyAssetCallback, LoadDataTableFailureCallback, LoadDataTableUpdateCallback);
            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_DataTableHelper = null;
        }
        #endregion


        public override void Init()
        {
            SetDataTableHelper(new DefaultDataTableHelper());
        }

        /// <summary>
        /// 关闭并清理数据表管理器。
        /// </summary>
        public override void Shutdown()
        {
            foreach (KeyValuePair<string, IDataTable> dataTable in m_DataTables)
            {
                dataTable.Value.Shutdown();
            }

            m_DataTables.Clear();
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
           
        }

        /// <summary>
        /// 设置数据表辅助器。
        /// </summary>
        /// <param name="dataTableHelper">数据表辅助器。</param>
        public void SetDataTableHelper(DataTableHelperBase dataTableHelper)
        {
            if (dataTableHelper == null)
            {
                Debug.LogError("要设置的数据表辅助器为空");
                return;
            }

            m_DataTableHelper = dataTableHelper;
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T">数据行类型</typeparam>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableNameInType">数据表类型下的名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableName, string dataTableAssetName, string dataTableNameInType = "", object userData = null) where T : IDataRow
        {
            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法加载数据表");
                return;
            }

            if (m_DataTableHelper == null)
            {
                Debug.LogError("数据表辅助器为空，无法加载数据表");
                return;
            }

            if (typeof(T) == null)
            {
                Debug.LogError("要加载的数据表的行类型为空，无法加载数据表");
                return;
            }

            if (string.IsNullOrEmpty(dataTableName))
            {
                Debug.LogError("要加载的数据表名称为空，无法加载数据表");
                return;
            }
            if (string.IsNullOrEmpty(dataTableAssetName))
            {
                Debug.LogError("要加载的数据表资源名称为空，无法加载数据表");
                return;
            }
            LoadDataTableInfo info = new LoadDataTableInfo(typeof(T), dataTableAssetName, dataTableNameInType, userData);
            m_ResourceManager.LoadAsset(dataTableAssetName, m_LoadAssetCallbacks, info);
        }

        


        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <typeparam name="T">数据行的类型</typeparam>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable<T>(string name = "") where T : IDataRow
        {
            return m_DataTables.ContainsKey(Utility.Text.GetFullName<T>(name));
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataRowType">数据行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable(Type dataRowType, string name = "")
        {
            if (dataRowType == null)
            {
                Debug.LogError("要查询的数据行类型为空");
                return false;
            }

            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                Debug.LogError("要查询的数据行类型不合法");
                return false;
            }

            return m_DataTables.ContainsKey(Utility.Text.GetFullName(dataRowType, name));
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <typeparam name="T">数据行的类型</typeparam>
        /// <returns>要获取的数据表</returns>
        public DataTable<T> GetDataTable<T>(string name = "") where T : class,IDataRow,new ()
        {
            IDataTable dataTable = null;
            m_DataTables.TryGetValue(Utility.Text.GetFullName<T>(name), out dataTable);
            return dataTable as DataTable<T>;
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <returns>所有数据表</returns>
        public IDataTable[] GetAllDataTables()
        {
            int index = 0;
            IDataTable[] dataTables = new IDataTable[m_DataTables.Count];
            foreach (KeyValuePair<string, IDataTable> dataTable in m_DataTables)
            {
                dataTables[index++] = dataTable.Value;
            }

            return dataTables;
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据行的类型</typeparam>
        /// <param name="text">要解析的数据表文本</param>
        /// <param name="nameInType">数据表类型下的名称</param>
        /// <returns>要创建的数据表</returns>
        public DataTable<T> CreateDataTable<T>(string text,string nameInType = "") where T : class, IDataRow, new()
        {
            
            if (HasDataTable<T>())
            {
                Debug.LogError("要创建的数据表已存在：" + Utility.Text.GetFullName<T>(nameInType));
                return null;
            }

            DataTable<T> dataTable = new DataTable<T>(nameInType);
            //分割数据行
            string[] dataRowTexts = m_DataTableHelper.SplitToDataRows(text);
            //解析数据行
            foreach (string dataRowText in dataRowTexts)
            {
                dataTable.AddDataRow(dataRowText);
            }

            m_DataTables.Add(Utility.Text.GetFullName<T>(nameInType), dataTable);
            return dataTable;
        }


        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据行的类型</param>
        /// <param name="text">要解析的数据表文本</param>
        /// <param name="nameInType">数据表类型下的名称</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable CreateDataTable(Type dataRowType, string text, string nameInType = "")
        {
            if (dataRowType == null)
            {
                Debug.LogError("数据行类型为空");
                return null;
            }

            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                Debug.LogError("数据行类型不合法");
                return null;
            }

            if (HasDataTable(dataRowType, nameInType))
            {
                Debug.LogError("数据表已存在");
                return null;
            }


            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataRowType);
            IDataTable dataTable = (IDataTable)Activator.CreateInstance(dataTableType, nameInType);
            //分割数据表文本
            string[] dataRowTexts = m_DataTableHelper.SplitToDataRows(text);
            //按行依次添加到数据表对象里
            foreach (string dataRowText in dataRowTexts)
            {
                dataTable.AddDataRow(dataRowText);
            }

            m_DataTables.Add(Utility.Text.GetFullName(dataRowType, nameInType), dataTable);
            return dataTable;
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <typeparam name="T">数据行的类型</typeparam>
        public bool DestroyDataTable<T>() where T : IDataRow
        {
            IDataTable dataTable = null;
            if (m_DataTables.TryGetValue(typeof(T).FullName,out dataTable))
            {
                dataTable.Shutdown();
                return m_DataTables.Remove(typeof(T).FullName);
            }
            return false;
        }

        #region 加载资源的4个回调方法
        private void LoadDataTableSuccessCallback(string dataTableAssetName, object dataTableAsset, float duration, object userData)
        {
            try
            {
                //这里的userData是LoadDataTableInfo对象
                if (!m_DataTableHelper.LoadDataTable(dataTableAsset, userData))
                {
                    throw new Exception("辅助器加载数据表失败："+dataTableAssetName);
                }
            }
            catch (Exception exception)
            {
                //派发加载数据表失败事件
                LoadDataTableFailureEventArgs fe = ReferencePool.Acquire<LoadDataTableFailureEventArgs>();
                m_EventManager.Fire(this, fe.Fill(userData, dataTableAssetName, exception.ToString()));
                throw;
            }
            finally
            {
                m_DataTableHelper.ReleaseDataTableAsset(dataTableAsset);
            }

            //派发加载数据表成功事件
            LoadDataTableSuccessEventArgs se = ReferencePool.Acquire<LoadDataTableSuccessEventArgs>();
            m_EventManager.Fire(this, se.Fill(userData, dataTableAssetName, duration));

        }

        private void LoadDataTableFailureCallback(string dataTableAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            //派发加载数据表失败事件
            LoadDataTableFailureEventArgs e = ReferencePool.Acquire<LoadDataTableFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, dataTableAssetName,string.Format("加载数据表{0}失败：{1}",dataTableAssetName,errorMessage)));
        }

        private void LoadDataTableUpdateCallback(string dataTableAssetName, float progress, object userData)
        {
            //派发加载数据表更新事件
            LoadDataTableUpdateEventArgs e = ReferencePool.Acquire<LoadDataTableUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, dataTableAssetName, progress));
        }

        private void LoadDataTableDependencyAssetCallback(string dataTableAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            //派发加载数据表依赖资源事件
            LoadDataTableDependencyAssetEventArgs e = ReferencePool.Acquire<LoadDataTableDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(userData, dataTableAssetName, dependencyAssetName, loadedCount, totalCount));
        }
        #endregion
    }
}

