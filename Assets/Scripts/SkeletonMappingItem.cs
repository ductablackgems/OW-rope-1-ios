using System;
using UnityEngine;

namespace App.Player.SkeletonEffect
{
	[Serializable]
	public struct SkeletonMappingItem
	{
		public HumanBodyBones bone;

		public Quaternion difference;

		public SkeletonMappingItem(HumanBodyBones bone, Quaternion difference)
		{
			this.bone = bone;
			this.difference = difference;
		}
	}
}
