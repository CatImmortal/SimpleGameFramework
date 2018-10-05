using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.Event;
using SimpleGameFramework.Resource;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;

namespace SimpleGameFramework.Sound
{
    /// <summary>
    /// 声音管理器
    /// </summary>
    public class SoundManager : ManagerBase
    {

        #region 字段与属性
        /// <summary>
        /// 所有声音组的字典
        /// </summary>
        private Dictionary<string, SoundGroup> m_SoundGroups;

        /// <summary>
        /// 正在加载的声音
        /// </summary>
        private List<int> m_SoundsBeingLoaded;

        /// <summary>
        /// 需要在加载后释放的声音
        /// </summary>
        private HashSet<int> m_SoundsToReleaseOnLoad;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 资源加载回调方法集
        /// </summary>
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        /// <summary>
        /// 声音辅助器
        /// </summary>
        private SoundHelperBase m_SoundHelper;

        /// <summary>
        /// 序列编号
        /// </summary>
        private int m_Serial;

        /// <summary>
        /// 声音监听器
        /// </summary>
        private AudioListener m_AudioListener;

        /// <summary>
        /// 混音组
        /// </summary>
        private AudioMixer m_AudioMixer;

        /// <summary>
        /// 声音组数量
        /// </summary>
        public int SoundGroupCount
        {
            get
            {
                return m_SoundGroups.Count;
            }
        }
        #endregion

        public SoundManager()
        {
            m_SoundGroups = new Dictionary<string, SoundGroup>();
            m_SoundsBeingLoaded = new List<int>();
            m_SoundsToReleaseOnLoad = new HashSet<int>();

            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadSoundSuccessCallback, LoadSoundDependencyAssetCallback, LoadSoundFailureCallback, LoadSoundUpdateCallback);

            m_EventManager = FrameworkEntry.Instance.GetManager<EventManager>();

            m_SoundHelper = null;
            m_Serial = 0;

            m_AudioListener = FrameworkEntry.Instance.gameObject.GetOrAddComponent<AudioListener>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        #region 生命周期
        public override void Init()
        {
            SetSoundHelper(new DefaultSoundHelper());
        }

        /// <summary>
        /// 关闭并清理声音管理器
        /// </summary>
        public override void Shutdown()
        {
            StopAllLoadedSounds();
            m_SoundGroups.Clear();
            m_SoundsBeingLoaded.Clear();
            m_SoundsToReleaseOnLoad.Clear();
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        #endregion

        /// <summary>
        /// 设置声音辅助器
        /// </summary>
        /// <param name="soundHelper">声音辅助器。</param>
        public void SetSoundHelper(SoundHelperBase soundHelper)
        {
            if (soundHelper == null)
            {
                Debug.LogError("要设置的声音辅助器为空");
                
            }
            m_SoundHelper = soundHelper;
        }

        #region 声音组相关方法
        /// <summary>
        /// 是否存在指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>指定声音组是否存在</returns>
        public bool HasSoundGroup(string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Debug.LogError("要检查是否存在的声音组名称为空");
                return false;
            }

            return m_SoundGroups.ContainsKey(soundGroupName);
        }

        /// <summary>
        /// 获取指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>要获取的声音组</returns>
        public SoundGroup GetSoundGroup(string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Debug.LogError("要获取的声音组名称为空");
                return null;
            }

            SoundGroup soundGroup = null;
            if (m_SoundGroups.TryGetValue(soundGroupName, out soundGroup))
            {
                return soundGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有声音组。
        /// </summary>
        /// <returns>所有声音组。</returns>
        public SoundGroup[] GetAllSoundGroups()
        {
            int index = 0;
            SoundGroup[] soundGroups = new SoundGroup[m_SoundGroups.Count];
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                soundGroups[index++] = soundGroup.Value;
            }

            return soundGroups;
        }

        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundAgentHelperCount">声音代理辅助器数量</param>
        /// <param name="soundGroupAvoidBeingReplacedBySamePriority">声音组中的声音是否避免被同优先级声音替换</param>
        /// <param name="soundGroupMute">声音组是否静音</param>
        /// <param name="soundGroupVolume">声音组音量</param>
        /// <returns>是否增加声音组成功</returns>
        public bool AddSoundGroup(string soundGroupName, int soundAgentHelperCount, bool soundGroupAvoidBeingReplacedBySamePriority = false, bool soundGroupMute = false, float soundGroupVolume = 1f)
        {
            if (HasSoundGroup(soundGroupName))
            {
                Debug.LogError("要增加的声音组已存在：" + soundGroupName);
                return false;
            }

            if (string.IsNullOrEmpty(soundGroupName))
            {
                Debug.LogError("要增加的声音组名称为空"); 
                return false;
            }

            //创建声音组辅助器
            SoundGroupHelperBase soundGroupHelper = new GameObject().AddComponent<DefaultSoundGroupHelper>();
            soundGroupHelper.name = string.Format("Sound Group - {0}", soundGroupName);
            soundGroupHelper.transform.SetParent(FrameworkEntry.Instance.transform);

            //设置声音组辅助器的混音组
            if (m_AudioMixer != null)
            {
                AudioMixerGroup[] audioMixerGroups = m_AudioMixer.FindMatchingGroups(string.Format("Master/{0}", soundGroupName));
                if (audioMixerGroups.Length > 0)
                {
                    soundGroupHelper.AudioMixerGroup = audioMixerGroups[0];
                }
                else
                {
                    soundGroupHelper.AudioMixerGroup = m_AudioMixer.FindMatchingGroups("Master")[0];
                }
            }

            //创建声音组
            SoundGroup soundGroup = new SoundGroup(soundGroupName, soundGroupHelper)
            {
                AvoidBeingReplacedBySamePriority = soundGroupAvoidBeingReplacedBySamePriority,
                Mute = soundGroupMute,
                Volume = soundGroupVolume
            };
            m_SoundGroups.Add(soundGroupName, soundGroup);

            //添加声音代理辅助器
            for (int i = 0; i < soundAgentHelperCount; i++)
            {
                if (!AddSoundAgentHelper(soundGroupName, soundGroupHelper, i))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 声音相关方法
        /// <summary>
        /// 是否正在加载声音
        /// </summary>
        /// <param name="serialId">声音序列编号</param>
        /// <returns>是否正在加载声音</returns>
        public bool IsLoadingSound(int serialId)
        {
            return m_SoundsBeingLoaded.Contains(serialId);
        }


        /// <summary>
        /// 增加声音代理辅助器
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundGroupHelper">声音组辅助器</param>
        /// <param name="index">声音代理辅助器索引</param>
        /// <returns>是否增加声音代理辅助器成功</returns>
        private bool AddSoundAgentHelper(string soundGroupName, SoundGroupHelperBase soundGroupHelper, int index)
        {
            if (m_SoundHelper == null)
            {
                Debug.LogError("添加声音代理辅助器时声音辅助器为空");
                return false;
            }

            //获取声音组
            SoundGroup soundGroup = GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Debug.LogError("添加声音代理辅助器时声音组为空");
                return false;
            }

            //创建声音代理辅助器
            SoundAgentHelperBase soundAgentHelper = new GameObject().AddComponent<DefaultSoundAgentHelper>();
            soundAgentHelper.name = string.Format("Sound Agent Helper - {0} - {1}", soundGroupName, index.ToString());
            soundAgentHelper.transform.SetParent(soundGroupHelper.transform);

            //设置声音代理辅助器的混音组
            if (m_AudioMixer != null)
            {
                AudioMixerGroup[] audioMixerGroups = m_AudioMixer.FindMatchingGroups(string.Format("Master/{0}/{1}", soundGroupName, index.ToString()));
                if (audioMixerGroups.Length > 0)
                {
                    soundAgentHelper.AudioMixerGroup = audioMixerGroups[0];
                }
                else
                {
                    soundAgentHelper.AudioMixerGroup = soundGroupHelper.AudioMixerGroup;
                }
            }
            
            //为声音组添加声音代理辅助器
            soundGroup.AddSoundAgentHelper(m_SoundHelper, soundAgentHelper);
            return true;
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="bindingEntity">声音绑定的实体</param>
        /// <param name="worldPosition">声音所在的世界坐标</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams = null, Entity.Entity bindingEntity = null, Vector3 worldPosition = default(Vector3), object userData = null)
        {
            if (m_ResourceManager == null)
            {
                Debug.LogError("资源管理器为空，无法播放声音");
                return -1;
            }

            if (m_SoundHelper == null)
            {
                Debug.LogError("声音辅助器为空，无法播放声音");
                return -1;
            }

            if (playSoundParams == null)
            {
                playSoundParams = new PlaySoundParams();
            }

            PlaySoundInfo playSoundInfo = new PlaySoundInfo(bindingEntity, worldPosition, userData);

            int serialId = m_Serial++;
            PlaySoundErrorCode? errorCode = null;
            string errorMessage = null;
            //获取声音组
            SoundGroup soundGroup = GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                errorCode = PlaySoundErrorCode.SoundGroupNotExist;
                errorMessage = string.Format("声音组：{0} 不存在.", soundGroupName);
            }
            else if (soundGroup.SoundAgentCount <= 0)
            {
                errorCode = PlaySoundErrorCode.SoundGroupHasNoAgent;
                errorMessage = string.Format("声音组： {0} 没有声音代理", soundGroupName);
            }

            //错误码有值时
            if (errorCode.HasValue)
            {

                //派发播放声音失败事件
                PlaySoundFailureEventArgs e = ReferencePool.Acquire<PlaySoundFailureEventArgs>();
                m_EventManager.Fire(this, e.Fill(playSoundInfo, serialId, soundAssetName, soundGroupName,playSoundParams,errorCode.Value,errorMessage));
                Debug.LogError("播放声音失败：" + errorMessage);
                return serialId;
            }

            //加载声音
            m_SoundsBeingLoaded.Add(serialId);
            m_ResourceManager.LoadAsset(soundAssetName, m_LoadAssetCallbacks, new LoadSoundInfo(serialId,soundGroup, playSoundParams, playSoundInfo));

            return serialId;
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void PauseSound(int serialId, float fadeOutSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.PauseSound(serialId, fadeOutSeconds))
                {
                    return;
                }
            }

            Debug.LogError("没找到要暂停的声音：" + serialId);

        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void ResumeSound(int serialId, float fadeInSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.ResumeSound(serialId, fadeInSeconds))
                {
                    return;
                }
            }

            Debug.LogError("没找到要恢复的声音：" + serialId);
        }

        

        #region 停止播放声音
        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId, float fadeOutSeconds = Constant.DefaultFadeOutSeconds)
        {
            if (IsLoadingSound(serialId))
            {
                m_SoundsToReleaseOnLoad.Add(serialId);
                return true;
            }

            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.StopSound(serialId, fadeOutSeconds))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void StopAllLoadedSounds(float fadeOutSeconds = Constant.DefaultFadeOutSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                soundGroup.Value.StopAllLoadedSounds(fadeOutSeconds);
            }
        }

        /// <summary>
        /// 停止所有正在加载的声音
        /// </summary>
        public void StopAllLoadingSounds()
        {
            foreach (int serialId in m_SoundsBeingLoaded)
            {
                m_SoundsToReleaseOnLoad.Add(serialId);
            }
        }
        #endregion


        #endregion

        #region 加载资源的4个回调方法
        private void LoadSoundSuccessCallback(string soundAssetName, object soundAsset, float duration, object userData)
        {
            LoadSoundInfo loadSoundInfo = (LoadSoundInfo)userData;
            if (loadSoundInfo == null)
            {
                Debug.LogError("加载声音的信息为空");
                return;
            }

            m_SoundsBeingLoaded.Remove(loadSoundInfo.SerialId);

            //释放需要在加载后立即释放的声音
            if (m_SoundsToReleaseOnLoad.Contains(loadSoundInfo.SerialId))
            {
                Debug.Log("在加载成功后释放了声音：" + loadSoundInfo.SerialId.ToString());
                m_SoundsToReleaseOnLoad.Remove(loadSoundInfo.SerialId);
                m_SoundHelper.ReleaseSoundAsset(soundAsset);
                return;
            }

            //播放声音
            PlaySoundErrorCode? errorCode = null;
            SoundAgent soundAgent = loadSoundInfo.SoundGroup.PlaySound(loadSoundInfo.SerialId, soundAsset, loadSoundInfo.PlaySoundParams, out errorCode);

            if (soundAgent != null)
            {
                
                //获取到声音代理辅助器，并设置绑定的实体或位置
                PlaySoundInfo playSoundInfo = (PlaySoundInfo)loadSoundInfo.UserData;
                SoundAgentHelperBase soundAgentHelper = soundAgent.Helper;
                if (playSoundInfo.BindingEntity != null)
                {
                    soundAgentHelper.SetBindingEntity(playSoundInfo.BindingEntity);
                }
                else
                {
                    soundAgentHelper.SetWorldPosition(playSoundInfo.WorldPosition);
                }

                //派发播放声音成功事件
                PlaySoundSuccessEventArgs se = ReferencePool.Acquire<PlaySoundSuccessEventArgs>();
                m_EventManager.Fire(this, se.Fill(loadSoundInfo.UserData, loadSoundInfo.SerialId, soundAssetName, soundAgent, duration));
            }
            else
            {
                m_SoundsToReleaseOnLoad.Remove(loadSoundInfo.SerialId);
                m_SoundHelper.ReleaseSoundAsset(soundAsset);
                string errorMessage = string.Format("声音组： {0} 播放声音 '{1}' 失败.", loadSoundInfo.SoundGroup.Name, soundAssetName);

                //派发播放声音失败事件
                PlaySoundFailureEventArgs fe = ReferencePool.Acquire<PlaySoundFailureEventArgs>();
                m_EventManager.Fire(this, fe.Fill(loadSoundInfo.UserData, loadSoundInfo.SerialId, soundAssetName, loadSoundInfo.SoundGroup.Name, loadSoundInfo.PlaySoundParams, errorCode.Value, errorMessage));
                Debug.LogError("播放声音失败：" + errorMessage);
            }
        }

        private void LoadSoundFailureCallback(string soundAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            LoadSoundInfo loadSoundInfo = (LoadSoundInfo)userData;
            if (loadSoundInfo == null)
            {
                Debug.LogError("加载声音的信息为空");
                return;
            }
            m_SoundsBeingLoaded.Remove(loadSoundInfo.SerialId);
            m_SoundsToReleaseOnLoad.Remove(loadSoundInfo.SerialId);

            string appendErrorMessage = string.Format("加载声音失败,资源名： {0}, 状态： {1}, 错误信息：{2}", soundAssetName, status.ToString(), errorMessage);

            //派发播放声音失败事件
            PlaySoundFailureEventArgs e = ReferencePool.Acquire<PlaySoundFailureEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadSoundInfo.UserData, loadSoundInfo.SerialId, soundAssetName, loadSoundInfo.SoundGroup.Name, loadSoundInfo.PlaySoundParams, PlaySoundErrorCode.LoadAssetFailure, appendErrorMessage));
        }


        private void LoadSoundUpdateCallback(string soundAssetName, float progress, object userData)
        {
            LoadSoundInfo loadSoundInfo = (LoadSoundInfo)userData;
            if (loadSoundInfo == null)
            {
                Debug.LogError("加载声音的信息为空");
                return;
            }

            //派发加载声音更新事件
            PlaySoundUpdateEventArgs e = ReferencePool.Acquire<PlaySoundUpdateEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadSoundInfo.UserData, loadSoundInfo.SerialId, soundAssetName, loadSoundInfo.SoundGroup.Name, loadSoundInfo.PlaySoundParams, progress));
        }

        private void LoadSoundDependencyAssetCallback(string soundAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            LoadSoundInfo loadSoundInfo = (LoadSoundInfo)userData;
            if (loadSoundInfo == null)
            {
                Debug.LogError("加载声音的信息为空");
                return;
            }

            //派发加载声音时加载依赖资源事件
            PlaySoundDependencyAssetEventArgs e = ReferencePool.Acquire<PlaySoundDependencyAssetEventArgs>();
            m_EventManager.Fire(this, e.Fill(loadSoundInfo.UserData, loadSoundInfo.SerialId, soundAssetName, loadSoundInfo.SoundGroup.Name, loadSoundInfo.PlaySoundParams,dependencyAssetName,loadedCount,totalCount));
        }

        #endregion

        /// <summary>
        /// 刷新声音监听器
        /// </summary>
        private void RefreshAudioListener()
        {
            m_AudioListener.enabled = UnityEngine.Object.FindObjectsOfType<AudioListener>().Length <= 1;
        }

        #region 场景加载与卸载的回调方法
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
        {
            RefreshAudioListener();
        }

        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            RefreshAudioListener();
        }
        #endregion
    }
}

