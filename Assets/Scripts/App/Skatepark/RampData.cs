using UnityEngine;

namespace App.Skatepark
{
	public class RampData : ScriptableObject
	{
		public Vector3 vertNormal;

		public Vector3 copingPosition;

		public float minVertY;

		public bool isCorner;

		public bool drawGizmo;
	}
}
