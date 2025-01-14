using App.Spawn;
using UnityEngine;

namespace App.Camera
{
	public class CameraAnchor : MonoBehaviour
	{
		public float transitionSpeed = 0.8f;

		private PlayerRespawner respawner;

		private Vector3 remainDistance;

		private bool respawned = true;

		public Transform Target
		{
			get;
			private set;
		}

		public Vector3 Offset
		{
			get;
			private set;
		}

		public void Set(Transform target, Vector3 offset)
		{
			if (Target != null && target != null && (Target != target || Offset != offset))
			{
				Vector3 a = Target.position + Offset;
				Vector3 b = target.position + offset;
				if (respawned)
				{
					respawned = false;
					remainDistance = Vector3.zero;
				}
				else
				{
					remainDistance = a - b;
					float num = 1f;
					if (remainDistance.magnitude > num)
					{
						remainDistance = remainDistance.normalized * num;
					}
				}
			}
			Target = target;
			Offset = offset;
			if (Target != null)
			{
				base.transform.position = Target.position + Offset + remainDistance;
			}
		}

		public void TeleportToTarget()
		{
			if (!(Target == null))
			{
				base.transform.position = Target.position + Offset;
				remainDistance = Vector3.zero;
			}
		}

		private void Awake()
		{
			if (Target != null)
			{
				base.transform.position = Target.position + Offset + remainDistance;
			}
			respawner = ServiceLocator.Get<PlayerRespawner>(showError: false);
			if (respawner != null)
			{
				respawner.AfterRespawn += OnRespawn;
			}
		}

		private void OnDestroy()
		{
			if (respawner != null)
			{
				respawner.AfterRespawn -= OnRespawn;
			}
		}

		private void OnRespawn()
		{
			respawned = true;
			remainDistance = Vector3.zero;
		}

		private void Update()
		{
			remainDistance = Vector3.MoveTowards(remainDistance, Vector3.zero, Time.deltaTime * transitionSpeed);
			if (Target != null)
			{
				base.transform.position = Target.position + Offset + remainDistance;
			}
		}

		private void FixedUpdate()
		{
			if (Target != null)
			{
				base.transform.position = Target.position + Offset + remainDistance;
			}
		}

		private void LateUpdate()
		{
			if (Target != null)
			{
				base.transform.position = Target.position + Offset + remainDistance;
			}
		}
	}
}
