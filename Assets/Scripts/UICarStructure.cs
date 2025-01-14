using UnityEngine;
using UnityEngine.UI;

public class UICarStructure : MonoBehaviour
{
	public Car Car;

	private Image Image;

	private void Awake()
	{
		Image = GetComponent<Image>();
	}

	private void Update()
	{
		Image.fillAmount = 1f - Car.CarDamage;
	}
}
