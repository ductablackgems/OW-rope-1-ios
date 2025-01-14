using System;
using UnityEngine;

namespace Forge3D
{
	[RequireComponent(typeof(LineRenderer))]
	public class F3DCurvedBeam : MonoBehaviour
	{
		public Transform dest;

		public float beamScale;

		public float UVTime;

		private LineRenderer lineRenderer;

		public int curvePoints;

		public float curveHeight;

		private float initialBeamOffset;

		private void Start()
		{
			lineRenderer = GetComponent<LineRenderer>();
			initialBeamOffset = UnityEngine.Random.Range(0f, 5f);
			lineRenderer.SetVertexCount(curvePoints);
		}

		private void Update()
		{
			lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time * UVTime + initialBeamOffset, 0f));
			float num = Vector3.Distance(base.transform.position, dest.position);
			lineRenderer.SetPosition(0, base.transform.position);
			float num2 = (float)Math.PI / (float)(curvePoints - 1);
			for (int i = 1; i < curvePoints - 1; i++)
			{
				float d = num / (float)(curvePoints - 1) * (float)i;
				Vector3 a = Vector3.Normalize(dest.position - base.transform.position) * d;
				float d2 = Mathf.Sin(num2 * (float)i) * curveHeight;
				a += base.transform.up * d2;
				lineRenderer.SetPosition(i, base.transform.position + a);
			}
			lineRenderer.SetPosition(curvePoints - 1, dest.position);
			float x = num * (beamScale / 10f);
			lineRenderer.material.SetTextureScale("_MainTex", new Vector2(x, 1f));
		}
	}
}
