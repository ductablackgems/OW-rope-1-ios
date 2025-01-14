using UnityEngine;

public class saveHidingScript : MonoBehaviour
{
	public string Nazev;

	private int id;

	private void Awake()
	{
		if (PlayerPrefs.GetInt(Nazev) == 1)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerPrefs.SetInt(Nazev, 1);
		}
	}
}
