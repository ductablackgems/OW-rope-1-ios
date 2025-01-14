using UnityEngine;

namespace MagicalFX
{
	public class FX_Light : MonoBehaviour
	{
		private Light lighter;

		public float Delay = 0.5f;

		private void Start()
		{
			lighter = GetComponent<Light>();
		}

		private void Update()
		{
			if ((bool)lighter)
			{
				lighter.intensity = Mathf.Lerp(lighter.intensity, 0f, Delay);
			}
		}
	}
}
