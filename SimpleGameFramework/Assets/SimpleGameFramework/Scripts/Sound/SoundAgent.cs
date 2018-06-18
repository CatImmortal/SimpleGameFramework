using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 声音代理
    /// </summary>
    public class SoundAgent
    {
        #region 字段与属性
        /// <summary>
        /// 声音代理所在的声音组
        /// </summary>
        private SoundGroup m_SoundGroup;

        /// <summary>
        /// 声音辅助器
        /// </summary>
        private SoundHelperBase m_SoundHelper;

        /// <summary>
        /// 声音代理辅助器
        /// </summary>
        public SoundAgentHelperBase Helper { get; private set; }

        /// <summary>
        /// 序列编号
        /// </summary>
        public int SerialId { get; set; }

        /// <summary>
        /// 声音资源
        /// </summary>
        private object m_SoundAsset;

        /// <summary>
        /// 声音创建时间
        /// </summary>
        public DateTime SetSoundAssetTime { get; private set; }

        /// <summary>
        /// 当前是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return Helper.IsPlaying;
            }
        }

        private bool m_MuteInSoundGroup;

        private float m_VolumeInSoundGroup;

        /// <summary>
        /// 播放位置
        /// </summary>
        public float Time
        {
            get
            {
                return Helper.Time;
            }
            set
            {
                Helper.Time = value;
            }
        }

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return Helper.Mute;
            }
        }

        /// <summary>
        /// 置在声音组内是否静音
        /// </summary>
        public bool MuteInSoundGroup
        {
            get
            {
                return m_MuteInSoundGroup;
            }
            set
            {
                m_MuteInSoundGroup = value;
                RefreshMute();
            }
        }

        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Loop
        {
            get
            {
                return Helper.Loop;
            }
            set
            {
                Helper.Loop = value;
            }
        }

        /// <summary>
        /// 声音优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return Helper.Priority;
            }
            set
            {
                Helper.Priority = value;
            }
        }

        /// <summary>
        /// 音量大小
        /// </summary>
        public float Volume
        {
            get
            {
                return Helper.Volume;
            }
        }

        /// <summary>
        /// 在声音组内音量大小
        /// </summary>
        public float VolumeInSoundGroup
        {
            get
            {
                return m_VolumeInSoundGroup;
            }
            set
            {
                m_VolumeInSoundGroup = value;
                RefreshVolume();
            }
        }

        /// <summary>
        /// 声音音调
        /// </summary>
        public float Pitch
        {
            get
            {
                return Helper.Pitch;
            }
            set
            {
                Helper.Pitch = value;
            }
        }

        /// <summary>
        /// 声音立体声声相
        /// </summary>
        public float PanStereo
        {
            get
            {
                return Helper.PanStereo;
            }
            set
            {
                Helper.PanStereo = value;
            }
        }

        /// <summary>
        /// 声音空间混合量
        /// </summary>
        public float SpatialBlend
        {
            get
            {
                return Helper.SpatialBlend;
            }
            set
            {
                Helper.SpatialBlend = value;
            }
        }

        /// <summary>
        /// 声音最大距离
        /// </summary>
        public float MaxDistance
        {
            get
            {
                return Helper.MaxDistance;
            }
            set
            {
                Helper.MaxDistance = value;
            }
        }
        #endregion

        #region 构造方法
        public SoundAgent(SoundGroup soundGroup, SoundHelperBase soundHelper, SoundAgentHelperBase soundAgentHelper)
        {
            if (soundGroup == null)
            {
                Debug.LogError("用来构造声音代理对象的声音组为空");
            }

            if (soundHelper == null)
            {
                Debug.LogError("用来构造声音代理对象的声音辅助器为空");
            }

            if (soundAgentHelper == null)
            {
                Debug.LogError("用来构造声音代理对象的声音代理辅助器为空");
            }

            m_SoundGroup = soundGroup;
            m_SoundHelper = soundHelper;
            Helper = soundAgentHelper;
            Helper.resetSoundAgentEventHandler += OnResetSoundAgent;
            SerialId = 0;
            Reset();
        }
        #endregion



        #region 播放声音的相关操作

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void Play(float fadeInSeconds  = Constant.DefaultFadeInSeconds)
        {
           Helper.Play(fadeInSeconds);
        }

        /// <summary>
        /// 停止播放声音。
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void Stop(float fadeOutSeconds = Constant.DefaultFadeOutSeconds)
        {
            Helper.Stop(fadeOutSeconds);
        }

        /// <summary>
        /// 暂停播放声音。
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
        public void Pause(float fadeOutSeconds = Constant.DefaultFadeOutSeconds)
        {
            Helper.Pause(fadeOutSeconds);
        }


        /// <summary>
        /// 恢复播放声音。
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位。</param>
        public void Resume(float fadeInSeconds = Constant.DefaultFadeInSeconds)
        {
            Helper.Resume(fadeInSeconds);
        }
        #endregion

        /// <summary>
        /// 设置声音资源
        /// </summary>
        public bool SetSoundAsset(object soundAsset)
        {
            Reset();
            m_SoundAsset = soundAsset;
            SetSoundAssetTime = DateTime.Now;
            return Helper.SetSoundAsset(soundAsset);
        }

        /// <summary>
        /// 刷新是否静音
        /// </summary>
        public void RefreshMute()
        {
            Helper.Mute = m_SoundGroup.Mute || m_MuteInSoundGroup;
        }

        /// <summary>
        /// 刷新音量大小
        /// </summary>
        public void RefreshVolume()
        {
            Helper.Volume = m_SoundGroup.Volume * m_VolumeInSoundGroup;
        }


        /// <summary>
        /// 重置声音代理
        /// </summary>
        public void Reset()
        {
            if (m_SoundAsset != null)
            {
                m_SoundHelper.ReleaseSoundAsset(m_SoundAsset);
                m_SoundAsset = null;
            }

            SetSoundAssetTime = DateTime.MinValue;
            Time = Constant.DefaultTime;
            MuteInSoundGroup = Constant.DefaultMute;
            Loop = Constant.DefaultLoop;
            Priority = Constant.DefaultPriority;
            VolumeInSoundGroup = Constant.DefaultVolume;
            Pitch = Constant.DefaultPitch;
            PanStereo = Constant.DefaultPanStereo;
            SpatialBlend = Constant.DefaultSpatialBlend;
            MaxDistance = Constant.DefaultMaxDistance;
            Helper.Reset();
        }
        /// <summary>
        /// 重置声音代理时的回调
        /// </summary>
        private void OnResetSoundAgent(object sender, ResetSoundAgentEventArgs e)
        {
            Reset();
        }
    }
}

