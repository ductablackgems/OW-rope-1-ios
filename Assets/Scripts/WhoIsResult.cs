using UnityEngine;

namespace App
{
	public struct WhoIsResult
	{
		public Transform transform;

		public GameObject gameObject;

		public WhoIsItemDefinition definition;

		public bool skipped;

		public bool IsEmpty => transform == null;

		public WhoIsResult(Transform transform, WhoIsItemDefinition definition)
		{
			this.transform = transform;
			gameObject = ((transform == null) ? null : transform.gameObject);
			this.definition = definition;
			skipped = false;
		}

		public bool Compare(WhoIsItemDefinition definition)
		{
			return definition.Equals(this.definition);
		}

		public bool Compare(WhoIsItemDefinition[] mask)
		{
			for (int i = 0; i < mask.Length; i++)
			{
				if (Compare(mask[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
