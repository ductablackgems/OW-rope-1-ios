using App.Base;
using UnityEngine;

namespace App.Dogs
{
	public class DogSpot : BaseBehaviour
	{
		[SerializeField]
		private DogManager.DogSlot slot;

		public DogManager.DogSlot Slot => slot;

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.Position, 0.25f);
			Gizmos.color = color;
		}
	}
}
