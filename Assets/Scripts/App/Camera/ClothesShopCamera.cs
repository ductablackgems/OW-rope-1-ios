using App.GUI;
using App.Player.Clothes;
using App.Player.Clothes.GUI;
using System.Linq;
using UnityEngine;

namespace App.Camera
{
	public class ClothesShopCamera : MonoBehaviour
	{
		private readonly ClothesKind[] HeadClothesKinds = new ClothesKind[4]
		{
			ClothesKind.Hat,
			ClothesKind.Glasses,
			ClothesKind.Mask,
			ClothesKind.Face
		};

		public float speed = 5f;

		public float rotationSpeed = 100f;

		public DressStand dressStand;

		private ClothesPanel clothesPanel;

		private bool hasClothesPanel;

		private bool initialized;

		private void OnEnable()
		{
			Init();
			if (hasClothesPanel && HeadClothesKinds.Contains(clothesPanel.EditedClothesKind))
			{
				base.transform.position = dressStand.headCameraPosition.transform.position;
				//base.transform.rotation = dressStand.headCameraPosition.transform.rotation;
			}
			else
			{
				base.transform.position = dressStand.wholeBodyCameraPosition.transform.position;
				//base.transform.rotation = dressStand.wholeBodyCameraPosition.transform.rotation;
			}
		}

		private void Update()
		{
			Vector3 position;
			Quaternion rotation;
			if (hasClothesPanel && HeadClothesKinds.Contains(clothesPanel.EditedClothesKind))
			{
				position = dressStand.headCameraPosition.transform.position;
				rotation = dressStand.headCameraPosition.transform.rotation;
			}
			else
			{
				position = dressStand.wholeBodyCameraPosition.transform.position;
				rotation = dressStand.wholeBodyCameraPosition.transform.rotation;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, speed * Time.deltaTime);
			//base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				hasClothesPanel = ServiceLocator.Get<PanelsManager>().HasPanel(PanelType.ClothesShop);
				if (hasClothesPanel)
				{
					clothesPanel = ServiceLocator.Get<ClothesPanel>();
				}
			}
		}
	}
}
