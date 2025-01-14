using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class DebugDraw
	{
		private static Material sMaterial;

		private static Material sOverlayMaterial;

		private static MaterialPropertyBlock sMaterialBlock;

		private static List<Vector3> sLines;

		private static Vector3[] sLineVertices;

		private static Mesh sLine;

		private static Mesh sDisk;

		private static Mesh sTetrahedron;

		private static Mesh sCube;

		private static Mesh sOctahedron;

		private static Mesh sDodecahedron;

		private static Mesh sIcosahedron;

		private static Mesh sSphere;

		private static Mesh sBone;

		static DebugDraw()
		{
			sLines = new List<Vector3>();
			sLineVertices = new Vector3[8];
		}

		public static void Initialize()
		{
			sDisk = CreateDisk();
			sTetrahedron = CreateTetrahedron();
			sCube = CreateCube();
			sOctahedron = CreateOctahedron();
			sDodecahedron = CreateDodecahedron();
			sIcosahedron = CreateIcosahedron();
			sSphere = CreateSphere();
			sBone = CreateBone();
			for (int i = 0; i < sLineVertices.Length; i++)
			{
				sLineVertices[i] = Vector3.zero;
			}
			int[] triangles = new int[36]
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
			sLine = new Mesh();
			sLine.vertices = sLineVertices;
			sLine.triangles = triangles;
			sOverlayMaterial = new Material(Shader.Find("Standard"));
			sMaterial = new Material(Shader.Find("Standard"));
			sMaterialBlock = new MaterialPropertyBlock();
		}

		public static void Invalidate()
		{
			sMaterial = null;
			sOverlayMaterial = null;
		}

		public static void DrawCube(Vector3 rCenter, Vector3 rSize, Color rColor, bool rWireframe)
		{
			Vector3 vector = rSize * 0.5f;
			DrawLine(rCenter + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z), rCenter + new Vector3(vector.x, 0f - vector.y, 0f - vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z), rCenter + new Vector3(0f - vector.x, 0f - vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(vector.x, 0f - vector.y, 0f - vector.z), rCenter + new Vector3(vector.x, 0f - vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, 0f - vector.y, vector.z), rCenter + new Vector3(vector.x, 0f - vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, vector.y, 0f - vector.z), rCenter + new Vector3(vector.x, vector.y, 0f - vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, vector.y, 0f - vector.z), rCenter + new Vector3(0f - vector.x, vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(vector.x, vector.y, 0f - vector.z), rCenter + new Vector3(vector.x, vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, vector.y, vector.z), rCenter + new Vector3(vector.x, vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z), rCenter + new Vector3(0f - vector.x, vector.y, 0f - vector.z), rColor);
			DrawLine(rCenter + new Vector3(0f - vector.x, 0f - vector.y, vector.z), rCenter + new Vector3(0f - vector.x, vector.y, vector.z), rColor);
			DrawLine(rCenter + new Vector3(vector.x, 0f - vector.y, 0f - vector.z), rCenter + new Vector3(vector.x, vector.y, 0f - vector.z), rColor);
			DrawLine(rCenter + new Vector3(vector.x, 0f - vector.y, vector.z), rCenter + new Vector3(vector.x, vector.y, vector.z), rColor);
		}

		public static void DrawCircle(Vector3 rCenter, float rRadius, Color rColor)
		{
			DrawArc(rCenter, Quaternion.identity, 0f, 360f, rRadius, rColor);
		}

		public static void DrawWireSphere(Vector3 rCenter, float rRadius, Color rColor)
		{
			DrawArc(rCenter, Quaternion.identity, 0f, 360f, rRadius, rColor);
			DrawArc(rCenter, Quaternion.AngleAxis(90f, Vector3.right), 0f, 360f, rRadius, rColor);
			DrawArc(rCenter, Quaternion.AngleAxis(90f, Vector3.forward), 0f, 360f, rRadius, rColor);
		}

		public static void DrawArc(Vector3 rCenter, Quaternion rRotation, float rMinAngle, float rMaxAngle, float rRadius, Color rColor)
		{
			sLines.Clear();
			float num = 10f;
			Vector3 zero = Vector3.zero;
			for (float num2 = rMinAngle; num2 <= rMaxAngle; num2 += num)
			{
				float f = 0f - num2 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = rRadius * Mathf.Cos(f);
				zero.y = 0f;
				zero.z = rRadius * Mathf.Sin(f);
				sLines.Add(zero);
			}
			Matrix4x4 matrix4x = Matrix4x4.TRS(rCenter, rRotation, Vector3.one);
			for (int i = 0; i < sLines.Count; i++)
			{
				sLines[i] = matrix4x.MultiplyPoint3x4(sLines[i]);
			}
			DrawLines(sLines, rColor);
		}

		public static void DrawFrustumArc(Vector3 rPosition, Quaternion rRotation, float rHAngle, float rVAngle, float rDistance, Color rColor)
		{
			float num = 10f;
			Vector3 zero = Vector3.zero;
			float num2 = rHAngle * 0.5f;
			float num3 = rVAngle * 0.5f;
			List<Vector3> list = new List<Vector3>(2)
			{
				Vector3.zero,
				Vector3.zero
			};
			List<Vector3> list2 = new List<Vector3>(5)
			{
				Vector3.zero,
				Vector3.zero,
				Vector3.zero,
				Vector3.zero,
				Vector3.zero
			};
			List<Vector3> list3 = new List<Vector3>(5)
			{
				Vector3.zero,
				Vector3.zero,
				Vector3.zero,
				Vector3.zero,
				Vector3.zero
			};
			sLines.Clear();
			for (float num4 = 0f - num2; num4 <= num2; num4 += num)
			{
				float f = 0f - num4 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = 1f * Mathf.Cos(f);
				zero.y = 0f;
				zero.z = 1f * Mathf.Sin(f);
				sLines.Add(zero);
			}
			Matrix4x4 matrix4x = Matrix4x4.TRS(rPosition, rRotation * Quaternion.AngleAxis(num3, Vector3.right), Vector3.one);
			for (int i = 0; i < sLines.Count; i++)
			{
				sLines[i] = matrix4x.MultiplyPoint3x4(sLines[i]);
			}
			list2[1] = sLines[0];
			list3[1] = sLines[sLines.Count - 1];
			DrawLines(sLines, rColor);
			sLines.Clear();
			for (float num5 = 0f - num2; num5 <= num2; num5 += num)
			{
				float f2 = 0f - num5 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = rDistance * Mathf.Cos(f2);
				zero.y = 0f;
				zero.z = rDistance * Mathf.Sin(f2);
				sLines.Add(zero);
			}
			matrix4x = Matrix4x4.TRS(rPosition, rRotation * Quaternion.AngleAxis(num3, Vector3.right), Vector3.one);
			for (int j = 0; j < sLines.Count; j++)
			{
				sLines[j] = matrix4x.MultiplyPoint3x4(sLines[j]);
			}
			list2[0] = sLines[0];
			list2[4] = sLines[0];
			list3[0] = sLines[sLines.Count - 1];
			list3[4] = sLines[sLines.Count - 1];
			list[0] = sLines[sLines.Count / 2];
			DrawLines(sLines, rColor);
			sLines.Clear();
			for (float num6 = 0f - num2; num6 <= num2; num6 += num)
			{
				float f3 = 0f - num6 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = 1f * Mathf.Cos(f3);
				zero.y = 0f;
				zero.z = 1f * Mathf.Sin(f3);
				sLines.Add(zero);
			}
			matrix4x = Matrix4x4.TRS(rPosition, rRotation * Quaternion.AngleAxis(0f - num3, Vector3.right), Vector3.one);
			for (int k = 0; k < sLines.Count; k++)
			{
				sLines[k] = matrix4x.MultiplyPoint3x4(sLines[k]);
			}
			list2[2] = sLines[0];
			list3[2] = sLines[sLines.Count - 1];
			DrawLines(sLines, rColor);
			sLines.Clear();
			for (float num7 = 0f - num2; num7 <= num2; num7 += num)
			{
				float f4 = 0f - num7 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = rDistance * Mathf.Cos(f4);
				zero.y = 0f;
				zero.z = rDistance * Mathf.Sin(f4);
				sLines.Add(zero);
			}
			matrix4x = Matrix4x4.TRS(rPosition, rRotation * Quaternion.AngleAxis(0f - num3, Vector3.right), Vector3.one);
			for (int l = 0; l < sLines.Count; l++)
			{
				sLines[l] = matrix4x.MultiplyPoint3x4(sLines[l]);
			}
			list2[3] = sLines[0];
			list3[3] = sLines[sLines.Count - 1];
			list[1] = sLines[sLines.Count / 2];
			DrawLines(sLines, rColor);
			sLines.Clear();
			for (float num8 = 0f - num3; num8 <= num3; num8 += num)
			{
				float f5 = 0f - num8 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = 0f;
				zero.y = 1f * Mathf.Cos(f5);
				zero.z = 1f * Mathf.Sin(f5);
				sLines.Add(zero);
			}
			matrix4x = Matrix4x4.TRS(rPosition, rRotation, Vector3.one);
			for (int m = 0; m < sLines.Count; m++)
			{
				sLines[m] = matrix4x.MultiplyPoint3x4(sLines[m]);
			}
			DrawLines(sLines, rColor);
			sLines.Clear();
			for (float num9 = 0f - num3; num9 <= num3; num9 += num)
			{
				float f6 = 0f - num9 * ((float)Math.PI / 180f) + 1.57079f;
				zero.x = 0f;
				zero.y = rDistance * Mathf.Cos(f6);
				zero.z = rDistance * Mathf.Sin(f6);
				sLines.Add(zero);
			}
			matrix4x = Matrix4x4.TRS(rPosition, rRotation, Vector3.one);
			for (int n = 0; n < sLines.Count; n++)
			{
				sLines[n] = matrix4x.MultiplyPoint3x4(sLines[n]);
			}
			DrawLines(sLines, rColor);
			DrawLines(list2, rColor);
			DrawLines(list3, rColor);
		}

		public static void DrawLines(List<Vector3> rLines, Color rColor)
		{
			for (int i = 1; i < rLines.Count; i++)
			{
				UnityEngine.Debug.DrawLine(rLines[i - 1], rLines[i], rColor);
			}
		}

		public static void DrawLine(Vector3 rFrom, Vector3 rTo, Color rColor)
		{
			UnityEngine.Debug.DrawLine(rFrom, rTo, rColor);
		}

		public static void DrawLine(Vector3 rFrom, Vector3 rTo, float rThickness, Color rColor, float rAlpha)
		{
			if (sCube == null || sMaterial == null)
			{
				Initialize();
			}
			Vector3 pos = new Vector3(rFrom.x + (rTo.x - rFrom.x) / 2f, rFrom.y + (rTo.y - rFrom.y) / 2f, rFrom.z + (rTo.z - rFrom.z) / 2f);
			Quaternion q = Quaternion.FromToRotation(Vector3.right, (rTo - rFrom).normalized);
			Vector3 s = new Vector3(Vector3.Distance(rFrom, rTo), rThickness, rThickness);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sCube, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawLineOverlay(Vector3 rFrom, Vector3 rTo, float rThickness, Color rColor, float rAlpha)
		{
			if (sCube == null || sOverlayMaterial == null)
			{
				Initialize();
			}
			Vector3 pos = new Vector3(rFrom.x + (rTo.x - rFrom.x) / 2f, rFrom.y + (rTo.y - rFrom.y) / 2f, rFrom.z + (rTo.z - rFrom.z) / 2f);
			Quaternion q = Quaternion.FromToRotation(Vector3.right, (rTo - rFrom).normalized);
			Vector3 s = new Vector3(Vector3.Distance(rFrom, rTo), rThickness, rThickness);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sCube, matrix, sOverlayMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawTetrahedronMesh(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sTetrahedron == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sTetrahedron, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawCubeMesh(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sCube == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sCube, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawOctahedronMesh(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sOctahedron == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sOctahedron, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawOctahedronOverlay(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sOctahedron == null || sOverlayMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sOctahedron, matrix, sOverlayMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawDodecahedronMesh(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sDodecahedron == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sDodecahedron, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawIcosahedronMesh(Vector3 rPosition, Quaternion rRotation, float rSize, Color rColor, float rAlpha)
		{
			if (sIcosahedron == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rSize * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sIcosahedron, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawSphereMesh(Vector3 rPosition, float rRadius, Color rColor, float rAlpha)
		{
			if (sSphere == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, Quaternion.identity, rRadius * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sSphere, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawSphereOverlay(Vector3 rPosition, float rRadius, Color rColor, float rAlpha)
		{
			if (sSphere == null || sOverlayMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, Quaternion.identity, rRadius * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sSphere, matrix, sOverlayMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawDiskMesh(Vector3 rPosition, Quaternion rRotation, float rRadius, Color rColor, float rAlpha)
		{
			if (sDisk == null || sMaterial == null)
			{
				Initialize();
			}
			Matrix4x4 matrix = Matrix4x4.TRS(rPosition, rRotation, rRadius * 2f * Vector3.one);
			Color value = rColor;
			value.a = rAlpha;
			Color value2 = rColor * 0.5f;
			value2.a = rAlpha;
			sMaterialBlock.Clear();
			sMaterialBlock.SetColor("_Color", value);
			sMaterialBlock.SetColor("_Emission", value2);
			UnityEngine.Graphics.DrawMesh(sDisk, matrix, sMaterial, 0, null, 0, sMaterialBlock);
		}

		public static void DrawBoneMesh(Transform rBoneTransform, Color rColor, float rAlpha)
		{
			if (rBoneTransform == null)
			{
				return;
			}
			if (sBone == null || sOverlayMaterial == null)
			{
				Initialize();
			}
			float d = 0.02f;
			int num = Mathf.Max(rBoneTransform.childCount, 1);
			for (int i = 0; i < num; i++)
			{
				Quaternion q = rBoneTransform.rotation;
				if (rBoneTransform.childCount > i)
				{
					Transform child = rBoneTransform.GetChild(i);
					d = Vector3.Distance(rBoneTransform.position, child.position);
					q = Quaternion.FromToRotation(Vector3.up, child.position - rBoneTransform.position);
				}
				Matrix4x4 matrix = Matrix4x4.TRS(rBoneTransform.position, q, d * Vector3.one);
				Color value = rColor;
				value.a = rAlpha;
				Color value2 = rColor * 0.5f;
				value2.a = rAlpha;
				sMaterialBlock.Clear();
				sMaterialBlock.SetColor("_Color", value);
				sMaterialBlock.SetColor("_Emission", value2);
				UnityEngine.Graphics.DrawMesh(sBone, matrix, sOverlayMaterial, 0, null, 0, sMaterialBlock);
			}
		}

		public static void DrawSkeleton(Transform rRootTransform, Color rColor, float rAlpha)
		{
			if (!(rRootTransform == null))
			{
				DrawBoneMesh(rRootTransform, rColor, rAlpha);
				for (int i = 0; i < rRootTransform.childCount; i++)
				{
					DrawSkeleton(rRootTransform.GetChild(i), rColor, rAlpha);
				}
			}
		}

		public static void DrawSkeleton(Transform rRootTransform, Color rColor, float rAlpha, bool rDrawAxis, List<Transform> rSelectedBones, Color rSelectedColor)
		{
			if (!(rRootTransform == null))
			{
				if (rSelectedBones != null && rSelectedBones.IndexOf(rRootTransform) >= 0)
				{
					DrawBoneMesh(rRootTransform, rSelectedColor, 1f);
				}
				else
				{
					DrawBoneMesh(rRootTransform, rColor, rAlpha);
				}
				for (int i = 0; i < rRootTransform.childCount; i++)
				{
					DrawSkeleton(rRootTransform.GetChild(i), rColor, rAlpha, rDrawAxis, rSelectedBones, rSelectedColor);
				}
			}
		}

		public static void DrawHumanoidSkeleton(GameObject rObject, Color rColor, float rAlpha)
		{
			Animator component = rObject.GetComponent<Animator>();
			if (component == null)
			{
				return;
			}
			string[] names = Enum.GetNames(typeof(HumanBodyBones));
			for (int i = 0; i < names.Length; i++)
			{
				Transform boneTransform = component.GetBoneTransform((HumanBodyBones)i);
				if (boneTransform != null)
				{
					DrawBoneMesh(boneTransform, rColor, rAlpha);
				}
			}
		}

		public static void DrawTransform(Transform rTransform, float rSize)
		{
			Vector3 position = rTransform.position;
			Quaternion rotation = rTransform.rotation;
			DrawLineOverlay(position, position + rotation * Vector3.right * rSize, 0.002f, Color.red, 1f);
			DrawLineOverlay(position, position + rotation * Vector3.up * rSize, 0.002f, Color.green, 1f);
			DrawLineOverlay(position, position + rotation * Vector3.forward * rSize, 0.002f, Color.blue, 1f);
		}

		public static void DrawTransform(Vector3 rPosition, Quaternion rRotation, float rSize)
		{
			DrawLineOverlay(rPosition, rPosition + rRotation * Vector3.right * rSize, 0.002f, Color.red, 1f);
			DrawLineOverlay(rPosition, rPosition + rRotation * Vector3.up * rSize, 0.002f, Color.green, 1f);
			DrawLineOverlay(rPosition, rPosition + rRotation * Vector3.forward * rSize, 0.002f, Color.blue, 1f);
		}

		public static Mesh CreateTetrahedron()
		{
			Tetrahedron tetrahedron = new Tetrahedron();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = tetrahedron.Vertices;
			mesh.triangles = tetrahedron.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateCube()
		{
			Cube cube = new Cube();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = cube.Vertices;
			mesh.triangles = cube.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateOctahedron()
		{
			Octahedron octahedron = new Octahedron();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = octahedron.Vertices;
			mesh.triangles = octahedron.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateDodecahedron()
		{
			Dodecahedron dodecahedron = new Dodecahedron();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = dodecahedron.Vertices;
			mesh.triangles = dodecahedron.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateIcosahedron()
		{
			Icosahedron icosahedron = new Icosahedron();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = icosahedron.Vertices;
			mesh.triangles = icosahedron.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateSphere()
		{
			return IcoSphere.CreateSphere(4);
		}

		public static Mesh CreateDisk()
		{
			Disk disk = new Disk();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = disk.Vertices;
			mesh.triangles = disk.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}

		public static Mesh CreateBone()
		{
			Bone bone = new Bone();
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.vertices = bone.Vertices;
			mesh.triangles = bone.Triangles;
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}
