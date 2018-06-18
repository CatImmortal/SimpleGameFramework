using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.DataTable
{
    /// <summary>
    /// 数据表接口
    /// </summary>
    public interface IDataTable
    {

        /// <summary>
        /// 数据表名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 数据行的类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 数据表行数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 增加数据行
        /// </summary>
        /// <param name="dataRowText"></param>
        void AddDataRow(string dataRowText);

        /// <summary>
        /// 关闭并清理数据表
        /// </summary>
        void Shutdown();
    }
}

