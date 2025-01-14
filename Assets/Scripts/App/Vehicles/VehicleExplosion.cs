using App.Camera;
using App.Player;
using App.Util;
using App.Vehicles.Car.GarbageTruck;
using App.Vehicles.Tank;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles
{
	public class VehicleExplosion : MonoBehaviour
	{
		public GameObject burnedVehiclePrefab;

		public GameObject engineFire1;

		public GameObject engineFire2;

		public Transform wreckPosition;

		public Transform tankTower;

		private Health health;

		private VehicleComponents vehicleComponents;

		private GameObject burnedVehicle;

		private bool AddForceVehicleBool;

		public bool Car;

		private Transform player;

		private Material burnedMaterial;

		private float fireCooledAt = -1f;

		private bool initialized;

		private bool useWreckPrefab;

		private MonoBehaviour[] wreckScriptsToKeep;

		private MonoBehaviour[] components;

		private List<MonoBehaviour> componentsToDestroy;

		public bool IsExploded
		{
			get;
			private set;
		}

		public void CoolDownFire()
		{
			fireCooledAt = Time.time;
			engineFire1.SetActive(value: false);
			engineFire2.SetActive(value: false);
		}

		public bool IsInFire()
		{
			if (!initialized)
			{
				Process();
			}
			if (!engineFire1.activeSelf)
			{
				return engineFire2.activeSelf;
			}
			return true;
		}

		private void Awake()
		{
			health = this.GetComponentSafe<Health>();
			vehicleComponents = this.GetComponentSafe<VehicleComponents>();
			player = GameObject.FindGameObjectWithTag("Player").transform;
			burnedMaterial = (Resources.Load("BurnedMaterial", typeof(Material)) as Material);
			wreckScriptsToKeep = burnedVehiclePrefab.GetComponentsInChildren<MonoBehaviour>();
			components = GetComponents<MonoBehaviour>();
			componentsToDestroy = new List<MonoBehaviour>();
		}

		private void Update()
		{
			Process();
			if (health.Dead() && !IsExploded)
			{
				Explode();
			}
		}

		public void Process()
		{
			initialized = true;
			float currentHealth = health.GetCurrentHealth();
			if (currentHealth > 0.8f)
			{
				engineFire1.SetActive(value: false);
				engineFire2.SetActive(value: false);
			}
			else if (fireCooledAt < 0f || health.AttackedRecently(Time.time - fireCooledAt + 1f))
			{
				if (currentHealth > 0.3f)
				{
					engineFire1.SetActive(value: true);
					engineFire2.SetActive(value: false);
				}
				else
				{
					engineFire1.SetActive(value: false);
					engineFire2.SetActive(value: true);
				}
				fireCooledAt = -1f;
			}
			else
			{
				engineFire1.SetActive(value: false);
				engineFire2.SetActive(value: false);
			}
		}

		public void AddForceVehicle(float power)
		{
			AddForceVehicleBool = true;
			GetComponent<Rigidbody>().AddForce(player.forward * power, ForceMode.Impulse);
		}

		private void Explode()
		{
			useWreckPrefab = !health.IsSquashed;
			if (useWreckPrefab)
			{
				burnedVehicle = UnityEngine.Object.Instantiate(burnedVehiclePrefab);
				burnedVehicle.transform.position = ((wreckPosition == null) ? base.transform.position : wreckPosition.position);
				burnedVehicle.transform.rotation = base.transform.rotation;
				burnedVehicle.GetComponentSafe<Rigidbody>().velocity = this.GetComponentSafe<Rigidbody>().velocity;
				WreckHelper wreckHelper = (vehicleComponents.type == VehicleType.Tank) ? burnedVehicle.GetComponent<WreckHelper>() : null;
				if (wreckHelper != null)
				{
					bool num = vehicleComponents.driver != null && vehicleComponents.driver.CompareTag("Player");
					wreckHelper.FixTankTower(vehicleComponents.cameraTarget, tankTower);
					if (num)
					{
						ServiceLocator.Get<CameraManager>().SetWreckCamera(wreckHelper);
					}
				}
			}
			else
			{
				burnedVehicle = base.gameObject;
				ChangeMaterial(burnedMaterial);
				OnWreckViaMaterial();
			}
			VehicleType type = vehicleComponents.type;
			Transform driver = vehicleComponents.driver;
			KillCharactersInVehicle();
			if ((type == VehicleType.Tank || type == VehicleType.Mech) && driver != null)
			{
				driver.GetComponentSafe<RagdollHelper>().SetRagdollVelocity(base.transform.up * 12f);
			}
			if (AddForceVehicleBool && Car)
			{
				burnedVehicle.GetComponent<Rigidbody>().AddForce(player.forward * 15000f, ForceMode.Impulse);
			}
			ContainerParameters componentInChildren = base.gameObject.GetComponentInChildren<ContainerParameters>();
			GarbageTruckDispatching component = base.gameObject.GetComponent<GarbageTruckDispatching>();
			if ((bool)componentInChildren)
			{
				componentInChildren.OnDestroyDumper();
			}
			if ((bool)component)
			{
				component.MissionFailed();
			}
			if (useWreckPrefab)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			IsExploded = true;
		}

		private void KillCharactersInVehicle()
		{
			KickOffDriver();
			for (int i = 0; i < vehicleComponents.sitPoint.childCount; i++)
			{
				Transform child = vehicleComponents.sitPoint.GetChild(i);
				if (child.CompareTag("Player") || child.CompareTag("Enemy"))
				{
					vehicleComponents.driver = child;
					KickOffDriver();
					break;
				}
			}
			KickOffPassenger();
			if (!(vehicleComponents.passengerSitPoint != null))
			{
				return;
			}
			int num = 0;
			Transform child2;
			while (true)
			{
				if (num < vehicleComponents.passengerSitPoint.childCount)
				{
					child2 = vehicleComponents.passengerSitPoint.GetChild(num);
					if (child2.CompareTag("Player") || child2.CompareTag("Enemy"))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			vehicleComponents.passenger = child2;
			KickOffPassenger();
		}

		protected void OnWreckViaMaterial()
		{
			MonoBehaviour[] array = components;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				bool flag = false;
				MonoBehaviour[] array2 = wreckScriptsToKeep;
				foreach (MonoBehaviour monoBehaviour2 in array2)
				{
					if (monoBehaviour.GetType() == monoBehaviour2.GetType() && !componentsToDestroy.Contains(monoBehaviour))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					componentsToDestroy.Add(monoBehaviour);
				}
			}
			DestroyUnwantedComponents();
		}

		private void DestroyUnwantedComponents()
		{
			for (int i = 0; i < componentsToDestroy.Count; i++)
			{
				if (componentsToDestroy[i].GetType() != GetType() && componentsToDestroy[i] != null)
				{
					UnityEngine.Object.Destroy(componentsToDestroy[i]);
				}
			}
		}

		private void ChangeMaterial(Material material)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				Material[] array = new Material[meshRenderer.materials.Length];
				for (int j = 0; j < meshRenderer.materials.Length; j++)
				{
					array[j] = material;
				}
				meshRenderer.materials = array;
			}
		}

		private void KickOffDriver()
		{
			Transform driver = vehicleComponents.driver;
			vehicleComponents.KickOffCurrentDriver();
			if (driver != null)
			{
				driver.GetComponentSafe<Health>().Kill();
			}
		}

		private void KickOffPassenger()
		{
			Transform passenger = vehicleComponents.passenger;
			vehicleComponents.KickOffPassenger();
			if (passenger != null)
			{
				passenger.GetComponentSafe<Health>().Kill();
			}
		}
	}
}
