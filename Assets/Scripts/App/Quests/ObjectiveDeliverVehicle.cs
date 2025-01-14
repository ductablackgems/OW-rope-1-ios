using App.Vehicles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	[RequireComponent(typeof(Collider))]
	public class ObjectiveDeliverVehicle : GameplayObjective
	{
		[Serializable]
		public class DeliveryData
		{
			public VehicleType VehicleType;

			public int Amount;

			public int Delivered
			{
				get;
				set;
			}
		}

		[Header("Deliver Vehicle")]
		[SerializeField]
		private DeliveryData[] deliveryData = new DeliveryData[0];

		[SerializeField]
		private float destroyDistance = 10f;

		private List<DeliveryData> list = new List<DeliveryData>(8);

		private List<VehicleComponents> delivered = new List<VehicleComponents>(4);

		private VehicleComponents candidate;

		private int layer;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			layer = LayerMask.GetMask("Impact");
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			list.Clear();
			DeliveryData[] array = this.deliveryData;
			foreach (DeliveryData deliveryData in array)
			{
				list.Add(new DeliveryData
				{
					VehicleType = deliveryData.VehicleType,
					Amount = deliveryData.Amount,
					Delivered = 0
				});
			}
			base.ShortDecription = GetShortDescription();
		}

		protected override void OnReset()
		{
			base.OnReset();
			candidate = null;
			list.Clear();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			UpdateCandidate();
			DestroyDeliveredVehicles();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (candidate != null || other.gameObject.layer != 10)
			{
				return;
			}
			VehicleComponents vehicleComponents = other.GetComponent<VehicleComponents>();
			if (vehicleComponents == null)
			{
				vehicleComponents = other.GetComponentInParent<VehicleComponents>();
			}
			if (!(vehicleComponents == null))
			{
				VehicleComponents playerVehicle = GetPlayerVehicle();
				if (!(vehicleComponents != playerVehicle) && GetVehicleData(vehicleComponents) != null)
				{
					candidate = vehicleComponents;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!(candidate == null) && other.gameObject.layer == 10 && !(candidate.gameObject != other.gameObject))
			{
				candidate = null;
			}
		}

		private void UpdateCandidate()
		{
			if (!(candidate == null) && !(candidate.driver != null))
			{
				candidate.handleTrigger.gameObject.SetActive(value: false);
				delivered.Add(candidate);
				DeliveryData vehicleData = GetVehicleData(candidate);
				vehicleData.Delivered++;
				candidate = null;
				if (vehicleData.Amount == vehicleData.Delivered)
				{
					list.Remove(vehicleData);
				}
				base.ShortDecription = GetShortDescription();
				if (list.Count == 0)
				{
					DestroyDeliveredVehicles(force: true);
					Finish();
				}
			}
		}

		private VehicleComponents GetPlayerVehicle()
		{
			VehicleComponents vehicle = base.Player.PlayerMonitor.GetVehicle();
			if (vehicle == null)
			{
				return null;
			}
			if (vehicle.driver != base.Player.Transform)
			{
				return null;
			}
			return vehicle;
		}

		private DeliveryData GetVehicleData(VehicleComponents vehicle)
		{
			if (vehicle == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].VehicleType == vehicle.type)
				{
					return list[i];
				}
			}
			return null;
		}

		private void DestroyDeliveredVehicles(bool force = false)
		{
			if (delivered.Count == 0)
			{
				return;
			}
			int num = delivered.Count;
			while (num-- > 0)
			{
				VehicleComponents vehicleComponents = delivered[num];
				if (force)
				{
					delivered.RemoveAt(num);
					UnityEngine.Object.Destroy(vehicleComponents.gameObject);
					continue;
				}
				Vector3 vector = vehicleComponents.transform.position - base.Player.Transform.position;
				if (!(vector.magnitude < destroyDistance) && !(Vector3.Dot(vector.normalized, UnityEngine.Camera.main.transform.forward.normalized) > 0f))
				{
					delivered.RemoveAt(num);
					UnityEngine.Object.Destroy(vehicleComponents.gameObject);
				}
			}
		}

		protected override string GetShortDescription()
		{
			string text = string.Empty;
			for (int i = 0; i < list.Count; i++)
			{
				DeliveryData deliveryData = list[i];
				text += $"{deliveryData.Delivered}/{deliveryData.Amount} {deliveryData.VehicleType}\n";
			}
			return text;
		}
	}
}
