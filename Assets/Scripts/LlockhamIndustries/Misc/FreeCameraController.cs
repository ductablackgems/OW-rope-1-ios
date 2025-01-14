using UnityEngine;

namespace LlockhamIndustries.Misc
{
	public class FreeCameraController : GenericCameraController
	{
		[Header("Movement")]
		public float movementSpeed = 0.1f;

		public float movementThreshold = 0.1f;

		[Header("Limits")]
		public float minX = -10f;

		public float maxX = 10f;

		public float minZ = -10f;

		public float maxZ = 10f;

		private Vector2 mousePosition;

		private Vector3 cameraVelocity;

		private void Update()
		{
			EdgeScrollInput();
			RotationZoomInput();
		}

		private void LateUpdate()
		{
			ApplyEdgeScroll();
			ApplyRotationZoom();
		}

		private void EdgeScrollInput()
		{
			mousePosition = new Vector2(UnityEngine.Input.mousePosition.x / (float)Screen.width, UnityEngine.Input.mousePosition.y / (float)Screen.height);
		}

		private void ApplyEdgeScroll()
		{
			Vector3 a = Vector3.zero;
			if (mousePosition.x < movementThreshold)
			{
				a -= base.Right * (movementThreshold - mousePosition.x) / movementThreshold * movementSpeed;
			}
			if (1f - mousePosition.x < movementThreshold)
			{
				a += base.Right * (movementThreshold - (1f - mousePosition.x)) / movementThreshold * movementSpeed;
			}
			if (mousePosition.y < movementThreshold)
			{
				a -= base.Forward * (movementThreshold - mousePosition.y) / movementThreshold * movementSpeed;
			}
			if (1f - mousePosition.y < movementThreshold)
			{
				a += base.Forward * (movementThreshold - (1f - mousePosition.y)) / movementThreshold * movementSpeed;
			}
			a *= zoom / maxZoom;
			Vector3 vector = base.transform.position + a;
			vector.x = Mathf.Clamp(vector.x, minX, maxX);
			vector.z = Mathf.Clamp(vector.z, minZ, maxZ);
			base.transform.position = Vector3.SmoothDamp(base.transform.position, vector, ref cameraVelocity, 0.1f);
		}
	}
}
