using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Bone
	{
		public static Vector3[] BoneVertices = new Vector3[6]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0.1f, 0.1f, 0f),
			new Vector3(0f, 0.1f, -0.1f),
			new Vector3(-0.1f, 0.1f, 0f),
			new Vector3(0f, 0.1f, 0.1f),
			new Vector3(0f, 0f, 0f)
		};

		public Vector3[] Vertices;

		public int[] Triangles;

		public Bone()
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
			float[] array = new float[18]
			{
				0f,
				1f,
				0f,
				0.1f,
				0.1f,
				0f,
				0f,
				0.1f,
				-0.1f,
				-0.1f,
				0.1f,
				0f,
				0f,
				0.1f,
				0.1f,
				0f,
				0f,
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
			return new int[24]
			{
				1,
				2,
				0,
				2,
				3,
				0,
				3,
				4,
				0,
				0,
				4,
				1,
				5,
				2,
				1,
				5,
				3,
				2,
				5,
				4,
				3,
				5,
				1,
				4
			};
		}
	}
}
