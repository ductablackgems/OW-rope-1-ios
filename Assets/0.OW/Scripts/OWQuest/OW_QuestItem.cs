using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_QuestItem
    {
        [LabelText("Tên vật phẩm")]
        public string itemName;

        [PreviewField]
        [LabelText("Hình đại diện vật phẩm")]
        public Sprite icon;

        [LabelText("Giá trị vật phẩm")]
        [MinValue(0)]
        public int itemValue;
    }
}