using System;
using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.Data;
using _0.OW.Scripts.OWQuest;
using _0.OW.Scripts.OWQuest.Entity;
using _0.OW.Scripts.Player;
using App.Quests;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts
{
    public class OWManager : SingletonMono<OWManager>
    {
        public OW_PlayerController playerController;
        public List<OW_NPCEntity> npcEntities = new List<OW_NPCEntity>();


        [ReadOnly] public OW_NPCEntity currentNpcEntity;
        [ReadOnly] public OW_Quest currentQuest;

        public bool playerStop = false;
        protected override void Init()
        {
            base.Init();
            GetQuest();
        }

        private void Update()
        {
            LogHelper.LogYellow($"[QUEST] Index : {QuestData.QuestIndex}");
            LogHelper.LogYellow($"[QUEST] Progress : {QuestData.QuestProgressType}");
            LogHelper.LogYellow($"[QUEST] Collection : {QuestData.QuestCollection}");
        }

        public void GetQuest()
        {
            currentQuest = OW_QuestManager.Instance.ActiveCurrentQuest();
        }
        public void StartTalkPanel()
        {
            if(currentNpcEntity == null) return;
            OW_UIController.instance.ActiveTalkPanel(currentNpcEntity);
            playerController.playerMovement.StopMovement();
            playerStop = true;
        }
        public void HideTalkPanel()
        {
            OW_UIController.instance.HideTalkPanel();
            playerController.playerMovement.ResumeMovement();
            playerStop = false;
        }

        public void ActiveNPC(OW_Quest npcQuestData, OW_ProgressType type, string nameName)
        {
            var npc = npcEntities.Find(x => x.npcData.npcName == nameName);
            LogHelper.LogYellow($"[QUEST] Current NPC: {nameName}");
            npc.npcQuestData = npcQuestData;
            npc.questType = type;
            npc.noti.SetActive(type != OW_ProgressType.QuestReceiver);
            npc.notiComplete.SetActive(type == OW_ProgressType.QuestReceiver);
        }
    }
}