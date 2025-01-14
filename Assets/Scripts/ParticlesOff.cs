using System.Collections;
using UnityEngine;

public class ParticlesOff : MonoBehaviour
{
	public float time = 10f;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(time);
		GetComponent<ParticleSystem>().Stop();
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Stop();
		}
	}
}
