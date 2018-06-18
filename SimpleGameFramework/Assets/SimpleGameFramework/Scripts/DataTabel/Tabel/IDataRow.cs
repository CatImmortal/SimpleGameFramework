using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.DataTable
{
    /// <summary>
    /// 数据行接口
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 数据行的编号
        /// </summary>
         int Id { get; }

        /// <summary>
        /// 数据行文本内容解析器
        /// </summary>
        /// <param name="dataRowText">要解析的文本内容</param>
        void ParseDataRow(string dataRowText);
    }

}

