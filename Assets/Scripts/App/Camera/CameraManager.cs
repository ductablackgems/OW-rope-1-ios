using App.Vehicles;
using App.Vehicles.Tank;
using UnityEngine;

namespace App.Camera
{
	public class CameraManager : MonoBehaviour
	{
		private GunShopCamera gunShopCamera;

		private ClothesShopCamera clothesShopCamera;

		private HelicopterCamera helicopterCamera;

		private SkateboardCamera skateboardCamera;

		private AirplaneCamera aircraftCamera;

		private OotiiCamera ootiiCamera;

		private DogShopCamera dogShopCamera;

		private MonoBehaviour[] cameras;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		public void SetVehicleCamera(VehicleComponents vehicleComponents)
		{
			SetOotiiCamera();
			ootiiCamera.SetVehicle(vehicleComponents);
		}

		public void SetWreckCamera(WreckHelper wreck)
		{
			ootiiCamera.SetWreck(wreck);
		}

		public bool RunningWreckCamera()
		{
			return ootiiCamera.TargetingWreck();
		}

		public void SetPlayerCamera()
		{
			SetOotiiCamera();
			ootiiCamera.ClearVehicle();
		}

		public void SetMenuCamera()
		{
			MonoBehaviour[] array = cameras;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (!(monoBehaviour == null))
				{
					monoBehaviour.enabled = false;
				}
			}
			base.transform.position = initialPosition;
			base.transform.rotation = initialRotation;
		}

		public void SetGunShopCamera()
		{
			EnableCamera(gunShopCamera);
		}

		public void SetClothesShopCamera()
		{
			EnableCamera(clothesShopCamera);
		}

		public void SetHelicopterCamera(Transform cameraTarget)
		{
			helicopterCamera.targetPosition = cameraTarget;
			EnableCamera(helicopterCamera);
		}

		public void SetSkateboardCamera(GameObject skateboard)
		{
			skateboardCamera.ConnectSkateboard(skateboard);
			EnableCamera(skateboardCamera);
		}

		public void SetAircraftCamera(Transform cameraTarget)
		{
			aircraftCamera.Target = cameraTarget;
			EnableCamera(aircraftCamera);
		}

		public void SetDogShopCamera()
		{
			EnableCamera(dogShopCamera);
		}

		private void SetOotiiCamera()
		{
			EnableCamera(ootiiCamera);
		}

		private void Awake()
		{
			cameras = new MonoBehaviour[7]
			{
				gunShopCamera = this.GetComponentSafe<GunShopCamera>(),
				clothesShopCamera = this.GetComponentSafe<ClothesShopCamera>(),
				helicopterCamera = this.GetComponentSafe<HelicopterCamera>(),
				skateboardCamera = this.GetComponentSafe<SkateboardCamera>(),
				aircraftCamera = this.GetComponentSafe<AirplaneCamera>(),
				ootiiCamera = this.GetComponentSafe<OotiiCamera>(),
				dogShopCamera = GetComponent<DogShopCamera>()
			};
			initialPosition = base.transform.position;
			initialRotation = base.transform.rotation;
		}

		private void EnableCamera(MonoBehaviour toEnable)
		{
			MonoBehaviour[] array = cameras;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (!(monoBehaviour == null))
				{
					monoBehaviour.enabled = (monoBehaviour == toEnable);
				}
			}
		}
	}
}
