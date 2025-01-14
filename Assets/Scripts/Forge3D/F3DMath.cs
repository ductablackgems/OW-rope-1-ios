using UnityEngine;

namespace Forge3D
{
	public class F3DMath : MonoBehaviour
	{
		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
		{
			return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
		}

		public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
		{
			Vector3 lhs = Vector3.Cross(normal, referenceVector);
			return Vector3.Angle(referenceVector, otherVector) * Mathf.Sign(Vector3.Dot(lhs, otherVector));
		}
	}
}
