using UnityEngine;
using UnityEngine.UI;

public class toggleControlSingle : MonoBehaviour
{
	public GameObject[] effects;

	public ParticleSystem[] ps;

	public Toggle toggleButton;

	public Text toggleText;

	public Button prevButton;

	public Button nextButton;

	private int count;

	private void Start()
	{
		toggleText.text = effects[count].ToString();
		for (count = 0; count < effects.Length; count++)
		{
			ps[count] = effects[count].GetComponent<ParticleSystem>();
		}
		count = 0;
		effects[0].SetActive(value: true);
		ps[0].Play(withChildren: true);
	}

	public void onOff()
	{
		if (toggleButton.isOn)
		{
			effects[count].SetActive(value: true);
			ps[count].Play(withChildren: true);
		}
		else
		{
			effects[count].SetActive(value: false);
			ps[count].Stop(withChildren: true);
		}
	}

	public void next()
	{
		effects[count].SetActive(value: false);
		ps[count].Stop(withChildren: true);
		if (count == effects.Length - 1)
		{
			count = 0;
		}
		else
		{
			count++;
		}
		toggleText.text = effects[count].ToString();
		effects[count].SetActive(value: true);
		ps[count].Play(withChildren: true);
		toggleButton.isOn = true;
	}

	public void prev()
	{
		effects[count].SetActive(value: false);
		ps[count].Stop(withChildren: true);
		if (count == 0)
		{
			count = effects.Length - 1;
		}
		else
		{
			count--;
		}
		toggleText.text = effects[count].ToString();
		effects[count].SetActive(value: true);
		ps[count].Play(withChildren: true);
		toggleButton.isOn = true;
	}
}
