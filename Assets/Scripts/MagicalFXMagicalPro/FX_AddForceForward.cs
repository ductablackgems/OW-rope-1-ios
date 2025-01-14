using UnityEngine;

namespace MagicalFXMagicalPro
{
	[RequireComponent(typeof(Rigidbody))]
	public class FX_AddForceForward : MonoBehaviour
	{
		public float Force = 300f;

		private void Start()
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.AddForce(base.transform.forward * Force);
			}
		}

		private void Update()
		{
		}
	}
}
