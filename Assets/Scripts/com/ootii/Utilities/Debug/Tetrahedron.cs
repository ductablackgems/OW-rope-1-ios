using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Tetrahedron
	{
		public Vector3[] Vertices;

		public int[] Triangles;

		public Tetrahedron()
		{
			Vertices = CreateVertices();
			Triangles = CreateTriangles();
			Vector3[] array = new Vector3[Triangles.Length];
			for (int i = 0; i < Triangles.Length; i++)
			{
				array[i] = Vertices[Triangles[i]];
				Triangles[i] = i;
			}
			Vertices = array;
		}

		private Vector3[] CreateVertices()
		{
			int num = 3;
			float[] array = new float[12]
			{
				-0.3525f,
				-0.49851f,
				-0.610548f,
				-0.3525f,
				-0.49851f,
				0.610548f,
				0.705f,
				-0.49851f,
				-0f,
				0f,
				0.49851f,
				0f
			};
			Vector3[] array2 = new Vector3[array.Length / num];
			for (int i = 0; i < array.Length; i += num)
			{
				array2[i / num] = new Vector3(array[i], array[i + 1], array[i + 2]);
			}
			return array2;
		}

		private int[] CreateTriangles()
		{
			return new int[12]
			{
				2,
				1,
				0,
				2,
				3,
				1,
				3,
				2,
				0,
				1,
				3,
				0
			};
		}
	}
}
