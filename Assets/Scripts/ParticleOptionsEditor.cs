using UnityEngine;

[ExecuteInEditMode]
public class ParticleOptionsEditor : MonoBehaviour
{
	private ParticleSystem particleSystem;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		particleSystem.scalingMode = ParticleSystemScalingMode.Hierarchy;
		particleSystem.maxParticles = 150;
	}
}
