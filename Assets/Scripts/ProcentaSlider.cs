using UnityEngine;
using UnityEngine.UI;

public class ProcentaSlider : MonoBehaviour
{
	public Text textComponent;

	public int multipleCoof = 100;

	public void SetSliderValue(float sliderValue)
	{
		textComponent.text = Mathf.Round(sliderValue * (float)multipleCoof).ToString() + " %";
	}
}
