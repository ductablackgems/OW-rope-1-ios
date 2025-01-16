using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
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

        protected override void Init()
        {
            base.Init();
            GetQuest();
        }

        public void GetQuest()
        {
            currentQuest = OW_QuestManager.Instance.GetCurrentQuest();
        }
        public void StartTalkPanel()
        {
            if(currentNpcEntity == null) return;
            OW_UIController.instance.ActiveTalkPanel(currentNpcEntity);
            playerController.playerMovement.StopMovement();
        }
        public void HideTalkPanel()
        {
            OW_UIController.instance.HideTalkPanel();
            playerController.playerMovement.ResumeMovement();
        }

        public void ActiveNPC(OW_Quest npcQuestData, OW_ProgressType type, string nameName)
        {
            var npc = npcEntities.Find(x => x.npcData.npcName == nameName);
            npc.npcQuestData = npcQuestData;
            npc.questType = type;
            npc.noti.SetActive(type != OW_ProgressType.QuestReceiver);
        }
    }
}