using System.Collections;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	public class Destroy : MonoBehaviour
	{
		private int clearDistance = 100;

		private Transform playerTransform;

		private float playerDistance;

		private float interval = 1f;

		public void SetClearDistance(int value)
		{
			clearDistance = value;
		}

		private void Start()
		{
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			StartCoroutine(CheckDistance());
		}

		private IEnumerator CheckDistance()
		{
			while (true)
			{
				yield return new WaitForSeconds(interval);
				playerDistance = Vector3.Distance(base.transform.position, playerTransform.position);
				if (playerDistance > (float)clearDistance)
				{
					StopCoroutine(CheckDistance());
					base.gameObject.SetActive(value: false);
				}
			}
		}
	}
}
