using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_ScaleUp : MonoBehaviour
	{
		private Vector3 scaleTemp;

		public float ScaleSpeed = 30f;

		private bool Active = true;

		private void Start()
		{
			scaleTemp = base.transform.localScale;
			base.transform.localScale = Vector3.zero;
		}

		private void Update()
		{
			if (Active)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, scaleTemp, Time.deltaTime * ScaleSpeed);
			}
			if (Vector3.Magnitude(base.transform.localScale - scaleTemp) < 0.01f)
			{
				Active = false;
			}
		}
	}
}
