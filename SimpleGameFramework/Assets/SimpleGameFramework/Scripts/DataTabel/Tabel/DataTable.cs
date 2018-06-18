using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.DataTable
{
    /// <summary>
    /// 数据表
    /// </summary>
    /// <typeparam name="T">数据行的类型</typeparam>
    public class DataTable<T> : IDataTable,IEnumerable<T> where T: class, IDataRow,new()
    {

        #region 字段与属性
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 数据表内所有数据行
        /// </summary>
        private Dictionary<int, T> m_DataSet;

        /// <summary>
        /// 最小ID的数据行
        /// </summary>
        public T MinIdDataRow { get; private set; }

        /// <summary>
        /// 最大ID的数据行
        /// </summary>
        public T MaxIdDataRow { get; private set; }

        /// <summary>
        /// 数据行的类型
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 数据行数
        /// </summary>
        public int Count
        {
            get
            {
                return m_DataSet.Count;
            }
        }

        /// <summary>
        /// 获取数据行
        /// </summary>
        public T this[int id]
        {
            get
            {
                T dataRow = null;
                m_DataSet.TryGetValue(id, out dataRow);
                return dataRow;
            }
        }
        #endregion


        #region 构造方法
        public DataTable(string name)
        {
            Name = name;
            m_DataSet = new Dictionary<int, T>();
            MinIdDataRow = null;
            MaxIdDataRow = null;
        }
        #endregion

        #region 检查数据行
        /// <summary>
        /// 检查是否存在数据行
        /// </summary>
        /// <param name="id">数据行的编号</param>
        /// <returns>是否存在数据行</returns>
        public bool HasDataRow(int id)
        {
            return m_DataSet.ContainsKey(id);
        }

        /// <summary>
        /// 检查是否存在数据行
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>是否存在数据行</returns>
        public bool HasDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                Debug.LogError("检查数据行是否存在的条件为空");
            }

            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                if (condition(dataRow.Value))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 获取数据行
        /// <summary>
        /// 获取符合条件的数据行
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>符合条件的数据表行</returns>
        /// <remarks>当存在多个符合条件的数据表行时，仅返回第一个符合条件的数据表行</remarks>
        public T GetDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                Debug.LogError("获取数据行的条件为空");
            }

            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                T dr = dataRow.Value;
                if (condition(dr))
                {
                    return dr;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有数据行
        /// </summary>
        /// <returns>所有数据行</returns>
        public T[] GetAllDataRows()
        {
            int index = 0;
            T[] allDataRows = new T[m_DataSet.Count];
            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                allDataRows[index++] = dataRow.Value;
            }
            return allDataRows;
        }

        /// <summary>
        /// 获取所有符合条件的数据行
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>所有符合条件的数据行</returns>
        public T[] GetAllDataRows(Predicate<T> condition)
        {
            if (condition == null)
            {
                Debug.LogError("获取数据行的条件为空");
            }

            List<T> results = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                T dr = dataRow.Value;
                if (condition(dr))
                {
                    results.Add(dr);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取所有排序后的数据行
        /// </summary>
        /// <param name="comparison">要排序的条件</param>
        /// <returns>所有排序后的数据表行</returns>
        public T[] GetAllDataRows(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                Debug.LogError("获取数据行的排序方法为空");
            }

            List<T> allDataRows = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                allDataRows.Add(dataRow.Value);
            }

            allDataRows.Sort(comparison);
            return allDataRows.ToArray();
        }

        /// <summary>
        /// 获取所有排序后的符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <param name="comparison">要排序的条件。</param>
        /// <returns>所有排序后的符合条件的数据表行。</returns>
        public T[] GetAllDataRows(Predicate<T> condition, Comparison<T> comparison)
        {
            if (condition == null)
            {
                Debug.LogError("获取数据行的条件为空");
            }

            if (comparison == null)
            {
                Debug.LogError("获取数据行的排序方法为空");
            }

            List<T> results = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in m_DataSet)
            {
                T dr = dataRow.Value;
                if (condition(dr))
                {
                    results.Add(dr);
                }
            }

            results.Sort(comparison);
            return results.ToArray();
        }
        #endregion

        /// <summary>
        /// 返回一个循环访问数据表的枚举器
        /// </summary>
        /// <returns>可用于循环访问数据表的对象</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_DataSet.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_DataSet.Values.GetEnumerator();
        }

        /// <summary>
        /// 增加数据行
        /// </summary>
        /// <param name="dataRowText">要解析的数据行文本</param>
        public void AddDataRow(string dataRowText)
        {
            //创建一个数据行对象
            T dataRow = new T();

            try
            {
                //解析数据行文本
                dataRow.ParseDataRow(dataRowText);
            }
            catch (Exception e)
            {
                Debug.LogError("解析数据行文本时出现异常：" + e.Message);
            }

            if (HasDataRow(dataRow.Id))
            {
                Debug.LogError("解析的数据行已存在：" + dataRow.Id);
            }

            m_DataSet.Add(dataRow.Id, dataRow);

            //设置最大ID行与最小ID行
            if (MinIdDataRow == null || MinIdDataRow.Id > dataRow.Id)
            {
                MinIdDataRow = dataRow;
            }

            if (MaxIdDataRow == null || MaxIdDataRow.Id < dataRow.Id)
            {
                MaxIdDataRow = dataRow;
            }
        }

        /// <summary>
        /// 关闭并清理数据表
        /// </summary>
        public void Shutdown()
        {
            m_DataSet.Clear();
        }

     
    }
}

