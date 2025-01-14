using System.Collections;
using UnityEngine;

public class AIControl : CarControl
{
	private void Start()
	{
		StartCoroutine(ChangeIdea());
	}

	private IEnumerator ChangeIdea()
	{
		while (base.enabled)
		{
			ControlCar(Random.Range(0, 3) - 1, Random.Range(0, 3) - 1);
			yield return new WaitForSeconds(Random.value * 3f);
		}
	}
}
