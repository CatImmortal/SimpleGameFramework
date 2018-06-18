using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 声音组
    /// </summary>
    public class SoundGroup
    {
        #region 字段与属性
        /// <summary>
        /// 该组的所有声音代理
        /// </summary>
        private List<SoundAgent> m_SoundAgents;

        /// <summary>
        /// 是否静音
        /// </summary>
        private bool m_Mute;

        /// <summary>
        /// 音量大小
        /// </summary>
        private float m_Volume;

        /// <summary>
        /// 声音组名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 声音代理数
        /// </summary>
        public int SoundAgentCount
        {
            get
            {
                return m_SoundAgents.Count;
            }
        }

        /// <summary>
        /// 声音组中的声音是否避免被同优先级声音替换
        /// </summary>
        public bool AvoidBeingReplacedBySamePriority { get; set; }

        /// <summary>
        /// 声音组静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return m_Mute;
            }
            set
            {
                m_Mute = value;
                foreach (SoundAgent soundAgent in m_SoundAgents)
                {
                    soundAgent.RefreshMute();
                }
            }
        }

        /// <summary>
        /// 声音组音量
        /// </summary>
        public float Volume
        {
            get
            {
                return m_Volume;
            }
            set
            {
                m_Volume = value;
                foreach (SoundAgent soundAgent in m_SoundAgents)
                {
                    soundAgent.RefreshVolume();
                }
            }
        }

        /// <summary>
        /// 声音组辅助器
        /// </summary>
        public SoundGroupHelperBase Helper { get; private set; }
        #endregion

        #region 构造方法
        public SoundGroup(string name, SoundGroupHelperBase soundGroupHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("用来构造声音组对象的名称为空");
            }

            if (soundGroupHelper == null)
            {
                Debug.LogError("用来构造声音组对象的声音组辅助器为空");
            }

            Name = name;
            Helper = soundGroupHelper;
            m_SoundAgents = new List<SoundAgent>();
        }
        #endregion

        /// <summary>
        /// 增加声音代理辅助器
        /// </summary>
        public void AddSoundAgentHelper(SoundHelperBase soundHelper, SoundAgentHelperBase soundAgentHelper)
        {
            m_SoundAgents.Add(new SoundAgent(this, soundHelper, soundAgentHelper));
        }

        #region 播放声音相关的操作
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="serialId">声音的序列编号</param>
        /// <param name="soundAsset">声音资源</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="errorCode">错误码</param>
        /// <returns>用于播放的声音代理</returns>
        public SoundAgent PlaySound(int serialId, object soundAsset, PlaySoundParams playSoundParams, out PlaySoundErrorCode? errorCode)
        {
            errorCode = null;
            SoundAgent candidateAgent = null;
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (!soundAgent.IsPlaying)
                {
                    candidateAgent = soundAgent;
                    break;
                }

                //如果当前声音代理正在播放
                if (soundAgent.Priority < playSoundParams.Priority)
                {
                    if (candidateAgent == null || soundAgent.Priority < candidateAgent.Priority)
                    {
                        candidateAgent = soundAgent;
                    }
                }
                else if (!AvoidBeingReplacedBySamePriority && soundAgent.Priority == playSoundParams.Priority)
                {
                    if (candidateAgent == null || soundAgent.SetSoundAssetTime < candidateAgent.SetSoundAssetTime)
                    {
                        candidateAgent = soundAgent;
                    }
                }
            }

            if (candidateAgent == null)
            {
                errorCode = PlaySoundErrorCode.IgnoredDueToLowPriority;
                return null;
            }

            if (!candidateAgent.SetSoundAsset(soundAsset))
            {
                errorCode = PlaySoundErrorCode.SetSoundAssetFailure;
                return null;
            }

            candidateAgent.SerialId = serialId;
            candidateAgent.Time = playSoundParams.Time;
            candidateAgent.MuteInSoundGroup = playSoundParams.MuteInSoundGroup;
            candidateAgent.Loop = playSoundParams.Loop;
            candidateAgent.Priority = playSoundParams.Priority;
            candidateAgent.VolumeInSoundGroup = playSoundParams.VolumeInSoundGroup;
            candidateAgent.Pitch = playSoundParams.Pitch;
            candidateAgent.PanStereo = playSoundParams.PanStereo;
            candidateAgent.SpatialBlend = playSoundParams.SpatialBlend;
            candidateAgent.MaxDistance = playSoundParams.MaxDistance;

            //播放声音
            candidateAgent.Play(playSoundParams.FadeInSeconds);

            return candidateAgent;
        }

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId, float fadeOutSeconds)
        {
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (soundAgent.SerialId != serialId)
                {
                    continue;
                }

                soundAgent.Stop(fadeOutSeconds);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        /// <returns>是否暂停播放声音成功</returns>
        public bool PauseSound(int serialId, float fadeOutSeconds)
        {
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (soundAgent.SerialId != serialId)
                {
                    continue;
                }

                soundAgent.Pause(fadeOutSeconds);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        /// <returns>是否恢复播放声音成功</returns>
        public bool ResumeSound(int serialId, float fadeInSeconds)
        {
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (soundAgent.SerialId != serialId)
                {
                    continue;
                }

                soundAgent.Resume(fadeInSeconds);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        public void StopAllLoadedSounds()
        {
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (soundAgent.IsPlaying)
                {
                    soundAgent.Stop();
                }
            }
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void StopAllLoadedSounds(float fadeOutSeconds)
        {
            foreach (SoundAgent soundAgent in m_SoundAgents)
            {
                if (soundAgent.IsPlaying)
                {
                    soundAgent.Stop(fadeOutSeconds);
                }
            }
        }
        #endregion
    }
}

