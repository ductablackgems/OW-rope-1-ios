using UnityEngine;

namespace com.ootii.Geometry
{
	public class MeshOctree
	{
		public string Name = "";

		public MeshOctreeNode Root;

		public MeshOctree()
		{
		}

		public MeshOctree(Mesh rMesh)
		{
			Initialize(rMesh);
		}

		public void Initialize(Mesh rMesh)
		{
			if (!(rMesh == null))
			{
				Name = rMesh.name;
				float num = Mathf.Max(rMesh.bounds.size.x, Mathf.Max(rMesh.bounds.size.y, rMesh.bounds.size.z));
				Root = new MeshOctreeNode(rMesh.bounds.center, new Vector3(num, num, num));
				Root.MeshVertices = rMesh.vertices;
				Root.MeshTriangles = rMesh.triangles;
				int num2 = Root.MeshTriangles.Length / 3;
				for (int i = 0; i < num2; i++)
				{
					Root.Insert(i * 3);
				}
			}
		}

		public bool ContainsPoint(Vector3 rPoint)
		{
			if (Root == null)
			{
				return false;
			}
			return Root.ContainsPoint(rPoint);
		}

		public Vector3 ClosestPoint(Vector3 rPoint)
		{
			if (Root == null)
			{
				return Vector3.zero;
			}
			Vector3 min = Root.Min;
			Vector3 max = Root.Max;
			if (rPoint.x < min.x)
			{
				rPoint.x = min.x;
			}
			else if (rPoint.x > max.x)
			{
				rPoint.x = max.x;
			}
			if (rPoint.y < min.y)
			{
				rPoint.y = min.y;
			}
			else if (rPoint.y > max.y)
			{
				rPoint.y = max.y;
			}
			if (rPoint.z < min.z)
			{
				rPoint.z = min.z;
			}
			else if (rPoint.z > max.z)
			{
				rPoint.z = max.z;
			}
			return Root.ClosestPoint(rPoint);
		}

		public Vector3 ClosestPoint(Vector3 rPoint, float rRadius)
		{
			if (Root == null)
			{
				return Vector3.zero;
			}
			Vector3 min = Root.Min;
			Vector3 max = Root.Max;
			if (rPoint.x < min.x)
			{
				rPoint.x = min.x;
			}
			else if (rPoint.x > max.x)
			{
				rPoint.x = max.x;
			}
			if (rPoint.y < min.y)
			{
				rPoint.y = min.y;
			}
			else if (rPoint.y > max.y)
			{
				rPoint.y = max.y;
			}
			if (rPoint.z < min.z)
			{
				rPoint.z = min.z;
			}
			else if (rPoint.z > max.z)
			{
				rPoint.z = max.z;
			}
			return Root.ClosestPoint(rPoint, rRadius);
		}

		public void OnSceneGUI(Transform rTransform)
		{
		}
	}
}
