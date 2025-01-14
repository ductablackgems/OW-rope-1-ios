using App.AI;
using App.Vehicles.Mech;
using System;
using UnityEngine;

namespace App.Spawn
{
	public sealed class AIMechSpawner : AIVehicleSpawner
	{
		public float AirborneOffset = 30f;

		private int layerMask;

		private RaycastHit[] hits = new RaycastHit[4];

		protected override void OnInitialized()
		{
			base.OnInitialized();
			layerMask = LayerMask.GetMask("Ground", "Climbable", "Impact");
		}

		protected override Vector3 FindPosition(AIController controller, NavMeshUtils.ScanInput input)
		{
			bool flag = controller.GetComponent<MechJetpack>() != null;
			input.Validate = (flag ? new Func<Vector3, bool>(ValidateCandidateCallback) : null);
			Vector3 vector = NavMeshUtils.FindValidNavMeshPosition(input);
			if (flag && vector != Vector3.zero)
			{
				vector.y += AirborneOffset;
			}
			return vector;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		private bool ValidateCandidateCallback(Vector3 position)
		{
			Vector3 spawnPosition = position + Vector3.up * AirborneOffset;
			return CheckLandingPath(spawnPosition, position);
		}

		private bool CheckLandingPath(Vector3 spawnPosition, Vector3 landingPosition)
		{
			Vector3 vector = landingPosition - spawnPosition;
			float magnitude = vector.magnitude;
			int num = Physics.SphereCastNonAlloc(spawnPosition, 4f, vector.normalized, hits, magnitude, layerMask, QueryTriggerInteraction.Ignore);
			if (num == 0)
			{
				return false;
			}
			RaycastHit raycastHit = default(RaycastHit);
			raycastHit.distance = float.MaxValue;
			RaycastHit raycastHit2 = raycastHit;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit3 = hits[i];
				if (!(raycastHit3.distance > raycastHit2.distance))
				{
					raycastHit2 = raycastHit3;
				}
			}
			if (raycastHit2.collider == null)
			{
				return false;
			}
			return raycastHit2.collider.gameObject.layer == 20;
		}
	}
}
