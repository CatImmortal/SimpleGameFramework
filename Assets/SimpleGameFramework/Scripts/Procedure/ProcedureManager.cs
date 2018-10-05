using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Fsm;
namespace SimpleGameFramework.Procedure
{
    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager : ManagerBase
    {
        #region 字段与属性
        /// <summary>
        /// 状态机管理器
        /// </summary>
        private FsmManager m_FsmManager;

        /// <summary>
        /// 流程的状态机
        /// </summary>
        private Fsm<ProcedureManager> m_ProcedureFsm;

        /// <summary>
        /// 所有流程的列表
        /// </summary>
        private List<ProcedureBase> m_procedures;

        /// <summary>
        /// 入口流程
        /// </summary>
        private ProcedureBase m_EntranceProcedure;

        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    Debug.LogError("流程状态机为空，无法获取当前流程");
                }
                return (ProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }

        public override int Priority
        {
            get
            {
                return -10;
            }
        } 
        #endregion



        public ProcedureManager()
        {
            m_FsmManager = FrameworkEntry.Instance.GetManager<FsmManager>();
            m_ProcedureFsm = null;
            m_procedures = new List<ProcedureBase>();
        }

        public override void Init()
        {

        }

        public override void Shutdown()
        {
            
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        /// <summary>
        /// 添加流程
        /// </summary>
        public void AddProcedure(ProcedureBase procedure)
        {
            if (procedure == null)
            {
                Debug.LogError("要添加的流程为空");
                return;
            }
            m_procedures.Add(procedure);
        }

        /// <summary>
        /// 设置入口流程
        /// </summary>
        /// <param name="procedure"></param>
        public void SetEntranceProcedure(ProcedureBase procedure)
        {
            m_EntranceProcedure = procedure;
        }

        /// <summary>
        /// 创建流程状态机
        /// </summary>
        public void CreateProceduresFsm()
        {
            m_ProcedureFsm = m_FsmManager.CreateFsm(this,"", m_procedures.ToArray());

            if (m_EntranceProcedure == null)
            {
                Debug.LogError("入口流程为空，无法开始流程");
                return;
            }

            //开始流程
            m_ProcedureFsm.Start(m_EntranceProcedure.GetType());
        }

    }
}

