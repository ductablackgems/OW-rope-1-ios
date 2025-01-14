using System;
using UnityEngine;

namespace App.AI.Scanner
{
	[Serializable]
	public class AIScannerZone
	{
		public float maxDistance;

		public bool trackNew;

		public float keepTrackDuration;

		[Space(10f)]
		public bool use360Angle;

		public bool useRaycast;

		[Space(10f)]
		public bool useRaycastTracked;
	}
}
