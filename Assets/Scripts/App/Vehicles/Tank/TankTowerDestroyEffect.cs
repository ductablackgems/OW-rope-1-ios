using UnityEngine;

namespace App.Vehicles.Tank
{
	public class TankTowerDestroyEffect : MonoBehaviour
	{
		public Rigidbody towerRigidbody;

		private void Update()
		{
			base.enabled = false;
			Rigidbody componentSafe = this.GetComponentSafe<Rigidbody>();
			towerRigidbody.velocity = componentSafe.velocity + base.transform.up * 15f + base.transform.right * UnityEngine.Random.Range(-5f, 5f) + base.transform.forward * UnityEngine.Random.Range(-5f, 5f);
			towerRigidbody.transform.parent = null;
			towerRigidbody.tag = "Untagged";
			UnityEngine.Object.Destroy(towerRigidbody.gameObject, 12f);
		}
	}
}
