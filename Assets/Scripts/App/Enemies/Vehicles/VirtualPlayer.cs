using UnityEngine;

namespace App.Enemies.Vehicles
{
	public class VirtualPlayer : MonoBehaviour
	{
		private Transform playerTransform;

		protected void Start()
		{
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
		}

		protected void Update()
		{
			base.transform.position = playerTransform.position;
		}
	}
}
