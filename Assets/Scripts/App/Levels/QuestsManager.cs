using App.GameConfig;
using App.SaveSystem;
using App.Util;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace App.Levels
{
	public class QuestsManager : MonoBehaviour
	{
		private PlayerSaveEntity playerSave;

		private GameConfigScriptableObject gameConfig;

		public Text coiny;

		public Text Gangster;

		public Text streetCop;

		public Text car;

		public Text bike;

		public Text vehicle;

		private int coinyy;

		private int gang;

		private int cop;

		private int carN;

		private int bikeN;

		private int vehicN;

		private int coinyyMax = 31;

		private int gangMax = 3;

		private int copMax = 3;

		private int carNMax = 3;

		private int bikeNMax = 2;

		private int vehicNMax = 3;

		private bool coinyyBool;

		private bool gangBool;

		private bool copBool;

		private bool carNBool;

		private bool bikeNBool;

		private bool vehicNBool;

		private bool konec;

		public GameObject konecHry;

		public GameObject dalsiLevel;

		private int questCounter;

		public void ProccessCoin()
		{
			if (coinyy <= coinyyMax)
			{
				coinyy++;
				PlayerPrefs.SetInt("coiny", PlayerPrefs.GetInt("coiny") + 1);
				coiny.text = string.Format(LocalizationManager.Instance.GetText(5000), coinyy + "/" + coinyyMax);
			}
			if (coinyy == coinyyMax)
			{
				coinyyBool = true;
			}
		}

		private IEnumerator test()
		{
			yield return new WaitForSeconds(3f);
			pridateCar();
			yield return new WaitForSeconds(3f);
			pridateBike();
			yield return new WaitForSeconds(3f);
			pridateVehicle();
			yield return new WaitForSeconds(3f);
			pridateGang();
			yield return new WaitForSeconds(3f);
			pridateCop();
			yield return new WaitForSeconds(3f);
			pridateCar();
			yield return new WaitForSeconds(3f);
			pridateBike();
			yield return new WaitForSeconds(3f);
			pridateVehicle();
			yield return new WaitForSeconds(3f);
			pridateGang();
			yield return new WaitForSeconds(3f);
			pridateCop();
			yield return new WaitForSeconds(3f);
			pridateCar();
			yield return new WaitForSeconds(3f);
			pridateBike();
			yield return new WaitForSeconds(3f);
			pridateVehicle();
			yield return new WaitForSeconds(3f);
			pridateGang();
			yield return new WaitForSeconds(3f);
			pridateCop();
		}

		private void Start()
		{
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Menu")
			{
				if (!PlayerPrefs.HasKey("coiny"))
				{
					PlayerPrefs.SetInt("coiny", 0);
					PlayerPrefs.SetInt("carNMax", 3);
					PlayerPrefs.SetInt("bikeNMax", 2);
					PlayerPrefs.SetInt("vehicNMax", 3);
					PlayerPrefs.SetInt("gangMax", 3);
					PlayerPrefs.SetInt("copMax", 3);
				}
				coinyy = PlayerPrefs.GetInt("coiny");
				coiny.text = string.Format(LocalizationManager.Instance.GetText(5000), coinyy + "/" + coinyyMax);
				carN = PlayerPrefs.GetInt("carN");
				bikeN = PlayerPrefs.GetInt("bikeN");
				vehicN = PlayerPrefs.GetInt("vehicN");
				gang = PlayerPrefs.GetInt("gang");
				cop = PlayerPrefs.GetInt("cop");
				carNMax = PlayerPrefs.GetInt("carNMax");
				bikeNMax = PlayerPrefs.GetInt("bikeNMax");
				vehicNMax = PlayerPrefs.GetInt("vehicNMax");
				gangMax = PlayerPrefs.GetInt("gangMax");
				copMax = PlayerPrefs.GetInt("copMax");
				coiny.text = string.Format(LocalizationManager.Instance.GetText(5000), coinyy + "/" + coinyyMax);
				Gangster.text = string.Format(LocalizationManager.Instance.GetText(5001), gang + "/" + gangMax);
				streetCop.text = string.Format(LocalizationManager.Instance.GetText(5002), cop + "/" + copMax);
				car.text = string.Format(LocalizationManager.Instance.GetText(5003), carN + "/" + carNMax);
				bike.text = string.Format(LocalizationManager.Instance.GetText(5004), bikeN + "/" + bikeNMax);
				vehicle.text = string.Format(LocalizationManager.Instance.GetText(5005), vehicN + "/" + vehicNMax);
				questCounter = PlayerPrefs.GetInt("questCounter");
				playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
				gameConfig = ServiceLocator.Get<ConfigContainer>().gameConfig;
				if (carN != carNMax || bikeN != bikeNMax || vehicN != vehicNMax || gang != gangMax || cop != copMax)
				{
					checkingBool();
					dalsiLevelHide();
				}
			}
		}

		private void checkingBool()
		{
			if (carN == carNMax)
			{
				carNBool = true;
			}
			if (bikeN == bikeNMax)
			{
				bikeNBool = true;
			}
			if (vehicN == vehicNMax)
			{
				vehicNBool = true;
			}
			if (gang == gangMax)
			{
				gangBool = true;
			}
			if (cop == copMax)
			{
				copBool = true;
			}
		}

		public void pridateCar()
		{
			if (!carNBool)
			{
				if (carN < carNMax)
				{
					carN++;
				}
				car.text = string.Format(LocalizationManager.Instance.GetText(5003), carN + "/" + carNMax);
				PlayerPrefs.SetInt("carN", carN);
				checkingBool();
				if (carN == carNMax)
				{
					carNBool = true;
					dalsiLevel.GetComponent<Text>().enabled = true;
					dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5016), car.text);
					Invoke("dalsiLevelHide", 2f);
					RewardMission();
				}
			}
		}

		public void pridateBike()
		{
			if (!bikeNBool)
			{
				if (bikeN < bikeNMax)
				{
					bikeN++;
				}
				bike.text = string.Format(LocalizationManager.Instance.GetText(5004), bikeN + "/" + bikeNMax);
				PlayerPrefs.SetInt("bikeN", bikeN);
				if (bikeN == bikeNMax)
				{
					bikeNBool = true;
					dalsiLevel.GetComponent<Text>().enabled = true;
					dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5016), bike.text);
					Invoke("dalsiLevelHide", 2f);
					RewardMission();
				}
				checkingBool();
			}
		}

		public void pridateVehicle()
		{
			if (!vehicNBool)
			{
				if (vehicN < vehicNMax)
				{
					vehicN++;
				}
				vehicle.text = string.Format(LocalizationManager.Instance.GetText(5005), vehicN + "/" + vehicNMax);
				PlayerPrefs.SetInt("vehicN", vehicN);
				if (vehicN == vehicNMax)
				{
					vehicNBool = true;
					dalsiLevel.GetComponent<Text>().enabled = true;
					dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5016), vehicle.text);
					Invoke("dalsiLevelHide", 2f);
					RewardMission();
				}
				checkingBool();
			}
		}

		public void pridateGang()
		{
			if (!gangBool)
			{
				if (gang < gangMax)
				{
					gang++;
				}
				Gangster.text = string.Format(LocalizationManager.Instance.GetText(5001), gang + "/" + gangMax);
				PlayerPrefs.SetInt("gang", gang);
				if (gang == gangMax)
				{
					gangBool = true;
					dalsiLevel.GetComponent<Text>().enabled = true;
					dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5016), Gangster.text);
					Invoke("dalsiLevelHide", 2f);
					RewardMission();
				}
				checkingBool();
			}
		}

		public void pridateCop()
		{
			if (!copBool)
			{
				if (cop < copMax)
				{
					cop++;
				}
				streetCop.text = string.Format(LocalizationManager.Instance.GetText(5002), cop + "/" + copMax);
				PlayerPrefs.SetInt("cop", cop);
				if (cop == copMax)
				{
					copBool = true;
					dalsiLevel.GetComponent<Text>().enabled = true;
					dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5016), streetCop.text);
					Invoke("dalsiLevelHide", 2f);
					RewardMission();
				}
				checkingBool();
			}
		}

		private void Update()
		{
			if (!gangBool || !vehicNBool || !bikeNBool || !carNBool || !copBool || konec)
			{
				return;
			}
			questCounter++;
			if (questCounter >= 5)
			{
				if (coinyyBool)
				{
					Invoke("GameFinish", 2f);
					konec = true;
				}
				return;
			}
			if (questCounter < 5)
			{
				Invoke("NextLevel", 2f);
			}
			gangMax *= 2;
			copMax *= 2;
			carNMax *= 2;
			bikeNMax *= 2;
			vehicNMax *= 2;
			Gangster.text = string.Format(LocalizationManager.Instance.GetText(5001), gang + "/" + gangMax);
			streetCop.text = string.Format(LocalizationManager.Instance.GetText(5002), cop + "/" + copMax);
			car.text = string.Format(LocalizationManager.Instance.GetText(5003), carN + "/" + carNMax);
			bike.text = string.Format(LocalizationManager.Instance.GetText(5004), bikeN + "/" + bikeNMax);
			vehicle.text = string.Format(LocalizationManager.Instance.GetText(5005), vehicN + "/" + vehicNMax);
			gangBool = false;
			vehicNBool = false;
			bikeNBool = false;
			carNBool = false;
			copBool = false;
			PlayerPrefs.SetInt("carNMax", carNMax);
			PlayerPrefs.SetInt("bikeNMax", bikeNMax);
			PlayerPrefs.SetInt("vehicNMax", vehicNMax);
			PlayerPrefs.SetInt("gangMax", gangMax);
			PlayerPrefs.SetInt("copMax", copMax);
		}

		private void NextLevel()
		{
			CancelInvoke("dalsiLevelHide");
			PlayerPrefs.SetInt("questCounter", questCounter);
			dalsiLevel.GetComponent<Text>().enabled = true;
			dalsiLevel.GetComponent<Text>().text = string.Format(LocalizationManager.Instance.GetText(5017), questCounter);
			Invoke("dalsiLevelHide", 4f);
		}

		private void GameFinish()
		{
			konecHry.SetActive(value: true);
			ServiceLocator.Get<Pauser>().endGame = true;
			Time.timeScale = 0f;
		}

		private void dalsiLevelHide()
		{
			dalsiLevel.GetComponent<Text>().enabled = false;
		}

		private void RewardMission()
		{
			if (gameConfig.title == GameTitle.Gta)
			{
				int num = Mathf.RoundToInt(UnityEngine.Random.Range(100, 300) / 50) * 50;
				playerSave.score += num;
				playerSave.Save();
			}
		}
	}
}
