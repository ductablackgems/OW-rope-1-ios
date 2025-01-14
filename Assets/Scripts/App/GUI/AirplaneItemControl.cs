using App.Prefabs;
using App.Shop;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class AirplaneItemControl : MonoBehaviour
	{
		public enum EState
		{
			Buy,
			Spawn,
			Respawn
		}

		[SerializeField]
		private Text textName;

		[SerializeField]
		private Text textPrice;

		[SerializeField]
		private Text textRepairPrice;

		[SerializeField]
		private Text textRespawnPrice;

		[SerializeField]
		private Image image;

		[SerializeField]
		private Button buttonBuy;

		[SerializeField]
		private Button buttonSpawn;

		[SerializeField]
		private Button buttonRespawn;

		private bool isOwned;

		private EState state;

		private Color colorPrice;

		private Color colorRespawn;

		public AirplaneShopInfo ShopInfo
		{
			get;
			private set;
		}

		public VehiclePrefabId ID
		{
			get;
			private set;
		}

		public EState State
		{
			get
			{
				return state;
			}
			set
			{
				SetState(value);
			}
		}

		public event Action<AirplaneItemControl> ItemBuy;

		public event Action<AirplaneItemControl> ItemSpawn;

		public event Action<AirplaneItemControl> ItemRespawn;

		public void Initialize(VehiclePrefabId prefabID)
		{
			ID = prefabID;
			ShopInfo = ID.GetComponent<AirplaneShopInfo>();
			textName.text = ShopInfo.itemName;
			image.sprite = ShopInfo.image;
			colorPrice = textPrice.color;
			colorRespawn = textRespawnPrice.color;
			SetPrice(textPrice, ShopInfo.price);
			SetPrice(textRepairPrice, ShopInfo.repairPrice);
			SetPrice(textRespawnPrice, ShopInfo.respawnPrice);
			buttonBuy.onClick.AddListener(OnButtonBuyClicked);
			buttonSpawn.onClick.AddListener(OnButtonSpawnClicked);
			buttonRespawn.onClick.AddListener(OnButtonRespawnClicked);
		}

		public void Refresh(int availableMoney)
		{
			SetPriceColor(textPrice, colorPrice, ShopInfo.price < availableMoney);
			SetPriceColor(textRespawnPrice, colorPrice, ShopInfo.respawnPrice < availableMoney);
		}

		private void OnButtonBuyClicked()
		{
			this.ItemBuy(this);
		}

		private void OnButtonSpawnClicked()
		{
			this.ItemSpawn(this);
		}

		private void OnButtonRespawnClicked()
		{
			this.ItemRespawn(this);
		}

		private void SetState(EState state)
		{
			this.state = state;
			buttonBuy.gameObject.SetActive(state == EState.Buy);
			buttonSpawn.gameObject.SetActive(state == EState.Spawn);
			buttonRespawn.gameObject.SetActive(state == EState.Respawn);
		}

		private void SetPrice(Text text, int price)
		{
			if (!(text == null))
			{
				text.text = $"${price}";
			}
		}

		private static void SetPriceColor(Text text, Color colorAvailable, bool isAvailable)
		{
			text.color = ((!isAvailable) ? Color.red : colorAvailable);
		}
	}
}
