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

        public OW_Quest ActiveCurrentQuest()
        {
            if (QuestData.QuestIndex >= questList.Count) return null;
            var currentQuest = questList[QuestData.QuestIndex];
            ActiveQuest(currentQuest);
            return currentQuest;
        }


        private void ActiveQuest(OW_Quest quest)
        {
            switch (QuestData.QuestProgressType)
            {
                case OW_ProgressType.QuestGiver:
                    OWManager.instance.ActiveNPC(quest, OW_ProgressType.QuestGiver, quest.questGiver.npcName);
                    break;
                case OW_ProgressType.InProgress:
                    CheckProgress(quest);
                    break;
                case OW_ProgressType.QuestReceiver:
                    OWManager.instance.ActiveNPC(quest, OW_ProgressType.QuestReceiver, quest.questReceiver.npcName);
                    break;
            }
        }

        private void CheckProgress(OW_Quest quest)
        {
            switch (quest.objective[QuestData.QuestCollection].objectiveType)
            {
                case OW_ObjectiveType.TalkToNPC:
                    var currentNpcQuestData = quest.objective[QuestData.QuestCollection].targetNPC.npcName;
                    OWManager.instance.ActiveNPC(quest, OW_ProgressType.InProgress, currentNpcQuestData);
                    break;
                case OW_ObjectiveType.KillEnemies:
                    break;
                case OW_ObjectiveType.CollectItems:
                    break;
            }
        }

        public void CompleteGiver()
        {
            QuestData.QuestProgressType = OW_ProgressType.InProgress;
            ActiveCurrentQuest();
        }

        public void CompleteProgress()
        {
            QuestData.QuestCollection += 1;
            if (QuestData.QuestCollection >= questList[QuestData.QuestIndex].objective.Count)
            {
                QuestData.QuestCollection = 0;
                QuestData.QuestProgressType = OW_ProgressType.QuestReceiver;
            }
            ActiveCurrentQuest();
        }

        public void CompleteQuest()
        {
            
        }
    }
}