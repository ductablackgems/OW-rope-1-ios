using UnityEngine;

namespace App.Player.FightSystem
{
	public class FightSoundsScriptableObject : ScriptableObject
	{
		public AudioClip[] strikeClips;

		public AudioClip[] missClips;

		public AudioClip[] hitClips;
	}
}
