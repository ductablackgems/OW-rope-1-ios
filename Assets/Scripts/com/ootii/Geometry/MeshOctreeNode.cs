using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Geometry
{
	public class MeshOctreeNode
	{
		public const int MAX_TRIANGLES = 20;

		public const float MIN_NODE_SIZE = 0.05f;

		public const float EPSILON = 1E-05f;

		private static List<int> sClosestTrianglesIndexes = new List<int>();

		public Vector3 Center = Vector3.zero;

		public Vector3 Size = Vector3.zero;

		public Vector3 Min = Vector3.zero;

		public Vector3 Max = Vector3.zero;

		public MeshOctreeNode[] Children;

		public List<int> TriangleIndexes;

		public Vector3[] MeshVertices;

		public int[] MeshTriangles;

		public MeshOctreeNode()
		{
		}

		public MeshOctreeNode(Vector3 rCenter, Vector3 rSize)
		{
			Center = rCenter;
			Size = rSize;
			Min = Center - Size * 0.5f;
			Max = Center + Size * 0.5f;
		}

		public MeshOctreeNode(float rX, float rY, float rZ, Vector3 rSize)
		{
			Center = new Vector3(rX, rY, rZ);
			Size = rSize;
			Min = Center - Size * 0.5f;
			Max = Center + Size * 0.5f;
		}

		public MeshOctreeNode(float rX, float rY, float rZ, Vector3 rSize, Vector3[] rVertexArray, int[] rTriangleArray)
		{
			Center = new Vector3(rX, rY, rZ);
			Size = rSize;
			Min = Center - Size * 0.5f;
			Max = Center + Size * 0.5f;
			MeshVertices = rVertexArray;
			MeshTriangles = rTriangleArray;
		}

		public bool ContainsPoint(Vector3 rPoint)
		{
			if (rPoint.x + 1E-05f < Min.x)
			{
				return false;
			}
			if (rPoint.x - 1E-05f > Max.x)
			{
				return false;
			}
			if (rPoint.y + 1E-05f < Min.y)
			{
				return false;
			}
			if (rPoint.y - 1E-05f > Max.y)
			{
				return false;
			}
			if (rPoint.z + 1E-05f < Min.z)
			{
				return false;
			}
			if (rPoint.z - 1E-05f > Max.z)
			{
				return false;
			}
			return true;
		}

		public bool ContainsPoint(Vector3 rPoint, float rRadius)
		{
			Vector3 vector = rPoint;
			vector.x = ((vector.x > Max.x) ? Max.x : vector.x);
			vector.x = ((vector.x < Min.x) ? Min.x : vector.x);
			vector.y = ((vector.y > Max.y) ? Max.y : vector.y);
			vector.y = ((vector.y < Min.y) ? Min.y : vector.y);
			vector.z = ((vector.z > Max.z) ? Max.z : vector.z);
			vector.z = ((vector.z < Min.z) ? Min.z : vector.z);
			return (vector - rPoint).sqrMagnitude <= rRadius * rRadius;
		}

		public Vector3 ClosestPoint(Vector3 rPoint)
		{
			Vector3 vector = Vector3.zero;
			vector.x = float.MaxValue;
			if (rPoint.x + 1E-05f < Min.x)
			{
				return vector;
			}
			if (rPoint.x - 1E-05f > Max.x)
			{
				return vector;
			}
			if (rPoint.y + 1E-05f < Min.y)
			{
				return vector;
			}
			if (rPoint.y - 1E-05f > Max.y)
			{
				return vector;
			}
			if (rPoint.z + 1E-05f < Min.z)
			{
				return vector;
			}
			if (rPoint.z - 1E-05f > Max.z)
			{
				return vector;
			}
			if (Children == null)
			{
				if (TriangleIndexes != null)
				{
					for (int i = 0; i < TriangleIndexes.Count; i++)
					{
						int num = TriangleIndexes[i];
						Vector3 vertex = MeshVertices[MeshTriangles[num]];
						Vector3 vertex2 = MeshVertices[MeshTriangles[num + 1]];
						Vector3 vertex3 = MeshVertices[MeshTriangles[num + 2]];
						MeshExt.ClosestPointOnTriangle(ref rPoint, ref vertex, ref vertex2, ref vertex3, out Vector3 result);
						if (result.x != float.MaxValue && (vector.x == float.MaxValue || Vector3.SqrMagnitude(result - rPoint) < Vector3.SqrMagnitude(vector - rPoint)))
						{
							vector = result;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < 8; j++)
				{
					Vector3 result = Children[j].ClosestPoint(rPoint);
					if (result.x != float.MaxValue && (vector.x == float.MaxValue || Vector3.SqrMagnitude(result - rPoint) < Vector3.SqrMagnitude(vector - rPoint)))
					{
						vector = result;
					}
				}
			}
			return vector;
		}

		public Vector3 ClosestPoint(Vector3 rPoint, float rRadius)
		{
			if (rRadius == 0f)
			{
				return ClosestPoint(rPoint);
			}
			Vector3 vector = Vector3.zero;
			vector.x = float.MaxValue;
			sClosestTrianglesIndexes.Clear();
			GetTriangles(rPoint, rRadius, sClosestTrianglesIndexes);
			for (int i = 0; i < sClosestTrianglesIndexes.Count; i++)
			{
				int num = sClosestTrianglesIndexes[i];
				Vector3 vertex = MeshVertices[MeshTriangles[num]];
				Vector3 vertex2 = MeshVertices[MeshTriangles[num + 1]];
				Vector3 vertex3 = MeshVertices[MeshTriangles[num + 2]];
				MeshExt.ClosestPointOnTriangle(ref rPoint, ref vertex, ref vertex2, ref vertex3, out Vector3 result);
				if (result.x != float.MaxValue && (vector.x == float.MaxValue || Vector3.SqrMagnitude(result - rPoint) < Vector3.SqrMagnitude(vector - rPoint)))
				{
					vector = result;
				}
			}
			return vector;
		}

		public int ClosestTriangle(Vector3 rPoint)
		{
			int result = -1;
			if (rPoint.x + 1E-05f < Min.x)
			{
				return result;
			}
			if (rPoint.x - 1E-05f > Max.x)
			{
				return result;
			}
			if (rPoint.y + 1E-05f < Min.y)
			{
				return result;
			}
			if (rPoint.y - 1E-05f > Max.y)
			{
				return result;
			}
			if (rPoint.z + 1E-05f < Min.z)
			{
				return result;
			}
			if (rPoint.z - 1E-05f > Max.z)
			{
				return result;
			}
			Vector3 vector = Vector3.zero;
			vector.x = float.MaxValue;
			if (Children == null)
			{
				if (TriangleIndexes != null)
				{
					for (int i = 0; i < TriangleIndexes.Count; i++)
					{
						int num = TriangleIndexes[i];
						Vector3 vertex = MeshVertices[MeshTriangles[num]];
						Vector3 vertex2 = MeshVertices[MeshTriangles[num + 1]];
						Vector3 vertex3 = MeshVertices[MeshTriangles[num + 2]];
						MeshExt.ClosestPointOnTriangle(ref rPoint, ref vertex, ref vertex2, ref vertex3, out Vector3 result2);
						if (result2.x != float.MaxValue && (vector.x == float.MaxValue || Vector3.SqrMagnitude(result2 - rPoint) < Vector3.SqrMagnitude(vector - rPoint)))
						{
							result = num;
							vector = result2;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < 8; j++)
				{
					int num2 = Children[j].ClosestTriangle(rPoint);
					if (num2 >= 0)
					{
						result = num2;
					}
				}
			}
			return result;
		}

		public MeshOctreeNode ClosestNode(Vector3 rPoint)
		{
			MeshOctreeNode meshOctreeNode = null;
			if (rPoint.x + 1E-05f < Min.x)
			{
				return meshOctreeNode;
			}
			if (rPoint.x - 1E-05f > Max.x)
			{
				return meshOctreeNode;
			}
			if (rPoint.y + 1E-05f < Min.y)
			{
				return meshOctreeNode;
			}
			if (rPoint.y - 1E-05f > Max.y)
			{
				return meshOctreeNode;
			}
			if (rPoint.z + 1E-05f < Min.z)
			{
				return meshOctreeNode;
			}
			if (rPoint.z - 1E-05f > Max.z)
			{
				return meshOctreeNode;
			}
			if (Children == null)
			{
				meshOctreeNode = this;
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					MeshOctreeNode meshOctreeNode2 = Children[i].ClosestNode(rPoint);
					if (meshOctreeNode2 != null && (meshOctreeNode == null || meshOctreeNode2.Size.sqrMagnitude < meshOctreeNode.Size.sqrMagnitude))
					{
						meshOctreeNode = meshOctreeNode2;
					}
				}
			}
			return meshOctreeNode;
		}

		public void Insert(int rTriangleIndex)
		{
			GetTriangleBounds(rTriangleIndex, out Vector3 rTriangleCenter, out Vector3 rTriangleMin, out Vector3 rTriangleMax);
			Insert(rTriangleIndex, rTriangleCenter, rTriangleMin, rTriangleMax);
		}

		public void Insert(int rTriangleIndex, Vector3 rTriangleCenter, float rTriangleRadius)
		{
			Vector3 vector = Center - rTriangleCenter;
			Vector3 vector2 = rTriangleCenter + vector.normalized * Mathf.Min(vector.magnitude, rTriangleRadius);
			if (vector2.x + 1E-05f < Min.x || vector2.x - 1E-05f > Max.x || vector2.y + 1E-05f < Min.y || vector2.y - 1E-05f > Max.y || vector2.z + 1E-05f < Min.z || vector2.z - 1E-05f > Max.z)
			{
				return;
			}
			if (Children == null)
			{
				if (TriangleIndexes == null)
				{
					TriangleIndexes = new List<int>();
				}
				if (TriangleIndexes.Count < 20 || Size.x <= 0.05f)
				{
					TriangleIndexes.Add(rTriangleIndex);
				}
				else
				{
					Split();
				}
			}
			if (Children != null)
			{
				for (int i = 0; i < 8; i++)
				{
					Children[i].Insert(rTriangleIndex, rTriangleCenter, rTriangleRadius);
				}
			}
		}

		public void Insert(int rTriangleIndex, Vector3 rTriangleCenter, Vector3 rTriangleMin, Vector3 rTriangleMax)
		{
			if (rTriangleMax.x + 1E-05f < Min.x || rTriangleMin.x - 1E-05f > Max.x || rTriangleMax.y + 1E-05f < Min.y || rTriangleMin.y - 1E-05f > Max.y || rTriangleMax.z + 1E-05f < Min.z || rTriangleMin.z - 1E-05f > Max.z)
			{
				return;
			}
			if (Children == null)
			{
				if (TriangleIndexes == null)
				{
					TriangleIndexes = new List<int>();
				}
				if (TriangleIndexes.Count < 20 || Size.x <= 0.05f)
				{
					TriangleIndexes.Add(rTriangleIndex);
				}
				else
				{
					Split();
				}
			}
			if (Children != null)
			{
				for (int i = 0; i < 8; i++)
				{
					Children[i].Insert(rTriangleIndex, rTriangleCenter, rTriangleMin, rTriangleMax);
				}
			}
		}

		public virtual void Split()
		{
			Vector3 vector = Size * 0.5f;
			Vector3 vector2 = vector * 0.5f;
			Children = new MeshOctreeNode[8];
			Children[0] = new MeshOctreeNode(Center.x - vector2.x, Center.y - vector2.y, Center.z - vector2.z, vector, MeshVertices, MeshTriangles);
			Children[1] = new MeshOctreeNode(Center.x + vector2.x, Center.y - vector2.y, Center.z - vector2.z, vector, MeshVertices, MeshTriangles);
			Children[2] = new MeshOctreeNode(Center.x - vector2.x, Center.y + vector2.y, Center.z - vector2.z, vector, MeshVertices, MeshTriangles);
			Children[3] = new MeshOctreeNode(Center.x + vector2.x, Center.y + vector2.y, Center.z - vector2.z, vector, MeshVertices, MeshTriangles);
			Children[4] = new MeshOctreeNode(Center.x - vector2.x, Center.y - vector2.y, Center.z + vector2.z, vector, MeshVertices, MeshTriangles);
			Children[5] = new MeshOctreeNode(Center.x + vector2.x, Center.y - vector2.y, Center.z + vector2.z, vector, MeshVertices, MeshTriangles);
			Children[6] = new MeshOctreeNode(Center.x - vector2.x, Center.y + vector2.y, Center.z + vector2.z, vector, MeshVertices, MeshTriangles);
			Children[7] = new MeshOctreeNode(Center.x + vector2.x, Center.y + vector2.y, Center.z + vector2.z, vector, MeshVertices, MeshTriangles);
			for (int i = 0; i < TriangleIndexes.Count; i++)
			{
				int rTriangleIndex = TriangleIndexes[i];
				GetTriangleBounds(rTriangleIndex, out Vector3 rTriangleCenter, out Vector3 rTriangleMin, out Vector3 rTriangleMax);
				for (int j = 0; j < 8; j++)
				{
					Children[j].Insert(rTriangleIndex, rTriangleCenter, rTriangleMin, rTriangleMax);
				}
			}
			TriangleIndexes.Clear();
			TriangleIndexes = null;
		}

		public void GetTriangles(Vector3 rPoint, float rRadius, List<int> rTriangles)
		{
			if (!ContainsPoint(rPoint, rRadius))
			{
				return;
			}
			if (Children == null)
			{
				if (TriangleIndexes == null)
				{
					return;
				}
				for (int i = 0; i < TriangleIndexes.Count; i++)
				{
					int item = TriangleIndexes[i];
					if (!rTriangles.Contains(item))
					{
						rTriangles.Add(item);
					}
				}
			}
			else
			{
				for (int j = 0; j < Children.Length; j++)
				{
					Children[j].GetTriangles(rPoint, rRadius, rTriangles);
				}
			}
		}

		public void GetTriangleBounds(int rTriangleIndex, out Vector3 rTriangleCenter, out float rTriangleRadius)
		{
			Vector3 a = MeshVertices[MeshTriangles[rTriangleIndex]];
			Vector3 vector = MeshVertices[MeshTriangles[rTriangleIndex + 1]];
			Vector3 vector2 = MeshVertices[MeshTriangles[rTriangleIndex + 2]];
			rTriangleCenter = (a + vector + vector2) / 3f;
			float a2 = Vector3.SqrMagnitude(a - rTriangleCenter);
			float a3 = Vector3.SqrMagnitude(vector - rTriangleCenter);
			float b = Vector3.SqrMagnitude(vector2 - rTriangleCenter);
			rTriangleRadius = Mathf.Sqrt(Mathf.Max(a2, Mathf.Max(a3, b)));
		}

		public void GetTriangleBounds(int rTriangleIndex, out Vector3 rTriangleCenter, out Vector3 rTriangleMin, out Vector3 rTriangleMax)
		{
			Vector3 vector = MeshVertices[MeshTriangles[rTriangleIndex]];
			Vector3 vector2 = MeshVertices[MeshTriangles[rTriangleIndex + 1]];
			Vector3 vector3 = MeshVertices[MeshTriangles[rTriangleIndex + 2]];
			rTriangleCenter = (vector + vector2 + vector3) / 3f;
			rTriangleMin = Vector3.zero;
			rTriangleMax = Vector3.zero;
			rTriangleMin.x = Mathf.Min(vector.x, Mathf.Min(vector2.x, vector3.x));
			rTriangleMax.x = Mathf.Max(vector.x, Mathf.Max(vector2.x, vector3.x));
			rTriangleMin.y = Mathf.Min(vector.y, Mathf.Min(vector2.y, vector3.y));
			rTriangleMax.y = Mathf.Max(vector.y, Mathf.Max(vector2.y, vector3.y));
			rTriangleMin.z = Mathf.Min(vector.z, Mathf.Min(vector2.z, vector3.z));
			rTriangleMax.z = Mathf.Max(vector.z, Mathf.Max(vector2.z, vector3.z));
		}

		public void OnSceneGUI(Transform rTransform)
		{
		}
	}
}
