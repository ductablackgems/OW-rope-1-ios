using UnityEngine;

namespace ChobiAssets.KTP
{
	public class Create_Tracks_CS : MonoBehaviour
	{
		public bool isLeft = true;

		public float posX = 1.45f;

		public float width = 0.8f;

		public float height = 0.08f;

		public float extraRadius;

		public Material mat;

		public float scale = 0.1f;

		public float lineA = 0.5f;

		public float lineB = 0.49f;

		public float lineC = 0.49f;

		public float lineD = 0.38f;

		public float lineE = 0.38f;

		public float lineF = 0.37f;

		public float lineG = 0.62f;

		public float lineH = 0.5f;

		public int guideCount;

		public float guideHeight = 0.07f;

		public float[] guidePositions;

		public float guideLineTop;

		public float guideLineBottom;

		public Vector3[] positionArray;

		public float[] radiusArray;

		public float[] startAngleArray;

		public float[] endAngleArray;

		public bool showTexture;

		private void Awake()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if ((bool)component && (bool)component.sharedMesh)
			{
				if (component.sharedMesh.name == " Instance")
				{
					UnityEngine.Debug.LogWarning("The mesh of '" + base.name + "' is not saved yet.");
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("The mesh of '" + base.name + "' is not created yet.");
			}
			UnityEngine.Object.Destroy(this);
		}

		private void OnDrawGizmosSelected()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (!component || !component.sharedMesh)
			{
				return;
			}
			Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, base.transform.rotation, base.transform.localScale);
			Vector3[] vertices = component.sharedMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(vertices[i] + base.transform.position, 0.01f);
			}
			if (!showTexture)
			{
				return;
			}
			Texture mainTexture = GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
			float num = 2f;
			Gizmos.DrawGUITexture(new Rect(num / 2f, num / 2f, 0f - num, 0f - num), mainTexture);
			float[] array = new float[10]
			{
				lineA,
				lineB,
				lineC,
				lineD,
				lineE,
				lineF,
				lineG,
				lineH,
				guideLineTop,
				guideLineBottom
			};
			Gizmos.color = Color.yellow;
			for (int j = 0; j < array.Length; j++)
			{
				Vector3 vector = new Vector3(num / 2f, (0f - num) / 2f + array[j] * num, 0f);
				for (int k = 0; k < 6; k++)
				{
					Gizmos.DrawLine(vector, vector + Vector3.left * num);
				}
			}
		}
	}
}
