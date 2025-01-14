using UnityEngine;

namespace ChobiAssets.KTP
{
	public class Create_SprocketWheels_CS : MonoBehaviour
	{
		public bool useArm;

		public float armDistance = 2.2f;

		public float armLength = 0.15f;

		public float armAngle = 60f;

		public Mesh armMesh_L;

		public Mesh armMesh_R;

		public Material armMaterial;

		public float wheelDistance = 2.9f;

		public float wheelMass = 100f;

		public float wheelRadius = 0.5f;

		public PhysicMaterial physicMaterial;

		public Mesh wheelMesh;

		public Material wheelMaterial;

		public float radiusOffset;
	}
}
