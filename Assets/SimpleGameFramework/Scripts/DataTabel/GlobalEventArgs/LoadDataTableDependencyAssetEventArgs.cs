using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.DataTable
{
    /// <summary>
    /// 加载数据表时加载依赖资源事件
    /// </summary>
    public class LoadDataTableDependencyAssetEventArgs : GlobalEventArgs
    {
        /// <summary>
        ///加载数据表时加载依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDataTableDependencyAssetEventArgs).GetHashCode();
        
        /// <summary>
        ///加载数据表时加载依赖资源事件编号
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 数据行的类型
        /// </summary>
        public Type DataRowType { get; private set; }

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string DataTableName { get; private set; }

        /// <summary>
        /// 获取数据表资源名称
        /// </summary>
        public string DataTableAssetName { get; private set; }


        /// <summary>
        /// 被加载的依赖资源名称
        /// </summary>
        public string DependencyAssetName { get; private set; }


        /// <summary>
        /// 当前已加载依赖资源数量。
        /// </summary>
        public int LoadedCount { get; private set; }

        /// <summary>
        /// 总共加载依赖资源数量
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载数据表时加载依赖资源事件。
        /// </summary>
        public override void Clear()
        {
            DataRowType = default(Type);
            DataTableName = default(string);
            DataTableAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载数据表时加载依赖资源事件
        /// </summary>
        /// <returns>加载数据表时加载依赖资源事件</returns>
        public LoadDataTableDependencyAssetEventArgs Fill(object userData, string dataTableAssetName, string dependencyAssetName, int loadedCount, int totalCount)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            DataRowType = loadDataTableInfo.DataRowType;
            DataTableName = loadDataTableInfo.DataTableName;
            DataTableAssetName = dataTableAssetName;
            DependencyAssetName = dependencyAssetName;
            LoadedCount = loadedCount;
            TotalCount = totalCount;
            UserData = loadDataTableInfo.UserData;

            return this;
        }
    }
}

