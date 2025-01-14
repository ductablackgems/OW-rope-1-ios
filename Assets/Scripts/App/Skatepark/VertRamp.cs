using UnityEngine;

namespace App.Skatepark
{
	public class VertRamp : MonoBehaviour
	{
		public RampData rampData;

		public Vector3 TranslateCopingPosition()
		{
			return base.transform.TransformPoint(rampData.copingPosition);
		}

		public Vector3 TranslateVertNormal()
		{
			return base.transform.TransformDirection(rampData.vertNormal).normalized;
		}

		public float TranslateMinVertY()
		{
			return TranslateMinVertPoint().y;
		}

		public float TranslateVertY(float y)
		{
			return base.transform.TransformPoint(Vector3.up * y).y;
		}

		private void OnDrawGizmos()
		{
			if (!(rampData == null) && rampData.drawGizmo)
			{
				DrawArrow.ForGizmo(TranslateMinVertPoint(), Vector3.up * 4f, Color.blue, 1f);
				if (!(rampData.vertNormal == Vector3.zero))
				{
					DrawArrow.ForGizmo(TranslateCopingPosition(), TranslateVertNormal() * 4f, Color.red, 1f);
				}
			}
		}

		private Vector3 TranslateMinVertPoint()
		{
			return base.transform.TransformPoint(Vector3.up * rampData.minVertY);
		}
	}
}
