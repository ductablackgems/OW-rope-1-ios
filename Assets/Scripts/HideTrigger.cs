using UnityEngine;

public class HideTrigger : MonoBehaviour
{
	private void Start()
	{
		if (PlayerPrefs.GetInt("tutorial") == 1)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
