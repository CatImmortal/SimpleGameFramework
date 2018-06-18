using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 任务序列ID
        /// </summary>
        int SerialId { get; }

        /// <summary>
        /// 任务是否完成
        /// </summary>
        bool Done { get; }
    }
}


