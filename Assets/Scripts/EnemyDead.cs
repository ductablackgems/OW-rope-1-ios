using UnityEngine;

public class EnemyDead : MonoBehaviour
{
	public int ScorePlus = 1;

	public int MoneyPlus = 20;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnDead()
	{
		BuyMenu buyMenu = (BuyMenu)UnityEngine.Object.FindObjectOfType(typeof(BuyMenu));
		if ((bool)buyMenu)
		{
			buyMenu.Money += MoneyPlus;
			buyMenu.Score += ScorePlus;
		}
	}
}
