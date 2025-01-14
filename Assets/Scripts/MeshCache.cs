using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class MeshCache
{
	private static List<MeshCache> Cache = new List<MeshCache>();

	public Mesh Mesh;

	public Vector3[] Vertices;

	public Color[] Colors;

	public Vector3 MeshSize;

	public Vector3 SizeFactor;

	public Bounds Bounds;

	public bool HasVertexColor
	{
		get
		{
			if (Colors != null)
			{
				return Colors.Length != 0;
			}
			return false;
		}
	}

	public static MeshCache GetMeshCache(Mesh sharedMesh)
	{
		MeshCache meshCache = (from c in Cache
			where c.Mesh == sharedMesh
			select c).FirstOrDefault();
		if (meshCache == null)
		{
			meshCache = new MeshCache(sharedMesh);
			Cache.Add(meshCache);
		}
		return meshCache;
	}

	private MeshCache(Mesh mesh)
	{
		if (mesh != null)
		{
			Mesh = mesh;
			Vertices = mesh.vertices;
			Colors = mesh.colors;
			Bounds = mesh.bounds;
			MeshSize = mesh.bounds.size;
			MeshSize.x = Mathf.Max(MeshSize.x, 0.1f);
			MeshSize.y = Mathf.Max(MeshSize.y, 0.1f);
			MeshSize.z = Mathf.Max(MeshSize.z, 0.1f);
			float num = Mathf.Max(MeshSize.x, MeshSize.y, MeshSize.z);
			SizeFactor = new Vector3(1f / (MeshSize.x / num), 1f / (MeshSize.y / num), 1f / (MeshSize.z / num));
		}
	}
}
