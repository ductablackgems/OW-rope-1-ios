using UnityEngine;

namespace LlockhamIndustries.Misc
{
	[RequireComponent(typeof(Locomotion))]
	public class LocomotionController : MonoBehaviour
	{
		public GenericCameraController cameraController;

		public float standardSpeed = 0.8f;

		public float balancedSpeed = 0.5f;

		public float sprintSpeed = 1.6f;

		private Locomotion locomotion;

		private Plane plane = new Plane(Vector3.up, 0f);

		private bool balanced;

		private float movementSpeed;

		private Vector3 movementVector;

		private float timeSinceDodge;

		private void Awake()
		{
			locomotion = GetComponent<Locomotion>();
		}

		private void Update()
		{
			MovementSpeedInput();
			MovementInput();
			BalanceInput();
		}

		private void MovementSpeedInput()
		{
			movementSpeed = standardSpeed;
			if (!balanced)
			{
				if (UnityEngine.Input.GetKey(KeyCode.LeftShift) && movementVector.magnitude > 0f)
				{
					movementSpeed = sprintSpeed;
				}
			}
			else
			{
				movementSpeed = balancedSpeed;
			}
		}

		private void MovementInput()
		{
			movementVector = Vector3.zero;
			if ((UnityEngine.Input.GetAxisRaw("Horizontal") != 0f || UnityEngine.Input.GetAxisRaw("Vertical") != 0f) && cameraController != null)
			{
				Vector3 zero = Vector3.zero;
				zero -= cameraController.Forward * UnityEngine.Input.GetAxisRaw("Vertical");
				zero -= cameraController.Right * UnityEngine.Input.GetAxisRaw("Horizontal");
				float d = Mathf.Max(Mathf.Abs(zero.x), Mathf.Abs(zero.z));
				movementVector = zero.normalized * d;
			}
			locomotion.Movement = movementVector * movementSpeed;
		}

		private void BalanceInput()
		{
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				balanced = true;
			}
			else
			{
				balanced = false;
			}
			if (balanced)
			{
				if (cameraController == null)
				{
					UnityEngine.Debug.Log("No Camera Controller Assigned! Please assign a valid camera controller.");
					return;
				}
				Ray ray = cameraController.GetComponentInChildren<Camera>().ScreenPointToRay(UnityEngine.Input.mousePosition);
				if (plane.Raycast(ray, out float enter))
				{
					locomotion.Direction = -(ray.GetPoint(enter) - base.transform.position).normalized;
				}
				else
				{
					UnityEngine.Debug.Log("Error Casting to Plane, Cannot Determine Cursor Location");
				}
			}
			else
			{
				locomotion.Direction = movementVector.normalized;
			}
		}
	}
}
