using UnityEngine;
using UnityEngine.UI;

public class HUDFPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private void Start()
	{
		if (!GetComponent<Text>())
		{
			UnityEngine.Debug.Log("UtilityFramesPerSecond needs a Text component!");
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string str = $"{num:F2}";
			GetComponent<Text>().text = str + " fps";
			if (num < 30f)
			{
				GetComponent<Text>().material.color = Color.yellow;
			}
			else if (num < 10f)
			{
				GetComponent<Text>().material.color = Color.red;
			}
			else
			{
				GetComponent<Text>().material.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
