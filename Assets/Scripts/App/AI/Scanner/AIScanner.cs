using App.Player;
using App.Vehicles;
using UnityEngine;

namespace App.AI.Scanner
{
	public class AIScanner : AbstractAIScript
	{
		private readonly Vector3 RaycastOffset = Vector3.up * 1.6f;

		public float interval = 0.8f;

		public float maxAngle = 30f;

		public float hearDistance = 25f;

		public AIScannerZonesScriptableObject zones;

		public Transform raycastOrigin;

		private PlayerModel player;

		private DurationTimer checkTimer = new DurationTimer();

		private float trackedUntil = -1f;

		private ITargetManager targetManager;

		public bool HasTrackToPlayer
		{
			get;
			private set;
		}

		public static float LastNoiseTime
		{
			get;
			set;
		}

		public bool TrackShootable
		{
			get;
			private set;
		}

		public bool Shootable
		{
			get;
			private set;
		}

		public void SetTrackShootable(bool trackShootable)
		{
			TrackShootable = trackShootable;
			if (!TrackShootable)
			{
				Shootable = false;
			}
		}

		public void UpdateByOtherScanner(AIScanner otherScanner)
		{
			if (otherScanner.HasTrackToPlayer && otherScanner.trackedUntil > trackedUntil)
			{
				HasTrackToPlayer = true;
				trackedUntil = otherScanner.trackedUntil;
			}
		}

		private void Awake()
		{
			player = ServiceLocator.GetPlayerModel();
			targetManager = GetComponent<ITargetManager>();
		}

		private void Start()
		{
			checkTimer.FakeDone(interval);
		}

		private void Update()
		{
			if (!checkTimer.Done())
			{
				return;
			}
			checkTimer.Run(interval);
			bool flag = false;
			bool flag2 = false;
			GameObject target = GetTarget();
			float num = Vector3.Distance(base.ComponentsRoot.position, target.transform.position);
			if (TrackShootable)
			{
				flag = Hit(num, out bool vehicleHit);
				flag2 = true;
				Shootable = (flag && !vehicleHit);
			}
			if (num < hearDistance && LastNoiseTime >= Time.time - 2f)
			{
				trackedUntil = Time.time + 4f;
				HasTrackToPlayer = true;
				return;
			}
			HasTrackToPlayer = (trackedUntil >= Time.time);
			AIScannerZone zone = zones.GetZone((base.ComponentsRoot.position - player.Transform.position).magnitude);
			if (zone == null || (!HasTrackToPlayer && !zone.trackNew) || (!HasTrackToPlayer && !zone.use360Angle && !CheckAngle()))
			{
				return;
			}
			if (!(HasTrackToPlayer ? zone.useRaycastTracked : zone.useRaycast))
			{
				trackedUntil = Time.time + zone.keepTrackDuration;
				HasTrackToPlayer = true;
				return;
			}
			if (!flag2)
			{
				flag = Hit(zone.maxDistance, out bool _);
			}
			if (flag)
			{
				trackedUntil = Time.time + zone.keepTrackDuration;
				HasTrackToPlayer = true;
			}
		}

		private bool Hit(float distance, out bool vehicleHit)
		{
			GameObject target = GetTarget();
			vehicleHit = false;
			Vector3 normalized = (target.transform.position - base.ComponentsRoot.position).normalized;
			VehicleComponents vehicle = player.PlayerMonitor.GetVehicle();
			Vector3 start;
			if (raycastOrigin == null)
			{
				Vector3 b = (vehicle == null) ? RaycastOffset : (Vector3.up * 2f);
				start = base.ComponentsRoot.position + b + normalized * 0.5f;
			}
			else
			{
				start = raycastOrigin.position;
			}
			Vector3 vector = (vehicle == null) ? RaycastOffset : Vector3.up;
			vector = ((target == player.GameObject) ? vector : Vector3.zero);
			int layerMask = 1063937;
			if (!Physics.Linecast(start, target.transform.position + vector, out RaycastHit hitInfo, layerMask))
			{
				return false;
			}
			WhoIsResult whoIsResult = WhoIs.Resolve(hitInfo.collider, WhoIs.Masks.AIScanner);
			if (whoIsResult.IsEmpty)
			{
				return false;
			}
			if (whoIsResult.Compare(WhoIs.Masks.AllVehicles) && vehicle != null && vehicle.transform.Equals(whoIsResult.transform))
			{
				vehicleHit = true;
			}
			return true;
		}

		private bool CheckAngle()
		{
			return Vector3.Angle(base.ComponentsRoot.forward, player.Transform.position - base.ComponentsRoot.position) <= maxAngle;
		}

		private GameObject GetTarget()
		{
			if (targetManager == null)
			{
				return player.GameObject;
			}
			GameObject target = targetManager.GetTarget();
			if (!(target != null))
			{
				return player.GameObject;
			}
			return target;
		}
	}
}
