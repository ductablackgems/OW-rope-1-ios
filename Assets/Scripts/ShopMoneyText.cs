using App;
using App.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class ShopMoneyText : MonoBehaviour
{
	private Text moneyText;

	private PlayerSaveEntity playerSave;

	protected void OnEnable()
	{
		moneyText = GetComponent<Text>();
		playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
		playerSave.OnSave += OnSave;
		UpdateScore();
	}

	protected void OnDisable()
	{
		playerSave.OnSave -= OnSave;
	}

	private void OnSave(AbstractSaveEntity entity)
	{
		UpdateScore();
	}

	private void UpdateScore()
	{
		moneyText.text = playerSave.score.ToString("N0");
	}
}
