using UnityEngine;

public class FlashLight : MonoBehaviour
{
	public float LightMult = 2f;

	private void Update()
	{
		if ((bool)GetComponent<Light>())
		{
			GetComponent<Light>().intensity -= LightMult * Time.deltaTime;
		}
	}
}
