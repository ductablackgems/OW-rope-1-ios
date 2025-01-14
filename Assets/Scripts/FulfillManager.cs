using App;
using App.SaveSystem;
using App.Shop;
using UnityEngine;
using UnityEngine.UI;

public class FulfillManager : MonoBehaviour
{
	private PlayerSaveEntity playerSave;

	public GameObject IAPShop;

	public GameObject NGUIOff;

	public GunShopView gunShopView;

	public Text moneyText;

	public bool pause;

	private int ID;

	private static FulfillManager instance;

	private SettingsSaveEntity settingsSave;

	private string App = "scsw";

	private void Awake()
	{
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
		if (Application.identifier == "com.hgamesart.ninjafrogrope")
		{
			App = "ninjafrogrope";
		}
		else if (Application.identifier == "com.hgamesart.crimedriver")
		{
			App = "gasn";
		}
		else if (Application.identifier == "com.hgamesart.ropestickman")
		{
			App = "ropestickman";
		}
		else if (Application.identifier == "com.hgamesart.ironmech")
		{
			App = "scsw";
		}
		else
		{
			App = "scsw";
		}
	}

	private void Start()
	{
		gunShopView = ServiceLocator.Get<GunShopView>();
		Invoke("FindButtons", 0.5f);
	}

	private void OnLevelWasLoaded(int level)
	{
		gunShopView = ServiceLocator.Get<GunShopView>();
		Invoke("FindButtons", 0.5f);
	}

	private void FindButtons()
	{
		NGUIOff = gunShopView.RewardUI[0];
		gunShopView.RewardUI[1] = IAPShop;
		gunShopView.RewardUI[2].GetComponent<CurrenyShopButton>().IAPShop = IAPShop;
		gunShopView.RewardUI[3].GetComponent<CurrenyShopButton>().IAPShop = IAPShop;
	}

	private void Fulfill(string productId)
	{
		if (productId == App + "_remove_ads")
		{
			ID = 1;
		}
		else if (productId == App + "_2500")
		{
			ID = 2;
		}
		else if (productId == App + "_15000")
		{
			ID = 3;
		}
		else if (productId == App + "_40000")
		{
			ID = 4;
		}
		else if (productId == App + "_100000")
		{
			ID = 5;
		}
		else if (productId == App + "_300000")
		{
			ID = 6;
		}
		else if (productId == App + "_1000000")
		{
			ID = 7;
		}
		switch (ID)
		{
		case 1:
			reklam();
			break;
		case 2:
			pridatPenize(2500);
			reklam();
			break;
		case 3:
			pridatPenize(15000);
			reklam();
			break;
		case 4:
			pridatPenize(40000);
			reklam();
			break;
		case 5:
			pridatPenize(100000);
			reklam();
			break;
		case 6:
			pridatPenize(300000);
			reklam();
			break;
		case 7:
			pridatPenize(1000000);
			reklam();
			break;
		default:
			UnityEngine.Debug.Log($"Unrecognized productId \"{productId}\"");
			break;
		}
	}

	private void reklam()
	{
		settingsSave.reklamy = 1f;
		settingsSave.Save();
	}

	private void pridatPenize(int Penize)
	{
		playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
		playerSave.score += Penize;
		playerSave.Save();
		GetComponent<AudioSource>().Play();
	}

	public void NGUIOn(bool hodnota)
	{
		NGUIOff.SetActive(hodnota);
		if (pause)
		{
			MonoBehaviour.print("pausa on");
			Time.timeScale = 1f;
			pause = false;
		}
	}
}
