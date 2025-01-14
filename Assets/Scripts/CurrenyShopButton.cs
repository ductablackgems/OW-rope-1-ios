using System.Collections;
using UnityEngine;

public class CurrenyShopButton : MonoBehaviour
{
	public GameObject IAPShop;

	public GameObject NGUIOff;

	private Vector2 position;

	private float targetTime = 3f;

	private void Awake()
	{
		position = base.transform.position;
	}

	private IEnumerator Start()
	{
		base.transform.position = new Vector2(0f, -5000f);
		while (IAPShop == null)
		{
			targetTime -= Time.deltaTime;
			if (targetTime <= 0f)
			{
				StopAllCoroutines();
			}
			yield return IAPShop != null;
		}
		if (IAPShop != null)
		{
			base.transform.position = position;
		}
	}

	private void OnClick()
	{
		if (IAPShop != null)
		{
			IAPShop.SetActive(value: true);
		}
		else
		{
			UnityEngine.Debug.LogError("Chybí IAPShop. Rada: Pusť hru přes menu");
			base.gameObject.SetActive(value: false);
		}
		if (NGUIOff != null && IAPShop != null)
		{
			NGUIOff.SetActive(value: true);
			if (Time.timeScale > 0.1f)
			{
				Time.timeScale = 0.0001f;
				IAPShop.transform.parent.GetComponent<FulfillManager>().pause = true;
			}
		}
	}
}
