using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Event;
using System;

namespace SimpleGameFramework.DataTable{

    /// <summary>
    /// 加载数据表失败事件
    /// </summary>
    public class LoadDataTableFailureEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 加载数据表失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDataTableFailureEventArgs).GetHashCode();

        /// <summary>
        /// 加载数据表失败事件编号
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
        /// 数据表资源名称
        /// </summary>
        public string DataTableAssetName { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载数据表失败事件
        /// </summary>
        public override void Clear()
        {
            DataRowType = default(Type);
            DataTableName = default(string);
            DataTableAssetName = default(string);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载数据表失败事件
        /// </summary>
        /// <returns>加载数据表失败事件</returns>
        public LoadDataTableFailureEventArgs Fill(object userData,string dataTableAssetName,string errorMessage)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            DataRowType = loadDataTableInfo.DataRowType;
            DataTableName = loadDataTableInfo.DataTableName;
            DataTableAssetName = dataTableAssetName;
            ErrorMessage = errorMessage;
            UserData = loadDataTableInfo.UserData;

            return this;
        }
    }
}

