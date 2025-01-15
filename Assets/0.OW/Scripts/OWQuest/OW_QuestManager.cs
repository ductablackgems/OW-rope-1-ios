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

        public void StartQuest()
        {
            if (QuestData.QuestIndex >= questList.Count) return;
            var quest = questList[QuestData.QuestIndex];
            // check progress type
            CheckProgressType();
        }

        private void CheckProgressType()
        {
            switch (QuestData.QuestProgressType)
            {
                // hiển thị các hướng dẫn liên quan đến nhiệm vụ
                case OW_ProgressType.QuestGiver:
                    LogHelper.LogYellow("Quest Progress: QuestGiver");
                    break;
                case OW_ProgressType.InProgress:
                    LogHelper.LogYellow("Quest Progress: InProgress");
                    break;
                case OW_ProgressType.QuestReceiver:
                    LogHelper.LogYellow("Quest Progress: QuestReceiver");
                    break;
            }
        }

        public void CompleteObjective(OW_Quest quest)
        {
            Debug.Log($"Objective Completed: {quest.questName}");
            // Kiểm tra hoàn thành toàn bộ nhiệm vụ
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
            Debug.Log($"Quest Completed: {quest.questName}");
            // Trao thưởng
            GiveReward(quest.reward);
        }

        private void GiveReward(OW_Reward reward)
        {
            Debug.Log($"Reward: {reward.experiencePoints} XP, {reward.gold} Gold");
            // Thêm logic nhận thưởng
        }
    }
}