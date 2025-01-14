using BG_Library.Common;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BG_Library.Audio
{
	[CreateAssetMenu(menuName = "BG/Audio/Random channel")]
	public class PlayRandomChannelSO : ScriptableObject
	{
		[BoxGroup("CONFIGS")] public AudioClip[] Clip;
		[BoxGroup("CONFIGS")] public AudioPlayer.AudCommonConf[] Confs;

		[BoxGroup("Status"), ReadOnly, SerializeField] int currentIndex;

		public void Play()
		{
			currentIndex = Random.Range(0, Clip.Length);
			AudioPlayer.Ins.PlayAudioClip(Clip[currentIndex], Confs[0]);
		}

		public void Play(int conf)
		{
			if (conf >= Confs.Length)
			{
				Debug.LogError($"Out of index Conf: {Clip[currentIndex].name}");
				return;
			}

			currentIndex = Random.Range(0, Clip.Length);
			AudioPlayer.Ins.PlayAudioClip(Clip[currentIndex], Confs[conf]);
		}

		public void Stop()
		{
			AudioPlayer.Ins.StopAudioClip(Clip[currentIndex].name, Confs[0].ChannelType);
		}

		public void Stop(string channel)
		{
			AudioPlayer.Ins.StopAudioClip(Clip[currentIndex].name, channel);
		}
	}
}