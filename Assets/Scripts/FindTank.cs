using UnityEngine;

public class FindTank : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerPrefs.GetInt("FindTank") == 2)
		{
			base.gameObject.SetActive(value: false);
		}
		Invoke("findTankHide", 6f);
	}

	private void findTankHide()
	{
		PlayerPrefs.SetInt("FindTank", PlayerPrefs.GetInt("FindTank") + 1);
		base.gameObject.SetActive(value: false);
	}
}
