using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_FadeToGround : MonoBehaviour
	{
		public Vector3 Speed = -Vector3.up;

		public bool Down;

		public float ScaleSpeed;

		public Vector3 Scale = Vector3.zero;

		private void Start()
		{
		}

		public void OnDead()
		{
			Down = true;
		}

		private void Update()
		{
			if (Down)
			{
				base.transform.position += Speed * Time.deltaTime;
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Scale, ScaleSpeed * Time.deltaTime);
			}
		}
	}
}
