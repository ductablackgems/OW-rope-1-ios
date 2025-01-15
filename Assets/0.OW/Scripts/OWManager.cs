using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.OWQuest;
using _0.OW.Scripts.OWQuest.Entity;
using App.Quests;
using UnityEngine;

namespace _0.OW.Scripts
{
    public class OWManager : SingletonMono<OWManager>
    {
        public List<OW_NPCEntity> npcEntities = new List<OW_NPCEntity>();

        public GameObject dialogButton;
        protected override void Init()
        {
            base.Init();
            OW_QuestManager.Instance.StartQuest();
        }


        public void ShowQuest()
        {
            Time.timeScale = 0.0001f;
        }
        
    }
}