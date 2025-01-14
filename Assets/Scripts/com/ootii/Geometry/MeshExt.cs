using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace com.ootii.Geometry
{
	public class MeshExt
	{
		public static Dictionary<int, MeshOctree> MeshOctrees = new Dictionary<int, MeshOctree>();

		public static Dictionary<int, float> MeshParseTime = new Dictionary<int, float>();

		public static Transform DebugTransform = null;

		public static Vector3 ClosestVertex(Vector3 rPoint, Transform rTransform, Mesh rMesh)
		{
			Vector3 result = Vector3.zero;
			float num = float.MaxValue;
			Vector3 a = rTransform.InverseTransformPoint(rPoint);
			for (int num2 = rMesh.vertices.Length - 1; num2 >= 0; num2--)
			{
				Vector3 vector = rMesh.vertices[num2];
				float sqrMagnitude = (a - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = vector;
				}
			}
			return result;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, float rRadius, Transform rTransform, Mesh rMesh)
		{
			MeshOctree meshOctree = null;
			int instanceID = rMesh.GetInstanceID();
			if (MeshOctrees.ContainsKey(instanceID))
			{
				meshOctree = MeshOctrees[instanceID];
			}
			else
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				meshOctree = new MeshOctree(rMesh);
				stopwatch.Stop();
				MeshOctrees.Add(instanceID, meshOctree);
				MeshParseTime.Add(instanceID, (float)stopwatch.ElapsedTicks / 1E+07f);
			}
			Vector3 rPoint2 = rTransform.InverseTransformPoint(rPoint);
			Vector3 vector = MeshOctrees[instanceID].ClosestPoint(rPoint2, rRadius);
			if (vector.x == float.MaxValue)
			{
				vector = Vector3Ext.Null;
			}
			if (vector != Vector3Ext.Null)
			{
				vector = rTransform.TransformPoint(vector);
			}
			return vector;
		}

		public static void ClosestPointOnTriangle(ref Vector3 point, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out Vector3 result)
		{
			Vector3 vector = vertex2 - vertex1;
			Vector3 vector2 = vertex3 - vertex1;
			Vector3 rhs = point - vertex1;
			float num = Vector3.Dot(vector, rhs);
			float num2 = Vector3.Dot(vector2, rhs);
			if (num <= 0f && num2 <= 0f)
			{
				result = vertex1;
				return;
			}
			Vector3 rhs2 = point - vertex2;
			float num3 = Vector3.Dot(vector, rhs2);
			float num4 = Vector3.Dot(vector2, rhs2);
			if (num3 >= 0f && num4 <= num3)
			{
				result = vertex2;
				return;
			}
			float num5 = num * num4 - num3 * num2;
			if (num5 <= 0f && num >= 0f && num3 <= 0f)
			{
				float d = num / (num - num3);
				result = vertex1 + d * vector;
				return;
			}
			Vector3 rhs3 = point - vertex3;
			float num6 = Vector3.Dot(vector, rhs3);
			float num7 = Vector3.Dot(vector2, rhs3);
			if (num7 >= 0f && num6 <= num7)
			{
				result = vertex3;
				return;
			}
			float num8 = num6 * num2 - num * num7;
			if (num8 <= 0f && num2 >= 0f && num7 <= 0f)
			{
				float d2 = num2 / (num2 - num7);
				result = vertex1 + d2 * vector2;
				return;
			}
			float num9 = num3 * num7 - num6 * num4;
			if (num9 <= 0f && num4 - num3 >= 0f && num6 - num7 >= 0f)
			{
				float d3 = (num4 - num3) / (num4 - num3 + (num6 - num7));
				result = vertex2 + d3 * (vertex3 - vertex2);
				return;
			}
			float num10 = 1f / (num9 + num8 + num5);
			float d4 = num8 * num10;
			float d5 = num5 * num10;
			result = vertex1 + vector * d4 + vector2 * d5;
		}
	}
}
