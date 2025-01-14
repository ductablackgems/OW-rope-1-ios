using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_MoveToTarget : MonoBehaviour
	{
		public Vector3 TargetPosition;

		public bool UseRayCase;

		public float DampingStart = 30f;

		public float DampingSpeed = 10f;

		public float Speed = 10f;

		public Vector3 SpreadMin;

		public Vector3 SpreadMax;

		private Rigidbody rigidBody;

		private void Start()
		{
			rigidBody = GetComponent<Rigidbody>();
			if (UseRayCase && Physics.Raycast(base.transform.position, base.transform.forward, out RaycastHit hitInfo))
			{
				TargetPosition = hitInfo.point;
			}
			Vector3 vector = new Vector3(UnityEngine.Random.Range(SpreadMin.x, SpreadMax.x), UnityEngine.Random.Range(SpreadMin.y, SpreadMax.y), UnityEngine.Random.Range(SpreadMin.z, SpreadMax.z)) * 0.01f;
			Vector3 forward = base.transform.forward * vector.z + base.transform.right * vector.x + base.transform.up * vector.y;
			base.transform.forward = forward;
		}

		private void Update()
		{
			rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
			DampingStart += DampingSpeed * Time.deltaTime;
			Quaternion b = Quaternion.LookRotation((TargetPosition - base.transform.position).normalized);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * DampingStart);
			base.transform.position += base.transform.forward * Speed * Time.deltaTime;
		}
	}
}
