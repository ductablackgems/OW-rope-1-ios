using App;
using UnityEngine;
using UnityEngine.UI;

public class DebugSensor : MonoBehaviour
{
	public Text text;

	private DurationTimer markTimer = new DurationTimer();

	private int count;

	private static DebugSensor instance;

	public static void Show(float duration)
	{
		instance.ShowInternal(duration);
	}

	private void ShowInternal(float duration)
	{
		markTimer.Run(duration);
		count++;
		text.text = count.ToString();
		text.gameObject.SetActive(value: true);
	}

	private void Awake()
	{
		instance = this;
		text.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (markTimer.Done())
		{
			markTimer.Stop();
			count = 0;
			text.gameObject.SetActive(value: false);
		}
	}
}
