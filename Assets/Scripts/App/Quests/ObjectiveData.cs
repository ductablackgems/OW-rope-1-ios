using App.Effects;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.Quests
{
	public sealed class ObjectiveData
	{
		private VehicleComponents vehicle;

		private Health vehicleHealth;

		private Health npc;

		public VehicleComponents Vehicle
		{
			get
			{
				return vehicle;
			}
			set
			{
				SetVehicle(value);
			}
		}

		public bool IsVehicleKilled
		{
			get;
			private set;
		}

		public Health NPC
		{
			get
			{
				return npc;
			}
			set
			{
				SetNPC(value);
			}
		}

		public bool IsNPCKilled
		{
			get;
			private set;
		}

		public GameplayEffect VehicleArrow
		{
			get;
			set;
		}

		public GameplayEffect NPCMark
		{
			get;
			set;
		}

		public void DestroyRelatedObjects()
		{
			CleanVehicle(destroy: true);
			CleanNPC();
		}

		public void Update(GameplayObjective objective)
		{
			UpdateVehicleArrowEffect(objective);
			UpdateNPCMarkEffect(objective);
		}

		public void OnQuestFinished()
		{
			CleanVehicle();
			CleanNPC();
		}

		public void CleanVehicle(bool destroy = false)
		{
			if (!(vehicle == null))
			{
				DestroyGameObject component = vehicle.GetComponent<DestroyGameObject>();
				if (component != null)
				{
					component.enabled = true;
				}
				if (vehicleHealth != null)
				{
					vehicleHealth.OnDie -= OnVehicleKilled;
				}
				if (VehicleArrow != null)
				{
					VehicleArrow.Deactivate();
				}
				if (destroy)
				{
					vehicle.KickOffCurrentDriver();
					UnityEngine.Object.Destroy(vehicle.gameObject);
				}
				vehicle = null;
				vehicleHealth = null;
				IsVehicleKilled = false;
				VehicleArrow = null;
			}
		}

		public void CleanNPC()
		{
			if (npc != null)
			{
				npc.OnDie -= OnNPCKilled;
			}
			if (NPCMark != null)
			{
				NPCMark.Deactivate();
			}
			npc = null;
			NPCMark = null;
			IsNPCKilled = false;
		}

		private void OnVehicleKilled()
		{
			if (vehicleHealth != null)
			{
				vehicleHealth.OnDie -= OnVehicleKilled;
			}
			IsVehicleKilled = true;
		}

		private void OnNPCKilled()
		{
			if (npc != null)
			{
				npc.OnDie -= OnNPCKilled;
			}
			IsNPCKilled = true;
		}

		private void SetVehicle(VehicleComponents value)
		{
			CleanVehicle();
			vehicle = value;
			if (!(vehicle == null))
			{
				vehicleHealth = ((vehicle.health != null) ? vehicle.health : vehicle.GetComponent<Health>());
				vehicleHealth.OnDie += OnVehicleKilled;
				DestroyGameObject component = vehicle.GetComponent<DestroyGameObject>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}

		private void SetNPC(Health value)
		{
			CleanNPC();
			npc = value;
			if (!(npc == null))
			{
				npc.OnDie += OnNPCKilled;
			}
		}

		private void UpdateVehicleArrowEffect(GameplayObjective objective)
		{
			if (objective == null || vehicle == null)
			{
				return;
			}
			if (IsVehicleKilled || !(vehicle.driver != objective.Player.Transform))
			{
				if (!(VehicleArrow == null))
				{
					VehicleArrow.Deactivate();
					VehicleArrow = null;
				}
			}
			else if (VehicleArrow == null)
			{
				VehicleArrow = objective.EffectManager.GetEffect(objective.EffectManager.arrowEffectPrefab);
				VehicleArrow.Activate(vehicle.transform.position, vehicle.transform);
				VehicleArrow.transform.localPosition = new Vector3(0f, 2f, 0f);
			}
		}

		private void UpdateNPCMarkEffect(GameplayObjective objective)
		{
			if (objective == null || npc == null)
			{
				return;
			}
			if (IsNPCKilled || (!(vehicle == null) && !(vehicle.passenger != npc.transform)))
			{
				if (!(NPCMark == null))
				{
					NPCMark.Deactivate();
					NPCMark = null;
				}
			}
			else if (NPCMark == null)
			{
				NPCMark = objective.EffectManager.GetEffect(objective.EffectManager.npcMarkEffectPrefab);
				NPCMark.Activate(npc.transform.position, npc.transform);
				NPCMark.transform.localPosition = new Vector3(0f, 2.25f, 0f);
			}
		}
	}
}
