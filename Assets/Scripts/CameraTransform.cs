using UnityEngine;

namespace com.ootii.Cameras
{
	public struct CameraTransform
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public float FieldOfView;

		public void Lerp(CameraTransform rFrom, CameraTransform rTo, float rTime)
		{
			Position = Vector3.Lerp(rFrom.Position, rTo.Position, rTime);
			Rotation = Quaternion.Slerp(rFrom.Rotation, rTo.Rotation, rTime);
		}
	}
}
