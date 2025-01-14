using BG_Library.Common;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BG_Library.Audio
{
    [CreateAssetMenu(menuName = "BG/Audio/List channel")]
    public class PlayListAudioChannelSO : ScriptableObject
    {
		[BoxGroup("CONFIGS")] public AudioClip[] Clip;
		[BoxGroup("CONFIGS")] public string TypeChannel;
		[BoxGroup("CONFIGS")] public bool Shuffle;
		[BoxGroup("CONFIGS"), Range(0, 1)] public float Volume = 1;
		[BoxGroup("CONFIGS"), Range(-3, 3)] public float Pitch = 1;

		[BoxGroup("DEBUG"), SerializeField, ReadOnly] int[] listOrder;
		[BoxGroup("DEBUG"), SerializeField, ReadOnly] int current;

		[BoxGroup("DEBUG"), SerializeField, ReadOnly] AudioPlayer.AudCommonConf Conf;

		public void Play()
		{
			current = 0;
			listOrder = new int[Clip.Length];

			if (Shuffle)
			{
				ShuffleList();
			}
			else
			{
				for (int i = 0; i < listOrder.Length; i++)
				{
					listOrder[i] = i;
				}
			}

			PlaySingle();
		}

		public void Stop()
		{
			AudioPlayer.Ins.StopAudioClip(Clip[listOrder[current]].name, TypeChannel);
		}

		public void StopEase(DG.Tweening.Ease ease = DG.Tweening.Ease.Linear, System.Action onFadeComplete = null)
		{
			AudioPlayer.Ins.StopEaseAudioClip(Clip[listOrder[current]].name, Conf, ease, onFadeComplete);
		}

		void PlaySingle()
		{
			AudioPlayer.Ins.PlayAudioClip(Clip[listOrder[current]], Conf, ()=> 
			{
				current++;
				if (current >= listOrder.Length)
				{
					current = 0;
				}

				PlaySingle();
			});
		}

		void ShuffleList()
		{
			var tempList = new List<int>();
			for (int i = 0; i < listOrder.Length; i++)
			{
				tempList.Add(i);
			}

			var random = new RandomNoRepeat<int>(tempList);
			for (int i = 0; i < listOrder.Length; i++)
			{
				listOrder[i] = random.Random();
			}
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			Conf = new AudioPlayer.AudCommonConf
			{
				ChannelType = TypeChannel,
				Loop = false,
				StopAll = true,
				IgnoreIfPlaying = true,
				CanStackPlay = false,
				Volume = Volume,
				Pitch = Pitch
			};
		}
#endif
	}
}