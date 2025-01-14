using UnityEngine;

namespace App.Player
{
	public class PlayerExplosion : MonoBehaviour, IExplosion
	{
		public GameObject notBurnedGO;

		public GameObject burnedGO;

		public void Explode()
		{
			notBurnedGO.SetActive(value: false);
			burnedGO.SetActive(value: true);
		}
	}
}
