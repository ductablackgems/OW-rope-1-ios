using UnityEngine;
using UnityEngine.UI;

public class sunSlider : MonoBehaviour
{
	public Slider mainSlider;

	public void sun()
	{
		float num = mainSlider.value * 2f;
		if ((double)mainSlider.value > 0.5)
		{
			num = (1f - num) * 2f + num;
		}
		GetComponent<Light>().intensity = Mathf.Clamp(num, 0.001f, 3f);
		base.transform.eulerAngles = new Vector3(mainSlider.value * 360f - 90f, -29f, 0f);
	}
}
