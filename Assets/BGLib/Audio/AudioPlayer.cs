using BG_Library.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace BG_Library.Audio
{
    [System.Serializable]
    public class Audio
    {
        [SerializeField] AudioSource source;
        [SerializeField] Coroutine corou;
        [SerializeField] bool isFadingOut;

        public string NameClip { get => source.clip.name; }
        public AudioSource Source { get => source; set => source = value; }
        public Coroutine Corou { get => corou; set => corou = value; }
        public bool IsFadingOut { get => isFadingOut; set => isFadingOut = value; }
    }

    public class AudioPlayer : MonoBehaviour
    {
        static AudioPlayer ins;
        public static AudioPlayer Ins
        {
            get
            {
                if (ins == null)
                    Setup();

                return ins;
            }
        }

        AudioSetting setting;
        public AudioSetting Setting => setting;

        static void Setup()
        {
            ins = Instantiate(LoadSource.LoadObject<GameObject>("Audio player")
                , DDOL.Instance.transform).GetComponent<AudioPlayer>();

            ins.setting = LoadSource.LoadObject<AudioSetting>(AudioSetting.AssetName);
            ins.setting.Init();

            LoadSceneManager.BeforeExitScene += () =>
            {
                for (int i = 0; i < ins.setting.ListAudioTypes.Length; i++)
                {
                    if (ins.setting.ListAudioTypes[i].IsDestroyOnLoad)
                    {
                        ins.StopAllCurrentAudio(ins.setting.ListAudioTypes[i]);
					}
                }
            };
        }

        [SerializeField] List<Audio> listAvailableAud = new List<Audio>();

        public void Preload()
		{

		}

        public void PlayAudioClip(AudioClip clip, AudCommonConf conf, System.Action OnComplete = null)
        {
			var listAudType = setting.GetAudioType(conf.ChannelType);
			if (listAudType == null)
			{
                Debug.LogError($"AUDIO PLAYER => CANNOT FIND TYPE: {conf.ChannelType}");
				return;
			}

			if (conf.StopAll) // Xoa tat ca am thanh cua Type nay
            {
                listAvailableAud.AddRange(listAudType.ListCurrentAud);
                StopAllCurrentAudio(listAudType);
            }

            if (conf.IgnoreIfPlaying && CheckSoundPlay(listAudType, clip)) // Neu dang play roi thi ko play lai nua
            {
                return;
            }

			if (!conf.CanStackPlay) // Neu dang play thi tat di roi play lai
			{
                var listStop = StopAudio(listAudType, clip.name);
                listAvailableAud.AddRange(listStop);
			}

			var aud = GetAvailableAudio(listAudType);
            aud.Source.clip = clip;
            aud.Source.loop = conf.Loop;
            aud.Source.volume = conf.Volume;
            aud.Source.pitch = conf.Pitch;

            if (aud.Source.isPlaying)
            {
                aud.Source.Stop();
            }
            aud.Source.Play();

            if (!conf.Loop)
			{
                aud.Corou = ins.StartCoroutine(DelayCallMaster.WaitAndDoIE(aud.Source.clip.length, () =>
                {
					if (aud.IsFadingOut)
					{
                        return;
					}

                    listAvailableAud.Add(aud);
                    StopAudio(listAudType, aud);

                    conf.OnComplete?.Invoke();
                    OnComplete?.Invoke();
                }));
            }
        }
        #region Stop
        /// <summary>
        /// Tat toan bo cac am thanh co ten "name" trong channel "type"
        /// </summary>
        public void StopAudioClip(string name, string channelName)
        {
            var listAudType = setting.GetAudioType(channelName);
            if (listAudType == null)
            {
				Debug.LogError($"AUDIO PLAYER => CANNOT FIND TYPE: {channelName}");
                return;
            }

            var listStop = StopAudio(listAudType, name);
            listAvailableAud.AddRange(listStop);
        }

        public void StopEaseAudioClip(string name, AudCommonConf conf, Ease ease = Ease.Linear, System.Action onFadeComplete = null)
        {
            var channelType = setting.GetAudioType(conf.ChannelType);
            if (channelType == null)
            {
				Debug.LogError($"AUDIO PLAYER => CANNOT FIND TYPE: {conf.ChannelType}");
                return;
            }

            var audios = channelType.ListCurrentAud.FindAll(aud => aud.NameClip == name);
            if (audios.Count == 0) // Khong co Aud muon dung trong list dang chay
            {
                return;
            }

            var aud = audios[0];
            aud.IsFadingOut = true;

            DOTweenManager.Ins.TweenFloatTime(conf.Volume, 0, 0.75f, f =>
            {
                aud.Source.volume = f;
            }).OnComplete(() =>
            {
                aud.IsFadingOut = false;
                listAvailableAud.Add(aud);
                StopAudio(channelType, aud);

                onFadeComplete?.Invoke();
            })
            .SetEase(ease);
        }

        /// <summary>
        /// Dung Aud
        /// </summary>
        void StopAudio(AudioSetting.AudioType channelType, Audio audio)
        {
            channelType.ListCurrentAud.Remove(audio);
            audio.Source.Stop();
            audio.Source.clip = null;
            audio.Source.outputAudioMixerGroup = null;

            if (audio.Corou != null) this.StopCoroutine(audio.Corou);
        }

        /// <summary>
        /// Dung toan bo Aud theo ten Clip
        /// </summary>
        /// <returns></returns>
        List<Audio> StopAudio(AudioSetting.AudioType channelType, string audio)
        {
            var audios = channelType.ListCurrentAud.FindAll(aud => aud.NameClip == audio);

            for (int i = 0; i < audios.Count; i++)
            {
                StopAudio(channelType, audios[i]);
            }

            return audios;
        }

        /// <summary>
        /// Dung toan bo Aud
        /// </summary>
        void StopAllCurrentAudio(AudioSetting.AudioType channelType)
        {
            for (int i = 0; i < channelType.ListCurrentAud.Count; i++)
            {
                channelType.ListCurrentAud[i].Source.Stop();
                channelType.ListCurrentAud[i].Source.clip = null;
                channelType.ListCurrentAud[i].Source.outputAudioMixerGroup = null;

                if (channelType.ListCurrentAud[i].Corou != null) this.StopCoroutine(channelType.ListCurrentAud[i].Corou);
            }

            channelType.ListCurrentAud.Clear();
        }
        #endregion

        bool CheckSoundPlay(AudioSetting.AudioType type, AudioClip aud)
		{
            return type.ListCurrentAud.Exists(sound => sound.NameClip == aud.name);
        }

		Audio GetAvailableAudio(AudioSetting.AudioType channelType)
		{
			if (listAvailableAud.Count > 0)
			{
				var audS = listAvailableAud[0];
				listAvailableAud.RemoveAt(0);

				audS.Source.outputAudioMixerGroup = channelType.AudioMixerGroup;
                audS.IsFadingOut = false;

                channelType.ListCurrentAud.Add(audS);
                return audS;
			}
			else
			{
				var audS = new Audio
				{
					Source = ins.gameObject.AddComponent<AudioSource>()
				};

				audS.Source.playOnAwake = false;
				listAvailableAud.Add(audS);

				return GetAvailableAudio(channelType);
			}
		}

		public void ChangeVol(string typeName, float value)
        {
			var listAudType = setting.GetAudioType(typeName);
			if (listAudType == null)
			{
				Debug.LogError($"CANNOT FIND TYPE: {typeName}");
				return;
			}
            float dBValue = (value == 0) ? -80f : Mathf.Log10(value) * 20;
			_ = listAudType.AudioMixer.SetFloat($"V_{typeName}", dBValue);
        }

        [System.Serializable]
        public class AudCommonConf
        {
#if UNITY_EDITOR
            [TextArea(1, 10)]
            [SerializeField] string Editor_Note;
#endif

            public string ChannelType;

            [Space(4)]
            public bool Loop;
            public bool StopAll; // Stop toan bo Aud cua Channel
            public bool IgnoreIfPlaying; // Neu Aud nay dang play roi thi ko play nua
            public bool CanStackPlay; // Co play chong len nhau duoc ko

            [Range(0, 1)] public float Volume = 1;
            [Range(-3, 3)] public float Pitch = 1;

            public UnityEvent OnComplete;
        }
    }
}