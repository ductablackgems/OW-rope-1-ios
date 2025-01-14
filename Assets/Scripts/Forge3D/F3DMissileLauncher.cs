using UnityEngine;
using UnityEngine.UI;

namespace Forge3D
{
	public class F3DMissileLauncher : MonoBehaviour
	{
		public static F3DMissileLauncher instance;

		public Transform missilePrefab;

		public Transform target;

		public Transform explosionPrefab;

		private F3DMissile.MissileType missileType;

		public Text missileTypeLabel;

		private void Start()
		{
			instance = this;
			missileType = F3DMissile.MissileType.Unguided;
			missileTypeLabel.text = "Missile type: Unguided";
		}

		public void SpawnExplosion(Vector3 position)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(explosionPrefab, position, Quaternion.identity, null);
		}

		private void ProcessInput()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Transform transform = F3DPoolManager.Pools["GeneratedPool"].Spawn(missilePrefab, base.transform.position + Vector3.up * 2f, Quaternion.identity, null);
				if (transform != null)
				{
					F3DMissile component = transform.GetComponent<F3DMissile>();
					component.missileType = missileType;
					if (target != null)
					{
						component.target = target;
					}
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
			{
				missileType = F3DMissile.MissileType.Unguided;
				missileTypeLabel.text = "Missile type: Unguided";
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
			{
				missileType = F3DMissile.MissileType.Guided;
				missileTypeLabel.text = "Missile type: Guided";
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
			{
				missileType = F3DMissile.MissileType.Predictive;
				missileTypeLabel.text = "Missile type: Predictive";
			}
		}

		private void Update()
		{
			ProcessInput();
		}
	}
}
