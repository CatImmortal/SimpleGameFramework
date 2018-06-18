using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Resource;
namespace SimpleGameFramework.DataTable
{
    public class DefaultDataTableHelper : DataTableHelperBase
    {
        private ResourceManager m_ResourceManager = null;
        private DataTableManager m_DataTableManager = null;

        public DefaultDataTableHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_DataTableManager = FrameworkEntry.Instance.GetManager<DataTableManager>();
        }

        public override void ReleaseDataTableAsset(object dataTableAsset)
        {
            m_ResourceManager.UnloadAsset(dataTableAsset);
        }

        /// <summary>
        /// 将要解析的数据表文本分割为数据行文本
        /// </summary>
        /// <param name="text">要解析的数据表文本</param>
        /// <returns>数据行文本</returns>
        public override string[] SplitToDataRows(string text)
        {
            List<string> texts = new List<string>();
            string[] rowTexts = Utility.Text.SplitToLines(text);
            for (int i = 0; i < rowTexts.Length; i++)
            {
                //跳过#开头的注释行
                if (rowTexts[i].Length <= 0 || rowTexts[i][0] == '#')
                {
                    continue;
                }

                texts.Add(rowTexts[i]);
            }

            return texts.ToArray();
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataRowType">数据行的类型</param>
        /// <param name="dataTableName">数据表名称，用来报错</param>
        /// <param name="dataTableNameInType">数据表类型下的名称，区分同类型数据行的不同数据表</param>
        /// <param name="dataTableAsset">数据表资源</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>加载是否成功</returns>
        protected override bool LoadDataTable(Type dataRowType, string dataTableName, string dataTableNameInType, object dataTableAsset, object userData)
        {
            TextAsset textAsset = dataTableAsset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("要加载的数据表资源为空：" + dataTableName);
                return false;
            }
            if (dataRowType == null)
            {
                Debug.LogError("要加载的数据表行类型为空");
                return false;
            }

            m_DataTableManager.CreateDataTable(dataRowType, textAsset.text, dataTableNameInType);
            return true;
        }
    }
}

