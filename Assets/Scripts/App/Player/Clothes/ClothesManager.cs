using App.SaveSystem;
using App.Shop;
using App.Shop.Slider;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player.Clothes
{
	public class ClothesManager : MonoBehaviour
	{
		public SkinnedMeshRenderer[] skinRenderer;

		private PlayerSaveEntity playerSave;

		private ShopSounds shopSounds;

		private Dictionary<ClothesKind, List<ClothesItem>> clothes = new Dictionary<ClothesKind, List<ClothesItem>>();

		private ClothesKind[] optionableClothesKinds = new ClothesKind[3]
		{
			ClothesKind.Hat,
			ClothesKind.Glasses,
			ClothesKind.Mask
		};

		private AbstractShopItemView abstractShopItemView;

		public event Action OnAction;

		public ClothesItem GetActiveItem(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
				return null;
			}
			foreach (ClothesItem item in value)
			{
				if (item.gameObject.activeSelf)
				{
					return item;
				}
			}
			return null;
		}

		public ClothesItem GetWearedItem(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
				return null;
			}
			foreach (ClothesItem item in value)
			{
				if (item.weared)
				{
					return item;
				}
			}
			return null;
		}

		public void MoveRight(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
				return;
			}
			shopSounds.Play(shopSounds.slideSound);
			bool flag = Array.IndexOf(optionableClothesKinds, kind) > -1;
			ClothesItem x = null;
			foreach (ClothesItem item in value)
			{
				if (x != null)
				{
					item.SetActive(active: true, skinRenderer);
					if (this.OnAction != null)
					{
						this.OnAction();
					}
					return;
				}
				if (item.gameObject.activeSelf)
				{
					x = item;
					item.SetActive(active: false);
				}
			}
			if (x == null || !flag)
			{
				value[0].SetActive(active: true, skinRenderer);
			}
			if (this.OnAction != null)
			{
				this.OnAction();
			}
		}

		public void MoveLeft(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
				return;
			}
			shopSounds.Play(shopSounds.slideSound);
			bool flag = Array.IndexOf(optionableClothesKinds, kind) > -1;
			ClothesItem clothesItem = null;
			foreach (ClothesItem item in value)
			{
				if (item.gameObject.activeSelf)
				{
					item.SetActive(active: false);
					if (clothesItem != null)
					{
						clothesItem.SetActive(active: true, skinRenderer);
					}
					else if (!flag)
					{
						value[value.Count - 1].SetActive(active: true, skinRenderer);
					}
					if (this.OnAction != null)
					{
						this.OnAction();
					}
					return;
				}
				clothesItem = item;
			}
			clothesItem.SetActive(active: true, skinRenderer);
			if (this.OnAction != null)
			{
				this.OnAction();
			}
		}

		public void TryWearCurrentItem(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
				return;
			}
			ClothesItem clothesItem = null;
			ClothesItem clothesItem2 = null;
			foreach (ClothesItem item in value)
			{
				if (item.gameObject.activeSelf)
				{
					clothesItem = item;
				}
				if (item.weared)
				{
					clothesItem2 = item;
				}
			}
			if (!(clothesItem != null) || clothesItem.buyed)
			{
				if (clothesItem2 != null)
				{
					clothesItem2.weared = false;
					playerSave.wearedClothes.Remove(clothesItem2.tid);
				}
				if (clothesItem != null)
				{
					clothesItem.weared = true;
					playerSave.wearedClothes.Add(clothesItem.tid);
				}
				playerSave.Save();
				if (this.OnAction != null)
				{
					this.OnAction();
				}
			}
		}

		public void TryBuyCurrentItem(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
			}
			else
			{
				foreach (ClothesItem item in value)
				{
					if (item.gameObject.activeSelf)
					{
						if (!item.buyed && item.price <= playerSave.score)
						{
							shopSounds.Play(shopSounds.buySound);
							playerSave.score -= item.price;
							item.buyed = true;
							playerSave.buyedClothes.Add(item.tid);
							playerSave.Save();
							if (this.OnAction != null)
							{
								this.OnAction();
							}
						}
						else
						{
							shopSounds.Play(shopSounds.NoMoneySound);
							if (abstractShopItemView.RewardUI.Length != 0)
							{
								for (int i = 0; i < 2; i++)
								{
									if (abstractShopItemView.RewardUI[i] != null)
									{
										abstractShopItemView.RewardUI[i].SetActive(value: true);
									}
								}
							}
						}
					}
				}
			}
		}

		public void TryUnlockBuyVideoCurrentItem(ClothesKind kind, GameObject obj)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
			}
			else
			{
				foreach (ClothesItem item in value)
				{
					if (item.gameObject.activeSelf)
					{
						shopSounds.Play(shopSounds.buySound);
						item.buyed = true;
						playerSave.buyedClothes.Add(item.tid);
						playerSave.Save();
						if (this.OnAction != null)
						{
							this.OnAction();
						}
						obj.SetActive(false);
					}
				}
			}
		}

		public void RevertItem(ClothesKind kind)
		{
			if (!clothes.TryGetValue(kind, out List<ClothesItem> value))
			{
				UnityEngine.Debug.LogErrorFormat("There's no clothes of kind '{0}'.", kind);
			}
			else
			{
				foreach (ClothesItem item in value)
				{
					item.SetActive(item.weared, skinRenderer);
				}
			}
		}

		private void Awake()
		{
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			shopSounds = ServiceLocator.Get<ShopSounds>();
			abstractShopItemView = ServiceLocator.Get<AbstractShopItemView>();
			ClothesItem[] componentsInChildren = GetComponentsInChildren<ClothesItem>(includeInactive: true);
			foreach (ClothesItem clothesItem in componentsInChildren)
			{
				clothesItem.buyed = (clothesItem.price == 0 || playerSave.buyedClothes.Contains(clothesItem.tid));
				clothesItem.weared = playerSave.wearedClothes.Contains(clothesItem.tid);
				if (!clothes.TryGetValue(clothesItem.kind, out List<ClothesItem> value))
				{
					value = new List<ClothesItem>();
					clothes.Add(clothesItem.kind, value);
				}
				value.Add(clothesItem);
			}
		}

		private void Start()
		{
			foreach (KeyValuePair<ClothesKind, List<ClothesItem>> clothe in clothes)
			{
				ClothesItem clothesItem = null;
				bool flag = false;
				foreach (ClothesItem item in clothe.Value)
				{
					if (item.gameObject.activeSelf)
					{
						if (clothesItem == null)
						{
							clothesItem = item;
						}
						else
						{
							UnityEngine.Debug.LogErrorFormat("There's already default clothes item of kind '{0}'.", item.kind.ToString());
						}
					}
					if (item.weared)
					{
						if (flag)
						{
							UnityEngine.Debug.LogErrorFormat("There's already weared clothes item of kind '{0}'.", item.kind.ToString());
							item.SetActive(active: false);
						}
						else
						{
							flag = true;
							item.SetActive(active: true, skinRenderer);
						}
					}
					else
					{
						item.SetActive(active: false);
					}
				}
				if (!flag && clothesItem != null)
				{
					clothesItem.weared = true;
					clothesItem.SetActive(active: true, skinRenderer);
				}
			}
			if (this.OnAction != null)
			{
				this.OnAction();
			}
		}
	}
}
