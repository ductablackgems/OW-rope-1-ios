using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_QuestObjective : OW_QuestDetail
    {
        [EnumToggleButtons]
        [LabelText("Loại mục tiêu")]
        public OW_ObjectiveType objectiveType; 
        
        [ShowIf("objectiveType", OW_ObjectiveType.TalkToNPC)]
        [LabelText("NPC cần nói chuyện")]
        public OW_NPC targetNPC; 

        [ShowIf("objectiveType", OW_ObjectiveType.KillEnemies)]
        [LabelText("Quái cần tiêu diệt")]
        public OW_Enemy targetEnemy; 

        [ShowIf("objectiveType", OW_ObjectiveType.KillEnemies)]
        [LabelText("Số lượng quái cần tiêu diệt")]
        [MinValue(1)]
        public int killCount = 1; 

        [FormerlySerializedAs("targetItemMission")]
        [FormerlySerializedAs("targetItem")]
        [ShowIf("objectiveType", OW_ObjectiveType.CollectItems)]
        [LabelText("Vật phẩm cần tìm")]
        public OW_QuestItem targetQuestItem; 

        [ShowIf("objectiveType", OW_ObjectiveType.CollectItems)]
        [LabelText("Số lượng vật phẩm cần thu thập")]
        [MinValue(1)]
        public int itemCount;
    }
}