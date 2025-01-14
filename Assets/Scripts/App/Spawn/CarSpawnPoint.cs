using App.Util;
using UnityEngine;

namespace App.Spawn
{
	public class CarSpawnPoint : SpawnPoint
	{
		[Tooltip("Specify the Vehicle ID, if you want to ignore the probability table. This Vehicle will be always spawned.")]
		[SerializeField]
		private string vehicleID;

		[Space]
		[Header("EDITOR")]
		[SerializeField]
		private Bounds bounds;

		private DurationTimer timer = new DurationTimer();

		public string VehicleID => vehicleID;

		public Bounds Bounds => bounds;

		public DurationTimer DespawnTimer => timer;

		public GameObject Vehicle
		{
			get;
			set;
		}

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Color blue = Color.blue;
			blue.a = 0.5f;
			Gizmos.color = blue;
			Gizmos.DrawSphere(base.Position, 1f);
			blue.a = 0.25f;
			GizmoUtils.DrawBounds(base.transform, bounds, blue);
		}
	}
}
