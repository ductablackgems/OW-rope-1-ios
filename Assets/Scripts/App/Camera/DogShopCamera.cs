using UnityEngine;

namespace App.Camera
{
	public class DogShopCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform initPosition;

		private void OnEnable()
		{
			if (!(initPosition == null))
			{
				base.transform.position = initPosition.position;
				base.transform.rotation = initPosition.rotation;
			}
		}
	}
}
