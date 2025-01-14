using UnityEngine;

public class HideTriggers : MonoBehaviour
{
	public GameObject[] objekty;

	public bool[] volba;

	public bool optimalization;

	private bool Enter;

	private void Start()
	{
		if (!optimalization)
		{
			return;
		}
		for (int i = 0; i < objekty.Length; i++)
		{
			if (objekty[i] != null)
			{
				objekty[i].SetActive(value: false);
			}
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (Enter || !(col.tag == "Player"))
		{
			return;
		}
		Enter = true;
		for (int i = 0; i < objekty.Length; i++)
		{
			if (objekty[i] != null)
			{
				objekty[i].SetActive(volba[i]);
			}
		}
		if (!optimalization)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (!optimalization || !(col.tag == "Player"))
		{
			return;
		}
		Enter = false;
		for (int i = 0; i < objekty.Length; i++)
		{
			if (objekty[i] != null)
			{
				objekty[i].SetActive(value: false);
			}
		}
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
