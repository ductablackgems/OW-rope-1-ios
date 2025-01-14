using UnityEngine;

namespace Forge3D
{
	public class F3DPlayerController : MonoBehaviour
	{
		public F3DTurret[] Turret;

		public bool DebugDrawTarget = true;

		private Vector3 targetPos;

		private void Start()
		{
		}

		private void Update()
		{
			for (int i = 0; i < Turret.Length && (bool)Turret[i]; i++)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Turret[i].PlayAnimation();
				}
				else if (Input.GetMouseButtonDown(1))
				{
					Turret[i].PlayAnimationLoop();
				}
				else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
				{
					Turret[i].StopAnimation();
				}
				Turret[i].SetNewTarget(GetNewTargetPos());
			}
		}

		private Vector3 GetNewTargetPos()
		{
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo))
			{
				targetPos = hitInfo.point;
				return targetPos;
			}
			return Vector3.zero;
		}

		private void OnDrawGizmos()
		{
			if (DebugDrawTarget && targetPos != Vector3.zero)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(targetPos, 0.5f);
			}
		}
	}
}
