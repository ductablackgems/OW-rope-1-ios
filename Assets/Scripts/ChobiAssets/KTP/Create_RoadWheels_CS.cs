using UnityEngine;

namespace ChobiAssets.KTP
{
	public class Create_RoadWheels_CS : MonoBehaviour
	{
		public float susDistance = 2f;

		public int num = 3;

		public float spacing = 1.05f;

		public float susLength = 0.4f;

		public float susAngle = 18f;

		public float susMass = 100f;

		public float susSpring = 15000f;

		public float susDamper = 500f;

		public float susTarget = -30f;

		public float susForwardLimit = 10f;

		public float susBackwardLimit = 30f;

		public Mesh susMesh_L;

		public Mesh susMesh_R;

		public Material susMaterial;

		public float reinforceRadius = 0.5f;

		public float wheelDistance = 2.9f;

		public float wheelMass = 100f;

		public float wheelRadius = 0.5f;

		public PhysicMaterial physicMaterial;

		public Mesh wheelMesh;

		public Material wheelMaterial;
	}
}
