using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Cube
	{
		public Vector3[] Vertices;

		public int[] Triangles;

		public Cube()
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
			float[] array = new float[24]
			{
				-0.5f,
				-0.5f,
				0.5f,
				0.5f,
				-0.5f,
				0.5f,
				-0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				-0.5f,
				0.5f,
				-0.5f,
				0.5f,
				0.5f,
				-0.5f,
				-0.5f,
				-0.5f,
				-0.5f,
				0.5f,
				-0.5f,
				-0.5f
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
			return new int[36]
			{
				3,
				2,
				0,
				3,
				0,
				1,
				3,
				5,
				2,
				2,
				5,
				4,
				7,
				6,
				4,
				7,
				4,
				5,
				1,
				0,
				6,
				1,
				6,
				7,
				3,
				1,
				5,
				1,
				7,
				5,
				2,
				6,
				0,
				6,
				2,
				4
			};
		}
	}
}
