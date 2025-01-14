using App.Vehicles.Tank;
using UnityEngine;

namespace ChobiAssets.KTP
{
	public class Track_Scroll_CS : MonoBehaviour
	{
		[Header("Scroll Animation settings")]
		[Tooltip("Reference wheel.")]
		public Transform referenceWheel;

		[Tooltip("Scroll Rate for X axis.")]
		public float scrollRate = 0.0005f;

		[Tooltip("Texture Name in the shader.")]
		public string textureName = "_MainTex";

		public bool isLeft;

		public TankController tankController;

		private Material thisMaterial;

		private float previousAng;

		private Vector2 offset;

		[HideInInspector]
		public float deltaAng;

		private void Awake()
		{
			thisMaterial = GetComponent<Renderer>().material;
			if (referenceWheel == null)
			{
				UnityEngine.Debug.LogWarning("Reference Wheel is not assigned in " + base.name);
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				Send_References();
			}
		}

		private void Send_References()
		{
			Vector3 localPosition = referenceWheel.localPosition;
			Vector3 extent = referenceWheel.GetComponent<MeshFilter>().mesh.bounds.extents;
		}

		private void Update()
		{
			if (tankController.enabled)
			{
				Calculate_Delta_Angle();
			}
		}

		private void Calculate_Delta_Angle()
		{
			float y = referenceWheel.localEulerAngles.y;
			deltaAng = Mathf.DeltaAngle(y, previousAng);
			if (isLeft)
			{
				offset.x += scrollRate * Time.deltaTime * tankController.LeftWheelRotationSpeed;
			}
			else
			{
				offset.x += scrollRate * Time.deltaTime * tankController.RightWheelRotationSpeed;
			}
			thisMaterial.SetTextureOffset(textureName, offset);
			previousAng = y;
		}

		private void Pause(bool isPaused)
		{
			base.enabled = !isPaused;
		}
	}
}
