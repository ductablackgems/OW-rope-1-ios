using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.Data;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [CreateAssetMenu(fileName = "OW_QuestManager", menuName = "Quest System/QuestManager", order = 1)]
    public class OW_QuestManager : ResourceSOManager<OW_QuestManager>
    {
        public List<OW_Quest> questList = new List<OW_Quest>();

        public OW_Quest GetCurrentQuest()
        {
            if (QuestData.QuestIndex >= questList.Count) return null;
            var currentQuest = questList[QuestData.QuestIndex];
            CheckProgressType(currentQuest);
            return currentQuest;
        }


        private void CheckProgressType(OW_Quest quest)
        {
            switch (QuestData.QuestProgressType)
            {
                case OW_ProgressType.QuestGiver:
                    OWManager.instance.ActiveNPC(quest, OW_ProgressType.QuestGiver, quest.questGiver.npcName);
                    break;
                case OW_ProgressType.InProgress:
                    LogHelper.LogYellow("Quest Progress: InProgress");
                    break;
                case OW_ProgressType.QuestReceiver:
                    OWManager.instance.ActiveNPC(quest, OW_ProgressType.QuestReceiver, quest.questReceiver.npcName);
                    break;
            }
        }

        public void CompleteObjective(OW_Quest quest)
        {
            if (CheckCompletion(quest))
            {
                CompleteQuest(quest);
            }
        }

        private bool CheckCompletion(OW_Quest quest)
        {
            // Kiểm tra nếu các mục tiêu của nhiệm vụ đã hoàn thành
            return true; // Tùy chỉnh logic ở đây
        }

        private void CompleteQuest(OW_Quest quest)
        {
            GiveReward(quest.reward);
        }

        private void GiveReward(OW_Reward reward)
        {
            Debug.Log($"Reward: {reward.experiencePoints} XP, {reward.gold} Gold");
            // Thêm logic nhận thưởng
        }
    }
}