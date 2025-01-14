using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_RandomScale : MonoBehaviour
	{
		public bool Blend;

		public float BlendSpeed = 0.5f;

		public float ScaleMin;

		public float ScaleMax = 1f;

		private Vector3 scaleTarget;

		private void Start()
		{
			scaleTarget = base.transform.localScale * UnityEngine.Random.Range(ScaleMin, ScaleMax);
			if (!Blend)
			{
				base.transform.localScale = scaleTarget;
			}
			else
			{
				base.transform.localScale = scaleTarget * 0.2f;
			}
		}

		private void Update()
		{
			if (Blend)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, scaleTarget, 0.5f);
			}
		}
	}
}
