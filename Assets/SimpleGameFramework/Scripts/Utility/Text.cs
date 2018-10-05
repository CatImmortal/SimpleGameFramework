using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework
{
    public static partial class Utility
    {
        /// <summary>
        /// 字符相关的实用方法
        /// </summary>
        public static class Text
        {
            private static int position;

            /// <summary>
            /// 将文本按行切分
            /// </summary>
            /// <param name="text">要切分的文本</param>
            /// <returns>按行切分后的文本</returns>
            public static string[] SplitToLines(string text)
            {
                List<string> texts = new List<string>();
                int position = 0;
                string rowText = null;
                while ((rowText = ReadLine(text, ref position)) != null)
                {
                    texts.Add(rowText);
                }

                return texts.ToArray();
            }

            /// <summary>
            /// 读取一行文本
            /// </summary>
            /// <param name="text">要读取的文本</param>
            /// <param name="position">开始的位置</param>
            /// <returns>一行文本</returns>
            private static string ReadLine(string text,ref int position)
            {
                if (text == null)
                {
                    return null;
                }

                int length = text.Length;
                int offset = position;

                //偏移位置小于文本长度就一直循环
                while (offset < length)
                {
                    char ch = text[offset];
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            //读取从开始位置到偏移位置的偏移量长度的字符串
                            string str = text.Substring(position, offset - position);
                            //更新开始位置
                            position = offset + 1;
                            return str;
                        default:
                            //偏移自增
                            offset++;
                            break;
                    }
                }

                //全部文本里都没有读取到换行符，就全部读取出来
                if (offset > position)
                {
                    string str = text.Substring(position, offset - position);
                    position = offset;
                    return str;
                }

                return null;
            }

            /// <summary>
            /// 根据类型和名称获取完整名称
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="name">名称</param>
            /// <returns>完整名称</returns>
            public static string GetFullName<T>(string name)
            {
                return GetFullName(typeof(T), name);
            }

            /// <summary>
            /// 根据类型和名称获取完整名称
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="name">名称</param>
            /// <returns>完整名称</returns>
            public static string GetFullName(Type type, string name)
            {
                if (type == null)
                {
                    Debug.LogError("要获取完整名称的类型为空");
                    return null;
                }

                string typeName = type.FullName;
                return string.IsNullOrEmpty(name) ? typeName : string.Format("{0}.{1}", typeName, name);
            }
        }
    }
    
}

