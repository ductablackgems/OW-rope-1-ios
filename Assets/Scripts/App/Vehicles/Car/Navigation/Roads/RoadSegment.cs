using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class RoadSegment : MonoBehaviour
	{
		public const float GizmosYOffset = 0.1f;

		public float maxSpeed = 14f;

		public RoadSegment previousSegment;

		public RoadSegment nextSegment;

		public Vector3 endPoint;

		public Road road;

		[Space]
		public float beginTangent;

		public float endTangent;

		public float Length
		{
			get;
			private set;
		}

		public float PreviousLength
		{
			get;
			set;
		}

		public float NextLength
		{
			get;
			set;
		}

		public Vector3 GetEndPosition()
		{
			if (!(nextSegment == null))
			{
				return nextSegment.transform.position;
			}
			return base.transform.TransformPoint(endPoint);
		}

		public Vector3 GetDistance(Vector3 position)
		{
			return base.transform.InverseTransformPoint(position);
		}

		public Vector3 GetClosestPosition(Vector3 position)
		{
			float d = Mathf.Clamp(base.transform.InverseTransformPoint(position).z, 0f, Length);
			return base.transform.position + base.transform.forward * d;
		}

		private void Awake()
		{
			if (nextSegment != null)
			{
				base.transform.LookAt(nextSegment.transform);
			}
			Length = Vector3.Distance(base.transform.position, GetEndPosition());
		}

		private void OnDrawGizmos()
		{
			Transform obj = null;
			bool flag = base.transform.Equals(obj);
			Transform parent = base.transform.parent;
			while (parent != null && !flag)
			{
				if (parent.Equals(obj))
				{
					flag = true;
				}
				parent = parent.parent;
			}
			Gizmos.color = (flag ? Color.green : new Color(14f / 255f, 43f / 85f, 0f));
			Vector3 b = new Vector3(0f, 0.1f);
			Vector3 endPosition = GetEndPosition();
			Gizmos.DrawLine(base.transform.position + b, endPosition + b);
			if (!RoadsGeneratorConfig.showOnlyFirstSegmentGizmo || !(previousSegment != null))
			{
				Gizmos.DrawSphere((base.transform.position * 1.3f + endPosition * 0.7f) / 2f, 0.4f);
			}
		}
	}
}
