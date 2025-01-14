using UnityEngine;

namespace App.Player
{
	public class HelicopterControls : MonoBehaviour
	{
		public float maxForce = 10f;

		public float maxTorque = 10f;

		private Rigidbody _rigidbody;

		public void Move(Vector3 direction, float turn)
		{
			direction = Vector3.ClampMagnitude(direction, 1f);
			_rigidbody.AddRelativeForce(direction * maxForce, ForceMode.Acceleration);
			if (Mathf.Abs(turn) > 0f)
			{
				base.transform.Rotate(new Vector3(0f, maxTorque * Time.fixedDeltaTime * turn, 0f));
			}
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
		}

		protected void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
		}
	}
}
