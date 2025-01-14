using UnityEngine;

public class magicEffectButton : MonoBehaviour
{
	public string button;

	public ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
		ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void offOnButton()
	{
		if (!ps.IsAlive(withChildren: true))
		{
			base.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(button))
		{
			offOnButton();
		}
	}
}
