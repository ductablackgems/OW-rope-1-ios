using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.Data;
using _0.OW.Scripts.OWQuest;
using _0.OW.Scripts.OWQuest.Entity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _0.OW.Scripts.UI
{
    public class OW_TalkPanel : MonoBehaviour
    {
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtQuestName;
        public TextMeshProUGUI content;
        public TextMeshProUGUI txtNext;

        public GameObject buttonNext;
        public GameObject buttonExit;
        public GameObject buttonQuest;

        public GameObject reward;


        [ReadOnly] public int currentIndex;
        [ReadOnly] public OW_NPCEntity currentNPC;
        [ReadOnly] public OW_NPC currentNpcQuestData;

        public void SetUp(OW_NPCEntity currentNPC)
        {
            this.currentNPC = currentNPC;
            txtName.text = currentNPC.npcData.npcName;
            content.text = currentNPC.npcData.defaultTalk;
            buttonNext.SetActive(false);
            buttonExit.SetActive(true);
            reward.SetActive(false);
            if (currentNPC.npcQuestData != null)
            {
                buttonQuest.SetActive(true);
                txtQuestName.text = $"{this.currentNPC.npcQuestData.questName}";
            }
            else
            {
                buttonQuest.SetActive(false);
            }

            currentNpcQuestData = null;
        }

        public void ChoiceQuest()
        {
            currentIndex = 0;
            txtNext.text = "NEXT";
            buttonQuest.SetActive(false);
            buttonNext.SetActive(true);
            GetCurrentNpcData();
            content.text = currentNpcQuestData.dialogues[currentIndex];
        }

        private void GetCurrentNpcData()
        {
            switch (currentNPC.questType)
            {
                case OW_ProgressType.QuestGiver:
                    currentNpcQuestData = currentNPC.npcQuestData.questGiver;
                    break;
                case OW_ProgressType.InProgress:
                    break;
                case OW_ProgressType.QuestReceiver:
                    currentNpcQuestData = currentNPC.npcQuestData.questReceiver;
                    break;
            }

        }

        public void Exit()
        {
            currentNPC = null;
            currentNpcQuestData = null;
            OWManager.instance.HideTalkPanel();
        }

        public void Next()
        {
            currentIndex += 1;
            if (currentIndex == currentNpcQuestData.dialogues.Count)
            {
                var ttt = currentNPC.questType == OW_ProgressType.QuestReceiver ? "COMPLETE" : "CONFIRM";
                content.text = "hehe";
                reward.SetActive(true);
                txtNext.text = ttt;
            }
            else if (currentIndex > currentNpcQuestData.dialogues.Count)
            {
                TalkComplete();
                Exit();
            }
            else
            {
                content.text = currentNpcQuestData.dialogues[currentIndex];
            }
        }

        private void TalkComplete()
        {
            switch (currentNPC.questType)
            {
                case OW_ProgressType.QuestGiver:
                    QuestData.QuestProgressType = OW_ProgressType.InProgress;
                    RemoveNPC();
                    // sang buoc tiep theo
                    break;
                case OW_ProgressType.InProgress:
                    // sang buoc tiep theo
                    break;
                case OW_ProgressType.QuestReceiver:
                    QuestData.StartNewQuestion();
                    RemoveNPC();
                    break;
            }

            void RemoveNPC()
            {
                currentNPC.npcQuestData = null;
                currentNPC.noti.SetActive(false);
                // currentNPC.notiComplete.SetActive(false);
            }
        }
    }
}