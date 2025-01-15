using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_Reward
    {
        [LabelText("Điểm kinh nghiệm")]
        [MinValue(0)]
        public int experiencePoints; // Điểm kinh nghiệm nhận được

        [LabelText("Vàng")]
        [MinValue(0)]
        public int gold; // Vàng nhận được

        [LabelText("Vật phẩm đặc biệt")]
        public List<OW_QuestItem> specialItem; // Vật phẩm đặc biệt (nếu có)
    }
}