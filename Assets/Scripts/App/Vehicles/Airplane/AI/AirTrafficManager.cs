using App.Player;
using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class AirTrafficManager : MonoBehaviour
	{
		private const float CHECK_AVAILABILITY_INTERVAL = 5f;

		[SerializeField]
		private AirplaneAIController[] prefabs;

		[SerializeField]
		private Bounds activationBounds;

		[Header("Timing")]
		[SerializeField]
		private FloatRange startAfterZoneEnter = new FloatRange(5f, 10f);

		[SerializeField]
		private FloatRange nextTrafficStart = new FloatRange(30f, 60f);

		private PlayerModel player;

		private AirTraffic activeTraffic;

		private List<AirTraffic> commonTraffics = new List<AirTraffic>(4);

		private List<AirTraffic> fallbackTraffics = new List<AirTraffic>(4);

		private DurationTimer timer = new DurationTimer();

		private bool isActive;

		private void Awake()
		{
			player = ServiceLocator.GetPlayerModel();
			AirTraffic[] componentsInChildren = GetComponentsInChildren<AirTraffic>();
			foreach (AirTraffic airTraffic in componentsInChildren)
			{
				airTraffic.Initalize(OnAirTrafficFinished);
				if (airTraffic is FlybyAirTraffic)
				{
					fallbackTraffics.Add(airTraffic);
				}
				else
				{
					commonTraffics.Add(airTraffic);
				}
			}
		}

		private bool GetActiveState()
		{
			bool isPlayerInRange = GetIsPlayerInRange();
			if (isPlayerInRange == isActive)
			{
				return isActive;
			}
			isActive = isPlayerInRange;
			if (!isActive)
			{
				timer.Stop();
				StopCurrentTrafic(force: true);
			}
			return isActive;
		}

		private void Update()
		{
			if (GetActiveState() && !(activeTraffic != null))
			{
				if (!timer.Running())
				{
					timer.Run(startAfterZoneEnter.GetRandomValue());
				}
				if (timer.Done())
				{
					RunTraffic();
				}
			}
		}

		private void OnAirTrafficFinished(AirTraffic traffic)
		{
			traffic.Stop();
			activeTraffic = null;
			timer.Run(nextTrafficStart.GetRandomValue());
		}

		private bool GetIsPlayerInRange()
		{
			return activationBounds.Contains(player.Transform.position - base.transform.position);
		}

		private void StopAllTraffics()
		{
			foreach (AirTraffic commonTraffic in commonTraffics)
			{
				commonTraffic.Stop(force: true);
			}
		}

		private void StopCurrentTrafic(bool force = false)
		{
			if (!(activeTraffic == null))
			{
				activeTraffic.Stop(force);
				activeTraffic = null;
			}
		}

		private void RunTraffic()
		{
			if (!(activeTraffic != null))
			{
				AirTraffic traffic = GetTraffic();
				if (traffic == null)
				{
					timer.Run(5f);
					return;
				}
				activeTraffic = traffic;
				AirplaneAIController prefab = prefabs[Random.Range(0, prefabs.Length)];
				activeTraffic.Run(prefab);
			}
		}

		private AirTraffic GetTraffic()
		{
			AirTraffic randomTraffic = GetRandomTraffic(commonTraffics);
			if (!(randomTraffic != null))
			{
				return GetRandomTraffic(fallbackTraffics);
			}
			return randomTraffic;
		}

		private AirTraffic GetRandomTraffic(List<AirTraffic> list)
		{
			list.Shuffle();
			for (int i = 0; i < list.Count; i++)
			{
				AirTraffic airTraffic = list[i];
				if (airTraffic.IsReady)
				{
					return airTraffic;
				}
			}
			return null;
		}

		private void OnDrawGizmosSelected()
		{
			Color blue = Color.blue;
			blue.a = 0.25f;
			GizmoUtils.DrawBounds(base.transform, activationBounds, blue);
		}
	}
}
