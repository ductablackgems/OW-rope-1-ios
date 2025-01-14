using UnityEngine;

namespace Forge3D
{
	public class F3DNebula : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
			base.transform.position -= Vector3.forward * Time.deltaTime * 4000f;
			if (base.transform.position.z < -9150f)
			{
				Vector3 position = base.transform.position;
				position.z = 9150f;
				base.transform.position = position;
				base.transform.rotation = UnityEngine.Random.rotation;
				base.transform.localScale = new Vector3(1f, 1f, 1f) * UnityEngine.Random.Range(200, 800);
			}
		}
	}
}
