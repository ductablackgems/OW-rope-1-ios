using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_QuestObjective : OW_QuestDetail
    {
        [EnumToggleButtons]
        public OW_ObjectiveType objectiveType; 
        
        [ShowIf("objectiveType", OW_ObjectiveType.TalkToNPC)]
        public OW_NPC targetNPC; 

        [ShowIf("objectiveType", OW_ObjectiveType.KillEnemies)]
        public OW_Enemy targetEnemy; 

        [ShowIf("objectiveType", OW_ObjectiveType.KillEnemies)]
        [MinValue(1)]
        public int killCount = 1; 

        [ShowIf("objectiveType", OW_ObjectiveType.CollectItems)]
        public OW_QuestItem targetQuestItem; 

        [ShowIf("objectiveType", OW_ObjectiveType.CollectItems)]
        [MinValue(1)]
        public int itemCount;
    }
}