using System.Collections;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
	public class LiquidSpawner : MonoBehaviour
	{
		[Header("GameObject")]
		public GameObject liquid;

		public Transform liquidParent;

		[Header("Positioning/Velocity")]
		public Vector3 offset;

		public Vector3 direction;

		public float speed = 4f;

		public float spread = 0.4f;

		[Header("Rate")]
		public float spawnRate;

		private void OnEnable()
		{
			StartCoroutine(SpawnLiquid());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private IEnumerator SpawnLiquid()
		{
			if (liquid != null)
			{
				while (true)
				{
					Object.Instantiate(liquid, base.transform.position + base.transform.rotation * offset, Quaternion.identity, liquidParent).GetComponent<Rigidbody>().velocity = Vector3.Lerp(base.transform.rotation * direction.normalized, UnityEngine.Random.insideUnitSphere, spread) * speed;
					yield return new WaitForSeconds(1f / Mathf.Max(0.001f, spawnRate));
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(base.transform.position + base.transform.rotation * offset, 0.2f);
		}
	}
}
