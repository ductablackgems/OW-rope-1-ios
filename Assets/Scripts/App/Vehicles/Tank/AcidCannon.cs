using App.Player;
using App.Player.SkeletonEffect;
using App.Util;
using UnityEngine;

namespace App.Vehicles.Tank
{
	public class AcidCannon : MonoBehaviour
	{
		public Transform waterFlowObject;

		public ParticleSystem waterFlowParticles;

		public float MaxDistance = 10f;

		private float Distance;

		public LayerMask layerMask;

		public bool debug;

		private float Pulse = -1f;

		private bool PulseBool;

		public GameObject owner;

		private const float HumanHeatSpeed = 1.4f;

		private TankManager tankManager;

		public void Clear()
		{
			waterFlowObject.gameObject.SetActive(value: false);
			waterFlowParticles.Stop(withChildren: true);
		}

		private void Start()
		{
			tankManager = GetComponentInParent<TankManager>();
			Clear();
		}

		private void FixedUpdate()
		{
			if (tankManager.Active)
			{
				Control(InputUtils.TankMinigun.IsPressed);
			}
		}

		public void Control(bool attackPressed)
		{
			if (!attackPressed)
			{
				Clear();
				Distance = 0f;
				return;
			}
			Distance += 1f * Time.deltaTime * 8f;
			if (Distance >= MaxDistance)
			{
				Distance = MaxDistance;
			}
			Ray ray = new Ray(base.transform.position, base.transform.forward * (Distance + 2f) + base.transform.up * (Pulse * -1f));
			if (debug)
			{
				waterFlowObject.gameObject.SetActive(value: true);
			}
			waterFlowParticles.Play(withChildren: true);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, Distance, layerMask))
			{
				if (debug)
				{
					waterFlowObject.localScale = new Vector3(waterFlowObject.localScale.x, waterFlowObject.localScale.y, hitInfo.distance);
					UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward * (Distance + 2f) + base.transform.up * (Pulse * -1f), Color.green);
				}
				if (Pulse <= -1f)
				{
					PulseBool = false;
				}
				else if (Pulse >= 5f)
				{
					PulseBool = true;
				}
				if (!PulseBool)
				{
					Pulse += 0.1f * Time.deltaTime * 400f;
				}
				else
				{
					Pulse = (Pulse -= 0.1f * Time.deltaTime * 400f);
				}
				WhoIsResult whoIsResult = WhoIs.Resolve(hitInfo.collider, WhoIs.Masks.Flamethrower);
				if (whoIsResult.IsEmpty)
				{
					return;
				}
				if (whoIsResult.Compare(WhoIs.Masks.AllRagdollableHumans))
				{
					HumanFireManager component = whoIsResult.gameObject.GetComponent<HumanFireManager>();
					if (component != null)
					{
						component.StartFire(GetComponentInParent<TankManager>().Owner, acidType: true);
					}
				}
				else if (!whoIsResult.Compare(WhoIs.Entities.RagdollPart) && whoIsResult.Compare(WhoIs.Masks.AllStreetVehicles))
				{
					VehicleComponents componentSafe = whoIsResult.transform.GetComponentSafe<VehicleComponents>();
					if (componentSafe.driver != null)
					{
						RagdollHelper componentSafe2 = componentSafe.driver.GetComponentSafe<RagdollHelper>();
						componentSafe.KickOffCurrentDriver(relocateCharacter: false);
						componentSafe2.SetRagdollVelocity(base.transform.forward * 8f);
						componentSafe2.GetComponentSafe<Health>().ApplyDamage(5f);
					}
				}
			}
			else
			{
				waterFlowObject.localScale = new Vector3(waterFlowObject.localScale.x, waterFlowObject.localScale.y, Distance);
			}
		}
	}
}
