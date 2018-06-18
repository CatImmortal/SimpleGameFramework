using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework
{
    /// <summary>
    /// 任务池
    /// </summary>
    /// <typeparam name="T">任务类型</typeparam>
    public class TaskPool<T> where T : ITask
    {
        #region 字段与属性
        /// <summary>
        /// 可用任务代理
        /// </summary>
        private  Stack<ITaskAgent<T>> m_FreeAgents;

        /// <summary>
        /// 工作中任务代理
        /// </summary>
        private  LinkedList<ITaskAgent<T>> m_WorkingAgents;

        /// <summary>
        /// 等待中任务
        /// </summary>
        private LinkedList<T> m_WaitingTasks;


        /// <summary>
        /// 可用任务代理数量
        /// </summary>
        public int FreeAgentCount { get { return m_FreeAgents.Count; } }

        /// <summary>
        /// 工作中任务代理数量
        /// </summary>
        public int WorkingAgentCount { get { return m_WorkingAgents.Count; } }

        /// <summary>
        /// 任务代理总数量
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return FreeAgentCount + WorkingAgentCount;
            }
        }

        /// <summary>
        /// 等待中任务数量
        /// </summary>
        public int WaitingTaskCount { get { return m_WaitingTasks.Count; } }
        #endregion


        public TaskPool()
        {
            m_FreeAgents = new Stack<ITaskAgent<T>>();
            m_WorkingAgents = new LinkedList<ITaskAgent<T>>();
            m_WaitingTasks = new LinkedList<T>();
        }

        #region 任务与代理的增加与移除

        /// <summary>
        /// 增加任务代理
        /// </summary>
        /// <param name="agent">要增加的任务代理</param>
        public void AddAgent(ITaskAgent<T> agent)
        {
            if (agent == null)
            {
                Debug.LogError("要增加的任务代理为空");
            }

            agent.Initialize();
            m_FreeAgents.Push(agent);
        }

        /// <summary>
        /// 增加任务
        /// </summary>
        /// <param name="task">要增加的任务</param>
        public void AddTask(T task)
        {
            m_WaitingTasks.AddLast(task);
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="serialId">要移除任务的序列编号</param>
        /// <returns>被移除的任务</returns>
        public T RemoveTask(int serialId)
        {
            foreach (T waitingTask in m_WaitingTasks)
            {
                if (waitingTask.SerialId == serialId)
                {
                    m_WaitingTasks.Remove(waitingTask);
                    return waitingTask;
                }
            }

            //连同工作中的任务代理一起移除
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                if (workingAgent.Task.SerialId == serialId)
                {
                    workingAgent.Reset();
                    m_FreeAgents.Push(workingAgent);
                    m_WorkingAgents.Remove(workingAgent);
                    return workingAgent.Task;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 移除所有任务
        /// </summary>
        public void RemoveAllTasks()
        {
            m_WaitingTasks.Clear();
            //重置所有工作中任务代理
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                workingAgent.Reset();
                m_FreeAgents.Push(workingAgent);
            }
            m_WorkingAgents.Clear();
        }

        #endregion

        #region 任务处理
        /// <summary>
        /// 任务池轮询
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            //获取第一个工作中任务代理
            LinkedListNode<ITaskAgent<T>> current = m_WorkingAgents.First;
            while (current != null)
            {
                //如果当前工作中任务代理已完成任务
                if (current.Value.Task.Done)
                {
                    //就让它重置并从工作中任务代理中移除
                    LinkedListNode<ITaskAgent<T>> next = current.Next;
                    current.Value.Reset();
                    m_FreeAgents.Push(current.Value);
                    m_WorkingAgents.Remove(current);
                    current = next;
                    continue;
                }

                //未完成就轮询任务代理
                current.Value.Update(elapseSeconds, realElapseSeconds);
                current = current.Next;
            }

            //有可用任务代理并且有等待中任务
            while (FreeAgentCount > 0 && WaitingTaskCount > 0)
            {
                //出栈一个任务代理
                ITaskAgent<T> agent = m_FreeAgents.Pop();
                //添加到工作中任务代理
                LinkedListNode<ITaskAgent<T>> agentNode = m_WorkingAgents.AddLast(agent);

                //获取一个等待中的任务
                T task = m_WaitingTasks.First.Value;
                m_WaitingTasks.RemoveFirst();

                //开始处理这个任务
                agent.Start(task);
                if (task.Done)
                {
                    agent.Reset();
                    m_FreeAgents.Push(agent);
                    m_WorkingAgents.Remove(agentNode);
                }
            }
        }


        /// <summary>
        /// 关闭并清理任务池
        /// </summary>
        public void Shutdown()
        {
            while (FreeAgentCount > 0)
            {
                m_FreeAgents.Pop().Shutdown();
            }

            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                workingAgent.Shutdown();
            }
            m_WorkingAgents.Clear();

            m_WaitingTasks.Clear();
        }
        #endregion


    }
}

