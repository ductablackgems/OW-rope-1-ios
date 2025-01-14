using UnityEngine;
using UnityEngine.UI;

public class EffectButtonLoop : MonoBehaviour
{
	public string button;

	public Toggle myToggle;

	public ParticleSystem ps;

	private void Start()
	{
		ps = ps.GetComponent<ParticleSystem>();
		var em = ps.emission;
		em.enabled = false;
	}

	public void offOnButton()
	{
		ParticleSystem.EmissionModule emission = ps.emission;
		if (myToggle.isOn)
		{
			emission.enabled = true;
		}
		else
		{
			emission.enabled = false;
		}
	}

	private void Update()
	{
		if (!(button == "") && UnityEngine.Input.GetKeyDown(button))
		{
			myToggle.isOn = !myToggle.isOn;
		}
	}
}
