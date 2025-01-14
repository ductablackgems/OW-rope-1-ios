using System.Collections;
using UnityEngine;

public class TimeHide : MonoBehaviour
{
	public float Timed = 3f;

	private bool Pause;

	private void OnEnable()
	{
		if ((double)Time.timeScale < 0.1)
		{
			Pause = true;
			Time.timeScale = 1f;
		}
		StartCoroutine(RemoveAfterSeconds(Timed));
	}

	private IEnumerator RemoveAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		base.gameObject.SetActive(value: false);
		if (Pause)
		{
			Pause = false;
			Time.timeScale = 0.0001f;
		}
	}
}
