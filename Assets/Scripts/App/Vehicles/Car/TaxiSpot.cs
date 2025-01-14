using App.AI;
using App.Spawn;
using App.Spawn.Pooling;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.Vehicles.Car
{
	public class TaxiSpot : MonoBehaviour
	{
		private const float VisibleDistance = 80f;

		public Transform waiterSpawnPosition;

		public GameObject spotObject;

		private EnemyPooler enemyPooler;

		private Transform player;

		[NonSerialized]
		public bool isWaiter;

		[NonSerialized]
		public Rigidbody vehicleRigidbody;

		[NonSerialized]
		public Transform doorHandle;

		[NonSerialized]
		public int missionKey;

		public GameObject WaitingCustomer
		{
			get;
			private set;
		}

		private void Awake()
		{
			enemyPooler = ServiceLocator.Get<EnemyPooler>();
			player = ServiceLocator.GetGameObject("Player").transform;
		}

		private void OnEnable()
		{
			spotObject.SetActive(value: true);
		}

		private void OnDisable()
		{
			WaitingCustomer = null;
			spotObject.SetActive(value: false);
		}

		private void Update()
		{
			if (isWaiter && WaitingCustomer == null)
			{
				Resetor resetor = enemyPooler.Pop();
				if (resetor != null)
				{
					WaitingCustomer = resetor.gameObject;
					WaitingCustomer.transform.position = waiterSpawnPosition.position;
					WaitingCustomer.transform.rotation = waiterSpawnPosition.rotation;
					WaitingCustomer.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = false;
					WaitingCustomer.GetComponentSafe<NavMeshAgent>().enabled = true;
					TaxiStopperAIModule componentInChildrenSafe = WaitingCustomer.GetComponentInChildrenSafe<TaxiStopperAIModule>();
					componentInChildrenSafe.targetHandle = doorHandle;
					componentInChildrenSafe.carRigidbody = vehicleRigidbody;
					componentInChildrenSafe.missionKey = missionKey;
					componentInChildrenSafe.enabled = true;
				}
			}
		}
	}
}
