using UnityEngine;

namespace App.Player.Clothes.GUI
{
    public class ClothesPanel : MonoBehaviour
    {
        public UILabel clothesKindLabel;

        public GameObject wearedLabel;

        public ClothesKind defaultClothesKind = ClothesKind.Shirt;

        private WearClothesButton wearClothesButton;

        private ClothesManager clothesManager;

        private ClothesKind editedClothesKind;

        [SerializeField]
        private int m_TextID;

        public ClothesKind EditedClothesKind
        {
            get
            {
                return editedClothesKind;
            }
            set
            {
                if (editedClothesKind != value)
                {
                    if (editedClothesKind != 0)
                    {
                        clothesManager.RevertItem(editedClothesKind);
                    }
                    editedClothesKind = value;
                    UpdatePanel();
                }
            }
        }

        private void Awake()
        {
            wearClothesButton = this.GetComponentInChildrenSafe<WearClothesButton>();
            clothesManager = ServiceLocator.Get<ClothesManager>();
            clothesManager.OnAction += OnClothesManagerAction;
        }

        private void OnDestroy()
        {
            clothesManager.OnAction -= OnClothesManagerAction;
        }

        private void OnClothesManagerAction()
        {
            UpdatePanel();
        }

        private void setClothesKindText(int Textid)
        {
            clothesKindLabel.GetComponent<LocalizedTextNGUI>().GetDisplayText(Textid, -1);
        }

        private void UpdatePanel()
        {
            ClothesItem clothesItem = (editedClothesKind == ClothesKind.Unsorted) ? null : clothesManager.GetActiveItem(editedClothesKind);
            ClothesItem x = (editedClothesKind == ClothesKind.Unsorted) ? null : clothesManager.GetWearedItem(editedClothesKind);
            clothesKindLabel.text = editedClothesKind.ToString();
            if (editedClothesKind.ToString() == "Skin")
            {
                clothesKindLabel.text = LocalizationManager.Instance.GetText(107);
            }
            else if (editedClothesKind.ToString() == "Hat")
            {
                clothesKindLabel.text = LocalizationManager.Instance.GetText(108);
            }
            if ((clothesItem != null && clothesItem.weared) || (x == null && clothesItem == null))
            {
                wearClothesButton.gameObject.SetActive(value: false);
                wearedLabel.SetActive(value: true);
            }
            else
            {
                wearClothesButton.SetItem(clothesItem);
                wearClothesButton.gameObject.SetActive(value: true);
                wearedLabel.SetActive(value: false);
            }
            wearClothesButton.SetItem(clothesItem);
            wearedLabel.SetActive((clothesItem != null && clothesItem.weared) || (x == null && clothesItem == null));
        }
    }
}
