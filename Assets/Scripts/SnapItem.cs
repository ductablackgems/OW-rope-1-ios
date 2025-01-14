using System;
using UnityEngine;

namespace Hero.Respawning.Snap
{
	[Serializable]
	public struct SnapItem
	{
		public int index;

		public Vector3 position;

		public Quaternion rotation;
	}
}
