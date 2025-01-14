using UnityEngine;

namespace App.Vehicles.Mech
{
	public class MechSounds : MonoBehaviour
	{
		public AudioSource Engine;

		public AudioSource FootSteps;

		public AudioSource LegMoves;

		public AudioSource Roller;

		public AudioSource Effects;

		[Header("Clips")]
		public AudioClip[] FootStepClips;

		public AudioClip[] LegsMoveClips;
	}
}
