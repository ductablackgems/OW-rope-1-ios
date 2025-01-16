using Sirenix.OdinInspector;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public enum OW_ObjectiveType
    {
       TalkToNPC,
       KillEnemies,
       CollectItems
    }
    
    [System.Serializable]
    public enum OW_ProgressType
    {
        QuestGiver =0,
        InProgress = 1,
         QuestReceiver =2
    }
}