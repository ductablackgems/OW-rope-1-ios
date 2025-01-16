using Sirenix.OdinInspector;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public enum OW_ObjectiveType
    {
        [LabelText("Nói chuyện với NPC")] TalkToNPC,
        [LabelText("Tiêu diệt quái vật")] KillEnemies,
        [LabelText("Thu thập vật phẩm")] CollectItems
    }
    
    [System.Serializable]
    public enum OW_ProgressType
    {
        [LabelText("Nhận nhiệm vụ")] QuestGiver =0,
        [LabelText("Thực hiện nhiệm vụ")] InProgress = 1,
        [LabelText("Trả nhiệm vụ")] QuestReceiver =2
    }
}