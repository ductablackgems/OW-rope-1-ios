using UnityEngine;

namespace ChobiAssets.KTP
{
	public class Track_Deform_CS : MonoBehaviour
	{
		public int anchorNum;

		public Transform[] anchorArray;

		public float[] widthArray;

		public float[] heightArray;

		public float[] offsetArray;

		private Mesh thisMesh;

		public float[] initialPosArray;

		public Vector3[] initialVertices;

		public IntArray[] movableVerticesList;

		private Vector3[] currentVertices;

		private void Awake()
		{
			thisMesh = GetComponent<MeshFilter>().mesh;
			thisMesh.MarkDynamic();
			for (int i = 0; i < anchorArray.Length; i++)
			{
				if (anchorArray[i] == null)
				{
					UnityEngine.Debug.LogWarning("Anchor Wheel is not assigned in " + base.name);
					UnityEngine.Object.Destroy(this);
				}
			}
			currentVertices = new Vector3[initialVertices.Length];
		}

		private void Update()
		{
			initialVertices.CopyTo(currentVertices, 0);
			for (int i = 0; i < anchorArray.Length; i++)
			{
				float num = anchorArray[i].localPosition.y - initialPosArray[i];
				for (int j = 0; j < movableVerticesList[i].intArray.Length; j++)
				{
					currentVertices[movableVerticesList[i].intArray[j]].y += num;
				}
			}
			thisMesh.vertices = currentVertices;
		}

		private void OnDrawGizmos()
		{
			if (anchorArray == null || anchorArray.Length == 0 || offsetArray == null || offsetArray.Length == 0)
			{
				return;
			}
			Gizmos.color = Color.green;
			for (int i = 0; i < anchorArray.Length; i++)
			{
				if (anchorArray[i] != null)
				{
					Vector3 size = new Vector3(0f, heightArray[i], widthArray[i]);
					Vector3 position = anchorArray[i].position;
					position.y += offsetArray[i];
					Gizmos.DrawWireCube(position, size);
				}
			}
		}

		private void Pause(bool isPaused)
		{
			base.enabled = !isPaused;
		}
	}
}
