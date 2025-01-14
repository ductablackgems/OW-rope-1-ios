using BG_Library.Common;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BG_Library.Audio
{
	[CreateAssetMenu(menuName = "BG/Audio/Single channel")]
	public class PlayAudioChannelSO : ScriptableObject
	{
		[BoxGroup("CONFIGS")] public AudioClip Clip;

		[BoxGroup("CONFIGS")] public AudioPlayer.AudCommonConf[] Confs;

		public void Play()
		{
			AudioPlayer.Ins.PlayAudioClip(Clip, Confs[0]);
		}

		public void Play(int conf)
		{
			if (conf >= Confs.Length)
			{
				Debug.LogError($"Out of index Conf: {Clip.name}");
				return;
			}
			AudioPlayer.Ins.PlayAudioClip(Clip, Confs[conf]);
		}

		public void Stop()
		{
			AudioPlayer.Ins.StopAudioClip(Clip.name, Confs[0].ChannelType);
		}

		public void Stop(string channel)
		{
			AudioPlayer.Ins.StopAudioClip(Clip.name, channel);
		}
	}
}