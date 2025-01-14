using App.GUI;
using App.Player;
using App.SaveSystem;
using App.Settings;
using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.Dogs
{
	public class DogManager : MonoBehaviour
	{
		public enum DogSlot
		{
			None,
			Primary,
			Secondary,
			Tertiary
		}

		private const float VEHICLE_ENTER_DESPAWN_AFTER = 4f;

		[SerializeField]
		private bool disableDogSpawn;

		private PlayerModel player;

		private PlayerSaveEntity playerSave;

		private DogSettings dogSettings;

		private List<Dog> dogs = new List<Dog>(3);

		private List<DogSpot> spots = new List<DogSpot>();

		private DurationTimer despawnTimer = new DurationTimer();

		private Dog dogToRevive;

		private DogRevivePanel revivePanel;

		private PanelsManager panelsManager;

		public List<Dog> Dogs => dogs;

		public void LodAllDogs(List<Dog> list)
		{
			foreach (DogSaveEntity dog in playerSave.dogs)
			{
				CreateAndAddDog(dog, list);
			}
		}

		public bool Buy(Dog dog)
		{
			DogSaveEntity dogSave = GetDogSave(dog);
			if (dogSave == null)
			{
				return false;
			}
			int price = dog.Settings.Price;
			if (playerSave.score < price)
			{
				return false;
			}
			playerSave.score -= price;
			dogSave.isOwned = true;
			dogSave.Save();
			playerSave.Save();
			return true;
		}

		public void Equip(Dog dog, DogSlot slot)
		{
			DogSaveEntity dogSave = GetDogSave(dog);
			if (dogSave != null)
			{
				if (slot != 0)
				{
					foreach (DogSaveEntity dog2 in playerSave.dogs)
					{
						if (dog2.Slot == slot)
						{
							dog2.Slot = DogSlot.None;
							dog2.Save();
						}
					}
				}
				dogSave.Slot = slot;
				dogSave.Save();
			}
		}

		public void Unequip(Dog dog)
		{
			Equip(dog, DogSlot.None);
		}

		public void Revive(Dog dog)
		{
			if (!(dog == null))
			{
				dog.Revive();
			}
		}

		public bool CanReviveWithAds()
		{
			return CallAdsManager.RewardedIsReady();
		}

		public void ReviveWithAd(Dog dog)
		{
			if (!(dog == null) && CanReviveWithAds())
			{
				CallAdsManager.ShowRewardVideo(() =>
				{
					OnDogReviveRequest(dog);
				});
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Start()
		{
			if (!disableDogSpawn)
			{
				SpawnDogs();
			}
		}

		private void Update()
		{
			UpdateDespawnTimer();
			if (InputUtils.DogAttack.IsDown)
			{
				AttackSelectedTarget();
			}
		}

		private void OnPlayerRespawned(object sender, object data)
		{
			foreach (Dog dog in dogs)
			{
				if (dog.IsAlive || dog.IsPrimary)
				{
					dog.Respawn();
				}
			}
		}

		private void OnPlayerRespawnDialogClosed(object sender, object data)
		{
			if (CanManualRevive())
			{
				revivePanel = (panelsManager.ShowPanel(PanelType.DogRevive) as DogRevivePanel);
				revivePanel.Initialize(OnDogReviveRequest, CanReviveWithAds, CloseRevivePanel);
				revivePanel.Show(dogs);
			}
		}

		private bool CanManualRevive()
		{
			foreach (Dog dog in dogs)
			{
				if (!dog.IsAlive && !dog.IsPrimary)
				{
					return true;
				}
			}
			return false;
		}

		private void OnPlayerVehicleEnter(object sender, object data)
		{
			despawnTimer.Run(4f);
		}

		private void OnPlayerVehicleLeave(object sender, object data)
		{
			despawnTimer.Stop();
			SpawnDogs();
		}

		private void OnDogReviveRequest(Dog dog)
		{			
			dogToRevive = dog;
			if (!(dogToRevive == null))
			{
				Revive(dogToRevive);
				dogToRevive = null;
				if (!(revivePanel == null) && !CanManualRevive())
				{
					CloseRevivePanel();
				}
			}
		}

		private void Initialize()
		{
			player = ServiceLocator.GetPlayerModel();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			panelsManager = ServiceLocator.Get<PanelsManager>();
			dogSettings = SettingsManager.DogSettings;
			GetComponentsInChildren(spots);
			TryAssignDefaultDog();
			if (!disableDogSpawn)
			{
				LoadEquippedDogs();
			}
			ServiceLocator.SubscibeMessage(MessageID.Player.Respawned, this, OnPlayerRespawned);
			ServiceLocator.SubscibeMessage(MessageID.Player.RespawnDialogClosed, this, OnPlayerRespawnDialogClosed);
			ServiceLocator.SubscibeMessage(MessageID.Player.VehicleEnter, this, OnPlayerVehicleEnter);
			ServiceLocator.SubscibeMessage(MessageID.Player.VehicleLeave, this, OnPlayerVehicleLeave);
		}

		private void LoadEquippedDogs()
		{
			foreach (DogSaveEntity dog in playerSave.dogs)
			{
				if (dog.Slot != 0)
				{
					CreateAndAddDog(dog, dogs);
				}
			}
		}

		private void CreateAndAddDog(DogSaveEntity dogSave, List<Dog> list)
		{
			DogSettingsItem dogSettingsItem = SettingsManager.DogSettings[dogSave.SettingsID];
			Dog dog = UnityEngine.Object.Instantiate(dogSettingsItem.Prefab);
			DogSpot dogSpot = GetDogSpot(dogSave);
			dog.Initialize(dogSettingsItem, dogSave, dogSpot, player);
			dog.gameObject.SetActive(value: false);
			list.Add(dog);
			if (dog.IsPrimary && !dog.IsAlive)
			{
				dog.Revive();
			}
		}

		private void TryAssignDefaultDog()
		{
			DogSaveEntity dogSave = playerSave.GetDogSave(SettingsManager.DogSettings.DefaultDogID);
			if (!dogSave.isOwned)
			{
				dogSave.isOwned = true;
				dogSave.Slot = DogSlot.Primary;
				playerSave.Save();
			}
		}

		private void SpawnDogs()
		{
			foreach (Dog dog in dogs)
			{
				if (!dog.IsSpawned && dog.IsAlive)
				{
					dog.Spawn();
				}
			}
		}

		private DogSpot GetDogSpot(DogSaveEntity saveEntity)
		{
			foreach (DogSpot spot in spots)
			{
				if (spot.Slot == saveEntity.Slot)
				{
					return spot;
				}
			}
			return spots[0];
		}

		private void DespawnDogs()
		{
			foreach (Dog dog in dogs)
			{
				if (dog.IsSpawned)
				{
					dog.Despawn();
				}
			}
		}

		private void AttackSelectedTarget()
		{
			foreach (Dog dog in dogs)
			{
				dog.AttackTargetInDirection();
			}
		}

		private void UpdateDespawnTimer()
		{
			if (despawnTimer.Done())
			{
				despawnTimer.Stop();
				DespawnDogs();
			}
		}

		private DogSaveEntity GetDogSave(Dog dog)
		{
			if (dog == null)
			{
				return null;
			}
			foreach (DogSaveEntity dog2 in playerSave.dogs)
			{
				if (dog2.SettingsID == dog.Settings.ID)
				{
					return dog2;
				}
			}
			return null;
		}

		private void CloseRevivePanel()
		{
			if (!(revivePanel == null))
			{
				panelsManager.ShowPanel(PanelType.Game);
				revivePanel = null;
				foreach (Dog dog in dogs)
				{
					if (dog.IsAlive)
					{
						dog.Respawn();
					}
				}
			}
		}
	}
}
