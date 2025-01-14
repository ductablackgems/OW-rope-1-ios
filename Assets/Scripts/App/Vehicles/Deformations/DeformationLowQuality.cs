using System.Collections;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	public class DeformationLowQuality : VehicleDeformationBase
	{
		public bool isWreck;

		private float yScaleOrigin;

		private float yScaleSquashed = 0.5f;

		protected override void Initialize()
		{
			base.Initialize();
			vehicleComponents = GetComponent<VehicleComponents>();
			yScaleOrigin = base.transform.localScale.y;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!isTankCollision && string.Compare(other.name, "tankSquashArea") == 0)
			{
				isTankCollision = true;
				OnTankCollision();
			}
		}

		private void OnTankCollision()
		{
			charactersInVehicle.Kill();
			DestroyDoors();
			base.transform.localScale = new Vector3(base.transform.localScale.x, yScaleSquashed, base.transform.localScale.z);
			if (!isWreck)
			{
				StartCoroutine(LateDamage());
			}
		}

		private IEnumerator LateDamage()
		{
			yield return new WaitForSeconds(1f);
			health.Squash(isCritical: false, 9);
		}
	}
}
