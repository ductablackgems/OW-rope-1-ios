using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Icosahedron
	{
		public Vector3[] Vertices;

		public int[] Triangles;

		public Icosahedron()
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
			float[] array = new float[36]
			{
				0.500001f,
				0f,
				-0.309017f,
				0.500001f,
				-0f,
				0.309017f,
				-0.500001f,
				-0f,
				0.309017f,
				-0.500001f,
				0f,
				-0.309017f,
				0f,
				-0.309017f,
				0.500001f,
				0f,
				0.309017f,
				0.500001f,
				0f,
				0.309017f,
				-0.500001f,
				0f,
				-0.309017f,
				-0.500001f,
				-0.309017f,
				-0.500001f,
				-0f,
				0.309017f,
				-0.500001f,
				-0f,
				0.309017f,
				0.500001f,
				0f,
				-0.309017f,
				0.500001f,
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
			return new int[60]
			{
				1,
				9,
				0,
				0,
				10,
				1,
				0,
				7,
				6,
				0,
				6,
				10,
				0,
				9,
				7,
				4,
				1,
				5,
				9,
				1,
				4,
				1,
				10,
				5,
				3,
				8,
				2,
				2,
				11,
				3,
				4,
				5,
				2,
				2,
				8,
				4,
				5,
				11,
				2,
				6,
				7,
				3,
				3,
				11,
				6,
				3,
				7,
				8,
				4,
				8,
				9,
				5,
				10,
				11,
				6,
				11,
				10,
				7,
				9,
				8
			};
		}
	}
}
