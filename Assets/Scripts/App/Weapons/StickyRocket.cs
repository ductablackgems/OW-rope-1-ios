using App.Player;
using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.Weapons
{
	public class StickyRocket : MonoBehaviour
	{
		private struct TargetColliderInfo
		{
			public bool isCar;

			public RaycastHit hit;

			public RagdollHelper ragdollHelper;

			public VehicleComponents vehicleComponents;
		}

		public float thrustForceOnHumans = 1000f;

		public float thrustForceOnCars = 3000f;

		private VehicleComponents vehicleComponents;

		private RagdollHelper ragdollHelper;

		private bool isAttachedToCar;

		private Rigidbody targetRigidbody;

		private Vector3 thrustDirection;

		private float force;

		private float lastThrustUpwardsTime;

		private static readonly List<RagdollHelper.BodyPart> tempBodyParts = new List<RagdollHelper.BodyPart>();

		private static readonly RaycastHit[] raycastHits = new RaycastHit[100];

		public bool Launched
		{
			get;
			private set;
		}

		public static bool CanStickRocket(Ray ray, float maxDistance, out Vector3 point, out Vector3 direction)
		{
			if (GetTargetCollider(ray, maxDistance, out TargetColliderInfo info))
			{
				point = info.hit.point;
				direction = AdjustDirection(info.hit.normal);
				return true;
			}
			point = Vector3.zero;
			direction = Vector3.zero;
			return false;
		}

		public static StickyRocket TryStickRocket(Ray ray, float maxDistance, StickyRocket prefab)
		{
			if (!GetTargetCollider(ray, maxDistance, out TargetColliderInfo info))
			{
				return null;
			}
			RaycastHit hit = info.hit;
			Collider collider = hit.collider;
			Rigidbody componentInParent = collider.GetComponentInParent<Rigidbody>();
			if (componentInParent == null)
			{
				return null;
			}
			if (!collider.CompareTag("RagdollPart") && componentInParent.isKinematic)
			{
				return null;
			}
			StickyRocket stickyRocket = UnityEngine.Object.Instantiate(prefab);
			stickyRocket.Init(collider.transform, componentInParent, info);
			return stickyRocket;
		}

		private void EnableTargetRagdoll()
		{
			if (isAttachedToCar)
			{
				return;
			}
			if (ragdollHelper != null)
			{
				ragdollHelper.Ragdolled = true;
				RagdollInteractor component = ragdollHelper.GetComponent<RagdollInteractor>();
				if (component != null)
				{
					component.standUpBlocked = true;
				}
			}
			if (vehicleComponents != null)
			{
				vehicleComponents.KickOffCurrentDriver(relocateCharacter: false);
			}
		}

		private static bool IsCar(VehicleComponents vehicleComponents)
		{
			if (vehicleComponents != null)
			{
				if (vehicleComponents.type != 0)
				{
					return vehicleComponents.type == VehicleType.Tank;
				}
				return true;
			}
			return false;
		}

		private static RagdollHelper GetRagdollHelper(VehicleComponents vehicleComponents, Transform transform)
		{
			if (vehicleComponents != null && !IsCar(vehicleComponents))
			{
				if (!(vehicleComponents.driver != null))
				{
					return null;
				}
				return vehicleComponents.driver.GetComponent<RagdollHelper>();
			}
			return transform.GetComponentInParent<RagdollHelper>();
		}

		private static bool GetTargetCollider(Ray ray, float maxDistance, out TargetColliderInfo info)
		{
			info = default(TargetColliderInfo);
			if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
			{
				return false;
			}
			Collider collider = hitInfo.collider;
			VehicleComponents componentInParent = collider.GetComponentInParent<VehicleComponents>();
			bool flag = info.isCar = IsCar(componentInParent);
			info.hit = hitInfo;
			info.vehicleComponents = componentInParent;
			if (flag)
			{
				return true;
			}
			RagdollHelper ragdollHelper = info.ragdollHelper = GetRagdollHelper(componentInParent, collider.transform);
			if (ragdollHelper == null)
			{
				return false;
			}
			if (ragdollHelper.CompareTag("Player"))
			{
				return false;
			}
			if (collider.CompareTag("RagdollPart"))
			{
				return true;
			}
			tempBodyParts.Clear();
			foreach (RagdollHelper.BodyPart bodyPart in ragdollHelper.BodyParts)
			{
				if ((bool)bodyPart.collider && !bodyPart.collider.enabled)
				{
					bodyPart.collider.enabled = true;
					tempBodyParts.Add(bodyPart);
				}
			}
			int num = Physics.RaycastNonAlloc(hitInfo.point, ray.direction, raycastHits, 1f, 1);
			foreach (RagdollHelper.BodyPart tempBodyPart in tempBodyParts)
			{
				tempBodyPart.collider.enabled = false;
			}
			float num2 = float.MaxValue;
			bool result = false;
			for (int i = 0; i < num; i++)
			{
				RaycastHit hit = raycastHits[i];
				if (hit.distance < num2)
				{
					num2 = hit.distance;
					result = hit.collider.CompareTag("RagdollPart");
					info.hit = hit;
				}
			}
			return result;
		}

		private static Vector3 AdjustDirection(Vector3 normal)
		{
			return Vector3.Slerp(normal, Vector3.down, 0.3f);
		}

		private void Init(Transform parent, Rigidbody rigidbody, TargetColliderInfo info)
		{
			GetComponent<Damage>().enabled = false;
			isAttachedToCar = info.isCar;
			ragdollHelper = info.ragdollHelper;
			vehicleComponents = info.vehicleComponents;
			force = (info.isCar ? thrustForceOnCars : thrustForceOnHumans);
			targetRigidbody = rigidbody;
			base.transform.SetParent(parent);
			base.transform.position = info.hit.point;
			Vector3 forward = AdjustDirection(info.hit.normal);
			base.transform.rotation = Quaternion.LookRotation(forward);
			thrustDirection = Vector3.up;
		}

		public void Launch()
		{
			if (Launched)
			{
				return;
			}
			GetComponent<AudioSource>().Play();
			GetComponentInChildren<ParticleSystem>().Play();
			targetRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			Launched = true;
			lastThrustUpwardsTime = Time.time - 0.2f;
			EnableTargetRagdoll();
			if (ragdollHelper != null)
			{
				AudioSource component = ragdollHelper.GetComponent<AudioSource>();
				if (!component.isPlaying)
				{
					component.Play();
				}
			}
			GetComponent<Damage>().enabled = true;
		}

		private void OnDisable()
		{
			if (!(base.gameObject == null))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void FixedUpdate()
		{
			if (Launched && !(targetRigidbody == null))
			{
				thrustDirection = -base.transform.forward;
				targetRigidbody.AddForceAtPosition(thrustDirection * force, base.transform.position, ForceMode.Force);
				if (thrustDirection.y >= 0f)
				{
					lastThrustUpwardsTime = Time.time;
				}
				if (targetRigidbody.velocity.y < 4f && Time.time < lastThrustUpwardsTime + 0.5f)
				{
					targetRigidbody.AddForceAtPosition(Vector3.up * force, base.transform.position, ForceMode.Force);
				}
			}
		}
	}
}
