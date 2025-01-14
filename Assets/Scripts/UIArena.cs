using UnityEngine;
using UnityEngine.UI;

public class UIArena : MonoBehaviour
{
	public Toggle ToogleColorDeformation;

	public Slider SliderColorDeformationR;

	public Slider SliderColorDeformationG;

	public Slider SliderColorDeformationB;

	public Slider SliderColorDeformationA;

	public Slider SliderColorDeformationIntesity;

	public Image ImageDeformationColor;

	public Toggle ToggleUseCarColor;

	public GameObject ColorDeformationControls;

	public Arena Arena;

	public void UpdateColorDeformationParameters()
	{
		ColorDeformationControls.SetActive(ToogleColorDeformation.isOn);
		ImageDeformationColor.color = new Color(SliderColorDeformationR.value, SliderColorDeformationG.value, SliderColorDeformationB.value, Mathf.Max(SliderColorDeformationA.value, 0.1f));
		Arena.ColorDeformationActive = ToogleColorDeformation.isOn;
		Arena.ColorDeformation = ImageDeformationColor.color;
		Arena.ColorDeformationItensity = SliderColorDeformationIntesity.value;
		Arena.ColorDeformationUseCarColor = ToggleUseCarColor.isOn;
		Arena.ApplyColorDeformation();
	}
}
