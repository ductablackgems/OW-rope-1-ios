using UnityEngine;

namespace App.Enemies
{
	public class EnemyExplosion : MonoBehaviour, IExplosion
	{
		public GameObject burnedObject;

		public void Explode()
		{
			burnedObject.transform.parent = null;
			burnedObject.SetActive(value: true);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
