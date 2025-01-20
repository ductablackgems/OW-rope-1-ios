using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_QuestItem
    {
        [LabelText("Tên vật phẩm")]
        public string itemName;

        [LabelText("Giá trị vật phẩm")]
        [MinValue(0)]
        public int itemValue;
    }
}