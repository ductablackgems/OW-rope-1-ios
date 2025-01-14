using App.Shop.GunSlider;
using UnityEngine;

namespace App.Camera
{
	public class GunShopCamera : MonoBehaviour
	{
		public float speed = 5f;

		public float rotationSpeed = 20f;

		public Transform initPosition;

		public GunSliderControl sliderControl;

		private void Awake()
		{
			sliderControl = ServiceLocator.Get<GunSliderControl>();
		}

		private void OnEnable()
		{
			if (initPosition != null)
			{
				base.transform.position = initPosition.position;
				base.transform.rotation = initPosition.rotation;
			}
		}

		private void Update()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, sliderControl.GetTargetCameraPosition(), speed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, sliderControl.GetTargetCameraRotation(), rotationSpeed * Time.deltaTime);
		}
	}
}
