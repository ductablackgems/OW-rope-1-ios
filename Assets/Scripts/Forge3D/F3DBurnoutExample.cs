using UnityEngine;

namespace Forge3D
{
	public class F3DBurnoutExample : MonoBehaviour
	{
		private MeshRenderer[] turretParts;

		private int BurnoutID;

		private void Start()
		{
			BurnoutID = Shader.PropertyToID("_BurnOut");
			turretParts = GetComponentsInChildren<MeshRenderer>();
		}

		private void Update()
		{
			for (int i = 0; i < turretParts.Length; i++)
			{
				turretParts[i].material.SetFloat(BurnoutID, Mathf.Lerp(0f, 2f, Mathf.Sin(Time.time) / 2f));
			}
		}
	}
}
