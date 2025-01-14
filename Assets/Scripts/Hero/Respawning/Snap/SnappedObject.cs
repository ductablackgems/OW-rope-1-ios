using System;
using UnityEngine;

namespace Hero.Respawning.Snap
{
	[Serializable]
	public class SnappedObject
	{
		public string tid;

		public GameObject gameObject;

		private bool tidHashLoaded;

		private int tidHash;

		public int GetTidHash()
		{
			if (!tidHashLoaded)
			{
				tidHash = tid.GetHashCode();
				tidHashLoaded = true;
			}
			return tidHash;
		}
	}
}
