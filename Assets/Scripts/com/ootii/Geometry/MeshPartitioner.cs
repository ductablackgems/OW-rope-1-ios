using System.Diagnostics;
using UnityEngine;

namespace com.ootii.Geometry
{
	[AddComponentMenu("ootii/Mesh Partitioner")]
	public class MeshPartitioner : MonoBehaviour
	{
		public MeshOctree MeshOctree;

		public bool ParseOnStart;

		public float ParseTime;

		public int ParseVertexCount;

		public bool RenderOctree;

		public bool RenderMesh;

		public bool RenderTestNode;

		public bool RenderTestTriangle;

		public Vector3 TestPosition = Vector3.zero;

		public Transform TestTransform;

		public bool ShowDebug;

		public void Start()
		{
			if (!ParseOnStart)
			{
				return;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Mesh mesh = ExtractMesh();
			if (mesh != null)
			{
				int instanceID = mesh.GetInstanceID();
				if (mesh.isReadable && !MeshExt.MeshOctrees.ContainsKey(instanceID))
				{
					MeshOctree value = new MeshOctree(mesh);
					MeshExt.MeshOctrees.Add(instanceID, value);
					MeshExt.MeshParseTime.Add(instanceID, 0f);
				}
			}
			stopwatch.Stop();
			ParseTime = (float)stopwatch.ElapsedTicks / 1E+07f;
		}

		public Mesh ExtractMesh()
		{
			Mesh mesh = null;
			MeshCollider component = base.gameObject.GetComponent<MeshCollider>();
			if (component != null)
			{
				mesh = component.sharedMesh;
			}
			component = base.gameObject.GetComponentInChildren<MeshCollider>();
			if (component != null)
			{
				mesh = component.sharedMesh;
			}
			if (mesh == null)
			{
				mesh = base.gameObject.GetComponent<MeshFilter>().sharedMesh;
			}
			if (mesh != null && !mesh.isReadable)
			{
				mesh = null;
			}
			return mesh;
		}
	}
}
