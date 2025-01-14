using UnityEngine;

namespace App.AI
{
	public abstract class AbstractAIScript : MonoBehaviour
	{
		private Transform componentsRoot;

		private bool baseInitialized;

		public Transform ComponentsRoot
		{
			get
			{
				if (!baseInitialized)
				{
					componentsRoot = base.transform.parent;
					baseInitialized = true;
				}
				return componentsRoot;
			}
		}
	}
}
