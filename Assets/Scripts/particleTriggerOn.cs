using UnityEngine;

public class particleTriggerOn : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") || other.name == "stolen")
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") || other.name == "stolen")
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
			}
		}
	}
}
