using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.DataTable
{
    /// <summary>
    /// 要加载数据表的信息
    /// </summary>
    public class LoadDataTableInfo
    {
        /// <summary>
        /// 数据行类型
        /// </summary>
        public Type DataRowType { get; private set; }

        /// <summary>
        /// 数据表名称（报错用）
        /// </summary>
        public string DataTableName { get; private set; }

        /// <summary>
        /// 数据表类型下的名称（区分同类型数据行的不同数据表）
        /// </summary>
        public string DataTableNameInType { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRowType">数据行类型</param>
        /// <param name="dataTableName"> 数据表名称（报错用）</param>
        /// <param name="dataTableNameInType">数据表类型下的名称（区分同类型数据行的不同数据表）</param>
        /// <param name="userData">用户自定义数据</param>
        public LoadDataTableInfo(Type dataRowType, string dataTableName, string dataTableNameInType, object userData)
        {
            DataRowType = dataRowType;
            DataTableName = dataTableName;
            DataTableNameInType = dataTableNameInType;
            UserData = userData;
        }
    }
}

