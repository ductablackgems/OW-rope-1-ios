using UnityEngine;

public class SelectDifficulty : MonoBehaviour
{
	public void SelectEasy(GameObject easy, GameObject med, GameObject hard)
	{
		easy.GetComponent<UIToggle>().value = true;
		med.GetComponent<UIToggle>().value = false;
		hard.GetComponent<UIToggle>().value = false;
	}

	public void SelectMed(GameObject easy, GameObject med, GameObject hard)
	{
		easy.GetComponent<UIToggle>().value = false;
		med.GetComponent<UIToggle>().value = true;
		hard.GetComponent<UIToggle>().value = false;
	}

	public void SelectHard(GameObject easy, GameObject med, GameObject hard)
	{
		med.GetComponent<UIToggle>().value = false;
		easy.GetComponent<UIToggle>().value = false;
		hard.GetComponent<UIToggle>().value = true;
	}
}
