using UnityEngine;
using UnityEngine.UI;

public class toggleControl : MonoBehaviour
{
	public GameObject[] effects;

	private ParticleSystem[] fx;

	public Toggle toggleButton;

	public Text toggleText;

	public Button prevButton;

	public Button nextButton;

	public int nonLoop = 19;

	private int count;

	private void Start()
	{
		fx = new ParticleSystem[effects.Length];
		for (int i = 0; i < effects.Length; i++)
		{
			fx[i] = effects[i].GetComponent<ParticleSystem>();
		}
		toggleText.text = fx[count].ToString();
		for (count = 1; count < fx.Length; count++)
		{
			fx[count].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		count = 0;
		fx[0].Play(withChildren: true);
		if (nonLoop < 0)
		{
			nonLoop = 1000000;
		}
	}

	public void onOff()
	{
		if (toggleButton.isOn)
		{
			fx[count].Play(withChildren: true);
		}
		else if (count > nonLoop)
		{
			fx[count].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			fx[count].Play(withChildren: true);
			toggleButton.isOn = true;
		}
		else
		{
			fx[count].Stop(withChildren: true);
		}
	}

	public void next()
	{
		fx[count].Stop(withChildren: true);
		if (count == fx.Length - 1)
		{
			count = 0;
		}
		else
		{
			count++;
		}
		toggleText.text = fx[count].ToString();
		fx[count].Play(withChildren: true);
		toggleButton.isOn = true;
	}

	public void prev()
	{
		fx[count].Stop(withChildren: true);
		if (count == 0)
		{
			count = fx.Length - 1;
		}
		else
		{
			count--;
		}
		toggleText.text = fx[count].ToString();
		fx[count].Play(withChildren: true);
		toggleButton.isOn = true;
	}

	public void toggleTargetModeRandom()
	{
		GameObject[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].transform.GetChild(0).GetChild(0).GetComponent<particleHomingMultiTarget>()
				.targetSelection = particleHomingMultiTarget.TSOP.random;
			}
		}

		public void toggleTargetModeClosest()
		{
			GameObject[] array = effects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].transform.GetChild(0).GetChild(0).GetComponent<particleHomingMultiTarget>()
					.targetSelection = particleHomingMultiTarget.TSOP.closest;
				}
			}
		}
