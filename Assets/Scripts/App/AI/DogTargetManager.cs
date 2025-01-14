using App.Dogs;
using App.Player;
using App.Util;
using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.AI
{
	public sealed class DogTargetManager : ITargetManager
	{
		private const float MAX_TARET_SELECTION_DISTANCE = 50f;

		private const float MAX_TARGET_REACH_DISTANCE = 100f;

		private const float RAYCAST_RADIUS = 2f;

		private int layerMask;

		private Dog dog;

		private Health playerHealth;

		private AbstractVehicleDriver driver;

		private RaycastHit[] hits = new RaycastHit[20];

		private List<RaycastHit> hitList = new List<RaycastHit>(20);

		private List<Health> assistences = new List<Health>(16);

		private List<GameObject> targetList = new List<GameObject>(16);

		private List<AbstractVehicleDriver> drivers = new List<AbstractVehicleDriver>(16);

		public GameObject Target
		{
			get
			{
				if (!(TargetHealth != null))
				{
					return null;
				}
				return TargetHealth.gameObject;
			}
		}

		public Health TargetHealth
		{
			get;
			private set;
		}

		public bool HasTarget => TargetHealth != null;

		public DogTargetManager(Dog dog)
		{
			this.dog = dog;
			layerMask = LayerMask.GetMask("Impact", "Default", "Enemy");
			playerHealth = dog.Owner.GetComponent<Health>();
			playerHealth.OnDamage += OnPlayerDamaged;
		}

		public void SetTargetInDirection()
		{
			UnityEngine.Camera main = UnityEngine.Camera.main;
			int num = Physics.SphereCastNonAlloc(direction: main.transform.forward, origin: main.transform.position, radius: 2f, results: hits, maxDistance: 50f, layerMask: layerMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (num == 0)
			{
				return;
			}
			hitList.Clear();
			for (int i = 0; i < num; i++)
			{
				hitList.Add(hits[i]);
			}
			ValidateRaycastHits(hitList);
			if (hitList.Count != 0)
			{
				Transform transform = hitList[0].transform;
				if (HasTarget && transform != TargetHealth.transform)
				{
					RemoveTarget();
				}
				SetTarget(transform.gameObject);
			}
		}

		public void Clean()
		{
			RemoveTarget();
			targetList.Clear();
		}

		public void Update()
		{
			ValidateTarget();
		}

		public void StopActiveDriver(bool isKickOut = false)
		{
			if (!(driver == null))
			{
				if (isKickOut)
				{
					driver.HandleKickOutOffVehicle();
				}
				else
				{
					driver.Stop();
				}
				driver = null;
			}
		}

		GameObject ITargetManager.GetTarget()
		{
			if (!(TargetHealth != null))
			{
				return null;
			}
			return TargetHealth.gameObject;
		}

		GameObject ITargetManager.GetVisibleTargetInRange()
		{
			return null;
		}

		private void OnPlayerDamaged(float damage, int damageType, GameObject agressor)
		{
			if ((damageType == 1 || damageType == 2) && !(agressor == null) && agressor.layer == 12)
			{
				if (HasTarget)
				{
					AddTargetToList(agressor);
				}
				SetTarget(agressor);
			}
		}

		private void OnTargetDamaged(float damage, int damageType, GameObject agressor)
		{
			if (!(agressor != dog.gameObject) && !assistences.Contains(TargetHealth))
			{
				assistences.Add(TargetHealth);
			}
		}

		private void OnTargetDie()
		{
			int num = assistences.IndexOf(TargetHealth);
			if (num >= 0)
			{
				dog.RegisterKill();
				assistences.RemoveAt(num);
			}
			RemoveTarget();
			SetTarget(GetNextTarget());
		}

		private void SetTarget(GameObject target)
		{
			if (!(target == null) && !(TargetHealth != null))
			{
				VehicleComponents component = target.GetComponent<VehicleComponents>();
				if (component != null && component.driver != null)
				{
					target = component.driver.gameObject;
					drivers.Clear();
					target.GetComponents(drivers);
					foreach (AbstractVehicleDriver driver2 in drivers)
					{
						if (driver2.DriverType == component.type)
						{
							driver = driver2;
							break;
						}
					}
				}
				Health component2 = target.GetComponent<Health>();
				if (!(component2 == null) && !component2.Dead())
				{
					TargetHealth = component2;
					TargetHealth.OnDie += OnTargetDie;
					TargetHealth.OnDamage += OnTargetDamaged;
				}
			}
		}

		private void ValidateRaycastHits(List<RaycastHit> hits)
		{
			int num = hits.Count;
			while (num-- > 0)
			{
				RaycastHit raycastHit = hits[num];
				if (WhoIs.Resolve(raycastHit.collider, WhoIs.Masks.BulletImpactResolving).IsEmpty || !CanAttack(raycastHit.transform))
				{
					hits.RemoveAt(num);
				}
			}
			if (hits.Count != 0)
			{
				Vector3 startPos = playerHealth.transform.position;
				hits.Sort(delegate(RaycastHit obj1, RaycastHit obj2)
				{
					float num3 = Vector3.Distance(startPos, obj1.transform.position);
					float value = Vector3.Distance(startPos, obj2.transform.position);
					return num3.CompareTo(value);
				});
			}
		}

		private bool CanAttack(Transform target)
		{
			if (target.gameObject == dog.gameObject)
			{
				return false;
			}
			if (target.gameObject == dog.Owner.gameObject)
			{
				return false;
			}
			if (target.CompareTag("Dog"))
			{
				return false;
			}
			Health component = target.GetComponent<Health>();
			if (component == null)
			{
				return false;
			}
			if (component.Dead())
			{
				return false;
			}
			VehicleComponents component2 = target.GetComponent<VehicleComponents>();
			if (component2 == null)
			{
				return true;
			}
			if (component2.driver == null)
			{
				return false;
			}
			bool flag = false;
			switch (component2.type)
			{
			case VehicleType.Bike:
			case VehicleType.Bicycle:
			case VehicleType.Gyroboard:
			case VehicleType.Skateboard:
				return true;
			default:
				return false;
			}
		}

		private void ValidateTarget()
		{
			if (!(TargetHealth == null) && !(Vector3.Distance(dog.Owner.transform.position, dog.transform.position) < 100f))
			{
				RemoveTarget();
			}
		}

		private void RemoveTarget()
		{
			if (!(TargetHealth == null))
			{
				TargetHealth.OnDie -= OnTargetDie;
				TargetHealth.OnDamage -= OnTargetDamaged;
				TargetHealth = null;
			}
		}

		private void AddTargetToList(GameObject target)
		{
			if (!(Target == target) && !targetList.Contains(target))
			{
				targetList.Add(target);
			}
		}

		private GameObject GetNextTarget()
		{
			float num = float.MaxValue;
			GameObject result = null;
			int num2 = targetList.Count;
			while (num2-- > 0)
			{
				GameObject gameObject = targetList[num2];
				if (!gameObject.activeSelf)
				{
					targetList.RemoveAt(num2);
					continue;
				}
				Health component = gameObject.GetComponent<Health>();
				if (component == null)
				{
					targetList.RemoveAt(num2);
					continue;
				}
				if (component.GetCurrentHealthNumeric() <= 0f)
				{
					targetList.RemoveAt(num2);
					continue;
				}
				float num4 = Vector3.Distance(dog.Position, gameObject.transform.position);
				if (!(num4 > num))
				{
					result = gameObject;
					num = num4;
				}
			}
			return result;
		}
	}
}
