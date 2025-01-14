using App.GUI.Panels;
using App.SaveSystem;
using App.Shop;
using App.Spawn;
using App.Vehicles.Airplane;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class AirplaneSelectionPanel : AbstractPanel
	{
		[SerializeField]
		private AirplaneItemControl itemPrefab;

		[SerializeField]
		private RectTransform listControl;

		[SerializeField]
		private Button buttonBack;

		[Space]
		[SerializeField]
		private AudioClip clickSound;

		private AirplaneItemControl[] items;

		private AirplaneSpawner spawner;

		private PlayerSaveEntity playerSave;

		private ShopSounds shopSounds;

		public event Action Close;


		public override PanelType GetPanelType()
		{
			return PanelType.AirplaneSelection;
		}

		private void OnEnable()
		{
			Initialize();
			RefreshItems();
		}

		private void OnItemBuy(AirplaneItemControl control)
		{
			if (TryProcessPayment(control.ShopInfo.price))
			{
				playerSave.buyedAirplanes.Add(control.ID.tid);
				playerSave.Save();
				control.State = AirplaneItemControl.EState.Spawn;
				RefreshItems();
			}
		}

		private void OnItemSpawn(AirplaneItemControl control)
		{
			spawner.Spawn(control.ID.tid);
			control.State = AirplaneItemControl.EState.Respawn;
			PlayClickSound();
		}

		private void OnItemRespawn(AirplaneItemControl control)
		{
			if (TryProcessPayment(control.ShopInfo.respawnPrice))
			{
				spawner.Respawn(control.ID.tid);
			}
		}

		private void OnButtonBackClicked()
		{
			PlayClickSound();
			this.Close();
		}

		private bool TryProcessPayment(int price)
		{
			if (playerSave.score < price)
			{
				shopSounds.Play(shopSounds.NoMoneySound);
				return false;
			}
			playerSave.score -= price;
			shopSounds.Play(shopSounds.buySound);
			playerSave.Save();
			return true;
		}

		private void Initialize()
		{
			if (items == null)
			{
				buttonBack.onClick.AddListener(OnButtonBackClicked);
				playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
				spawner = ServiceLocator.Get<AirplaneSpawner>();
				shopSounds = ServiceLocator.Get<ShopSounds>();
				items = new AirplaneItemControl[spawner.Prefabs.Count];
				for (int i = 0; i < spawner.Prefabs.Count; i++)
				{
					AirplaneController airplaneController = spawner.Prefabs[i];
					AirplaneItemControl airplaneItemControl = UnityEngine.Object.Instantiate(itemPrefab, listControl);
					airplaneItemControl.Initialize(airplaneController.ID);
					airplaneItemControl.State = (playerSave.buyedAirplanes.Contains(airplaneController.ID.tid) ? AirplaneItemControl.EState.Spawn : AirplaneItemControl.EState.Buy);
					airplaneItemControl.ItemBuy += OnItemBuy;
					airplaneItemControl.ItemSpawn += OnItemSpawn;
					airplaneItemControl.ItemRespawn += OnItemRespawn;
					items[i] = airplaneItemControl;
				}
			}
		}

		private void RefreshItems()
		{
			AirplaneItemControl[] array = items;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Refresh(playerSave.score);
			}
		}

		private void PlayClickSound()
		{
			shopSounds.Play(clickSound);
		}
	}
}
