using UnityEngine;

namespace App.Player.SkeletonEffect
{
	public class SkeletonMappingScriptableObject : ScriptableObject
	{
		public GameObject skeletonPrefab;

		public GameObject burnedSkeletonPrefab;

		public GameObject greenSkeletonPrefab;

		public GameObject electrifiedSkeletonPrefab;

		public GameObject skeletonFirePrefab;

		public GameObject greenSkeletonFirePrefab;

		public SkeletonMappingItem[] items;
	}
}
