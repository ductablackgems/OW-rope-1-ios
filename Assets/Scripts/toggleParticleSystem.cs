using UnityEngine;
using UnityEngine.UI;

public class toggleParticleSystem : MonoBehaviour
{
	private ParticleSystem ps;

	public Toggle toggleButton;

	private void OnEnable()
	{
		ps = GetComponent<ParticleSystem>();
	}

	public void onOff()
	{
		if (toggleButton.isOn)
		{
			ps.Play(withChildren: true);
		}
		else
		{
			ps.Stop(withChildren: true);
		}
	}
}
