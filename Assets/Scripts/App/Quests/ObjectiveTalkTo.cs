using App.AI;
using App.Interaction;
using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveTalkTo : GameplayObjective
	{
		[Header("Objective Talk To")]
		[SerializeField]
		private GameObject inSceneTarget;

		[Header("Spawn")]
		[SerializeField]
		private EnemyType humanToSpawn;

		[SerializeField]
		private bool despawnOnFinish;

		private GameObject npcTarget;

		private InteractiveObject npcInteraction;

		private DestroyGameObject npcDestroyer;

		private HumanSpawner spawner;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			npcTarget = inSceneTarget;
			spawner = ServiceLocator.Get<HumanSpawner>();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			if (npcTarget == null)
			{
				Resetor resetor = spawner.Spawn(humanToSpawn, base.Position, isWalking: false, force: true, SelectBestHumanToSpawn);
				npcTarget = resetor.gameObject;
			}
			data.NPC = npcTarget.GetComponent<Health>();
			npcDestroyer = npcTarget.GetComponent<DestroyGameObject>();
			if (npcDestroyer != null)
			{
				npcDestroyer.enabled = false;
			}
			npcInteraction = npcTarget.GetComponent<InteractiveObject>();
			if (npcInteraction == null)
			{
				npcInteraction = npcTarget.AddComponent<InteractiveObject>();
			}
			npcInteraction.Interaction += OnInteraction;
			npcTarget.SetActive(value: true);
			base.Player.PlayerMonitor.InteractiveObjectSensor.Register(npcInteraction);
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsNPCKilled)
			{
				Fail();
			}
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			if (!(npcTarget == null))
			{
				if (npcInteraction != null)
				{
					base.Player.PlayerMonitor.InteractiveObjectSensor.Unregister(npcInteraction);
					npcInteraction.Interaction -= OnInteraction;
					UnityEngine.Object.Destroy(npcInteraction);
				}
				if (npcDestroyer != null)
				{
					npcDestroyer.enabled = true;
				}
				if (despawnOnFinish)
				{
					spawner.Despawn(npcTarget);
				}
				else
				{
					npcTarget.GetComponentInChildren<FreeWalkAIModule>().enabled = true;
				}
				data.CleanNPC();
				npcTarget = null;
			}
		}

		protected override Vector3 GetNavigationPosition()
		{
			if (!(npcTarget != null))
			{
				return base.GetNavigationPosition();
			}
			return npcTarget.transform.position;
		}

		private void OnInteraction()
		{
			Finish();
		}

		private Resetor SelectBestHumanToSpawn(List<Resetor> candidates)
		{
			float num = 0f;
			Vector3 position = base.Player.Transform.position;
			Resetor result = null;
			foreach (Resetor candidate in candidates)
			{
				if (!(candidate == null))
				{
					float num2 = Vector3.Distance(candidate.transform.position, position);
					if (!(num2 < num))
					{
						num = num2;
						result = candidate;
					}
				}
			}
			return result;
		}
	}
}
