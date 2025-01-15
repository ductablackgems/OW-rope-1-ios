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
        
        [ReadOnly]public OW_NPCEntity currentNpcEntity;
        protected override void Init()
        {
            base.Init();
            OW_QuestManager.Instance.StartQuest();
        }


        public void StartTalkPanel()
        {
            OW_UIController.instance.ActivePanel(OW_UIController.PanelType.TalkPanel);
            playerController.playerMovement.StopMovement();
        }
        
    }
}