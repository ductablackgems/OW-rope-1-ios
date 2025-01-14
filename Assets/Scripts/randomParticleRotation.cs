using UnityEngine;

public class randomParticleRotation : MonoBehaviour
{
	public bool x;

	public bool y;

	public bool z;

	private void OnEnable()
	{
		if (x)
		{
			base.transform.localEulerAngles += new Vector3(UnityEngine.Random.value * 360f, 0f, 0f);
		}
		if (y)
		{
			base.transform.localEulerAngles += new Vector3(0f, UnityEngine.Random.value * 360f, 0f);
		}
		if (z)
		{
			base.transform.localEulerAngles += new Vector3(0f, 0f, UnityEngine.Random.value * 360f);
		}
	}
}
