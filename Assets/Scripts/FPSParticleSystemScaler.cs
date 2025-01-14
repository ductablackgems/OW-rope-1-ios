using UnityEngine;

[ExecuteInEditMode]
public class FPSParticleSystemScaler : MonoBehaviour
{
	public float particlesScale = 1f;

	private float oldScale;

	private void Start()
	{
		oldScale = particlesScale;
	}

	private void Update()
	{
	}
}
