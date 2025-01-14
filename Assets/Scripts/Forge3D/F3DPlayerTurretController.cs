using UnityEngine;

namespace Forge3D
{
	public class F3DPlayerTurretController : MonoBehaviour
	{
		private RaycastHit hitInfo;

		public F3DTurret turret;

		private bool isFiring;

		public F3DFXController fxController;

		private void Update()
		{
			CheckForTurn();
			CheckForFire();
		}

		private void CheckForFire()
		{
			if (!isFiring && UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
			{
				isFiring = true;
				fxController.Fire();
			}
			if (isFiring && UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
			{
				isFiring = false;
				fxController.Stop();
			}
		}

		private void CheckForTurn()
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hitInfo, 500f))
			{
				turret.SetNewTarget(hitInfo.point);
			}
		}
	}
}
