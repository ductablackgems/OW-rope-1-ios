using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_Position : MonoBehaviour
	{
		public Vector3 Offset = new Vector3(0f, 0.001f, 0f);

		public bool Normal;

		public SpawnMode Mode;

		private void Start()
		{
		}

		private void Awake()
		{
			if (Normal)
			{
				PlaceNormal(base.transform.position);
			}
		}

		public void PlaceNormal(Vector3 position)
		{
			if (Physics.Raycast(position, -Vector3.up * 100f, out RaycastHit hitInfo))
			{
				base.transform.position = hitInfo.point + Offset;
				base.transform.forward = hitInfo.normal;
			}
			else
			{
				base.transform.position = position + Offset;
			}
		}
	}
}
