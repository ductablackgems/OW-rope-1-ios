using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class IcoSphere
	{
		public class Icosahedron
		{
			public Vector3[] Vertices;

			public int[] Triangles;

			public Icosahedron()
			{
				Vertices = CreateVertices();
				Triangles = CreateTriangles();
			}

			private Vector3[] CreateVertices()
			{
				Vector3[] array = new Vector3[12];
				float num = 0.5f;
				float num2 = (num + Mathf.Sqrt(5f)) / 2f;
				array[0] = new Vector3(num2, 0f, num);
				array[9] = new Vector3(0f - num2, 0f, num);
				array[11] = new Vector3(0f - num2, 0f, 0f - num);
				array[1] = new Vector3(num2, 0f, 0f - num);
				array[2] = new Vector3(num, num2, 0f);
				array[5] = new Vector3(num, 0f - num2, 0f);
				array[10] = new Vector3(0f - num, 0f - num2, 0f);
				array[8] = new Vector3(0f - num, num2, 0f);
				array[3] = new Vector3(0f, num, num2);
				array[7] = new Vector3(0f, num, 0f - num2);
				array[6] = new Vector3(0f, 0f - num, 0f - num2);
				array[4] = new Vector3(0f, 0f - num, num2);
				for (int i = 0; i < 12; i++)
				{
					array[i].Normalize();
				}
				return array;
			}

			private int[] CreateTriangles()
			{
				return new int[60]
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
					4,
					5,
					0,
					5,
					1,
					0,
					6,
					7,
					1,
					2,
					1,
					7,
					7,
					8,
					2,
					2,
					8,
					3,
					8,
					9,
					3,
					3,
					9,
					4,
					9,
					10,
					4,
					10,
					5,
					4,
					10,
					6,
					5,
					6,
					1,
					5,
					6,
					11,
					7,
					7,
					11,
					8,
					8,
					11,
					9,
					9,
					11,
					10,
					10,
					11,
					6
				};
			}
		}

		public static Vector3[] vertices;

		public static int[] triangleIndices;

		private static int[,] triangles;

		public static Mesh CreateSphere(int rSubdivisions)
		{
			Icosahedron ico = new Icosahedron();
			get_triangulation(rSubdivisions, ico);
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = vertices;
			mesh.triangles = triangleIndices;
			mesh.RecalculateNormals();
			return mesh;
		}

		private static void get_triangulation(int num, Icosahedron ico)
		{
			Dictionary<Vector3, int> dictionary = new Dictionary<Vector3, int>();
			float[,] subMatrix = getSubMatrix(num + 2);
			int num2 = 0;
			int length = subMatrix.GetLength(0);
			int num3 = (num + 1) * (num + 1) * 20;
			vertices = new Vector3[num3 / 2 + 2];
			triangleIndices = new int[num3 * 3];
			triangles = new int[num3, 3];
			Vector3[] array = new Vector3[length];
			int[] array2 = new int[length];
			int[,] array3 = triangulate(num);
			int length2 = array3.GetLength(0);
			for (int i = 0; i < 20; i++)
			{
				Vector3 vector = ico.Vertices[ico.Triangles[i * 3]];
				Vector3 vector2 = ico.Vertices[ico.Triangles[i * 3 + 1]];
				Vector3 vector3 = ico.Vertices[ico.Triangles[i * 3 + 2]];
				for (int j = 0; j < length; j++)
				{
					array[j].x = subMatrix[j, 0] * vector.x + subMatrix[j, 1] * vector2.x + subMatrix[j, 2] * vector3.x;
					array[j].y = subMatrix[j, 0] * vector.y + subMatrix[j, 1] * vector2.y + subMatrix[j, 2] * vector3.y;
					array[j].z = subMatrix[j, 0] * vector.z + subMatrix[j, 1] * vector2.z + subMatrix[j, 2] * vector3.z;
					array[j].Normalize();
					if (!dictionary.TryGetValue(array[j], out int value))
					{
						dictionary[array[j]] = num2;
						value = num2;
						vertices[num2] = array[j];
						num2++;
					}
					array2[j] = value;
				}
				for (int k = 0; k < length2; k++)
				{
					triangles[length2 * i + k, 0] = array2[array3[k, 0]];
					triangles[length2 * i + k, 1] = array2[array3[k, 1]];
					triangles[length2 * i + k, 2] = array2[array3[k, 2]];
					triangleIndices[3 * length2 * i + 3 * k] = array2[array3[k, 0]];
					triangleIndices[3 * length2 * i + 3 * k + 1] = array2[array3[k, 1]];
					triangleIndices[3 * length2 * i + 3 * k + 2] = array2[array3[k, 2]];
				}
			}
		}

		private static int[,] triangulate(int num)
		{
			int num2 = num + 2;
			int[,] array = new int[(num2 - 1) * (num2 - 1), 3];
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num2 - 1; i++)
			{
				array[num4, 0] = num3 + 1;
				array[num4, 1] = num3 + num2 - i;
				array[num4, 2] = num3;
				num4++;
				for (int j = 1; j < num2 - 1 - i; j++)
				{
					array[num4, 0] = num3 + j;
					array[num4, 1] = num3 + num2 - i + j;
					array[num4, 2] = num3 + num2 - i + j - 1;
					num4++;
					array[num4, 0] = num3 + j + 1;
					array[num4, 1] = num3 + num2 - i + j;
					array[num4, 2] = num3 + j;
					num4++;
				}
				num3 += num2 - i;
			}
			return array;
		}

		private static Vector2[] getUV(Vector3[] vertices)
		{
			int num = vertices.Length;
			float num2 = (float)Math.PI;
			Vector2[] array = new Vector2[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = cartToLL(vertices[i]);
				array[i].x = (array[i].x + num2) / (2f * num2);
				array[i].y = (array[i].y + num2 / 2f) / num2;
			}
			return array;
		}

		private static Vector2 cartToLL(Vector3 point)
		{
			Vector2 result = default(Vector2);
			float magnitude = point.magnitude;
			if (point.x != 0f || point.y != 0f)
			{
				result.x = 0f - (float)Math.Atan2(point.y, point.x);
			}
			else
			{
				result.x = 0f;
			}
			if (magnitude > 0f)
			{
				result.y = (float)Math.Asin(point.z / magnitude);
			}
			else
			{
				result.y = 0f;
			}
			return result;
		}

		private static float[,] getSubMatrix(int num)
		{
			float[,] array = new float[num * (num + 1) / 2, 3];
			float[] array2 = new float[num];
			int[] array3 = new int[num];
			int[] array4 = new int[num];
			int[] array5 = new int[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = (float)i / (float)(num - 1);
				array3[i] = num - i;
				if (i > 0)
				{
					array4[i] = array4[i - 1] + array3[i - 1];
				}
				else
				{
					array4[i] = 0;
				}
				array5[i] = array4[i] + array3[i];
			}
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < array3[j]; k++)
				{
					int num2 = array4[j] + k;
					array[num2, 0] = array2[array3[j] - 1 - k];
					array[num2, 1] = array2[k];
					array[num2, 2] = array2[j];
				}
			}
			return array;
		}
	}
}
