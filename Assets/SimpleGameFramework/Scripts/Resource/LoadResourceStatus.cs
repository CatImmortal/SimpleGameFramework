using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Resource
{
    /// <summary>
    /// 加载资源状态
    /// </summary>
    public enum LoadResourceStatus
    {
        /// <summary>
        /// 加载资源完成
        /// </summary>
        Ok = 0,

        /// <summary>
        /// 资源尚未准备完毕
        /// </summary>
        NotReady,

        /// <summary>
        /// 资源不存在于磁盘上
        /// </summary>
        NotExist,

        /// <summary>
        /// 依赖资源错误
        /// </summary>
        DependencyError,

        /// <summary>
        /// 资源类型错误
        /// </summary>
        TypeError,

        /// <summary>
        /// 加载子资源错误
        /// </summary>
        ChildAssetError,

        /// <summary>
        /// 加载场景资源错误
        /// </summary>
        SceneAssetError,
    }
}

