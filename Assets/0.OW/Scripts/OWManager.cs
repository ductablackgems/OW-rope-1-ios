using System;
using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.Data;
using _0.OW.Scripts.Item;
using _0.OW.Scripts.Minimap;
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
        public OW_MiniMapController miniMapController;

        [ReadOnly] public OW_NPCEntity currentNpcEntity;
        [ReadOnly] public OW_Quest currentQuest;

        [ReadOnly] public OW_QuestItemEntity currentQuestItem;
        public List<GameObject> collectItemGroup;
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
            LogHelper.LogYellow($"[QUEST] Collection count : {QuestData.QuestCollectionCount}");
        }

        public void GetQuest()
        {
            currentQuest = OW_QuestManager.Instance.ActiveCurrentQuest();
        }

        public void StartTalkPanel()
        {
            if (currentNpcEntity == null) return;
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
            npc.npcQuestData = npcQuestData;
            npc.questType = type;
            npc.noti.SetActive(type != OW_ProgressType.QuestReceiver);
            npc.notiComplete.SetActive(type == OW_ProgressType.QuestReceiver);
        }

        public void ActiveNPC(OW_Quest npcQuestData, OW_ProgressType type, string nameName, Action<Vector3> callback)
        {
            var npc = npcEntities.Find(x => x.npcData.npcName == nameName);
            npc.npcQuestData = npcQuestData;
            npc.questType = type;
            npc.noti.SetActive(type != OW_ProgressType.QuestReceiver);
            npc.notiComplete.SetActive(type == OW_ProgressType.QuestReceiver);
            callback?.Invoke(npc.transform.position);
        }

        public void ShowQuestGroup(string questName)
        {
            var group = collectItemGroup.Find(x => x.name == questName);
            group.SetActive(true);
            List<Transform> items = new List<Transform>();
            foreach (Transform item in group.transform)
            {
                items.Add(item);
            }

            miniMapController.ShowCollectZone(items);
        }

        public void HideAllQuestGroups()
        {
            foreach (var item in collectItemGroup)
            {
                item.HideObject();
            }
        }
        public void CollectItem()
        {
            currentQuestItem.HideObject();
            QuestData.QuestCollectionCount += 1;
            OW_UIController.instance.takeButton.HideObject();
            if (QuestData.QuestCollectionCount >= currentQuest.objective[QuestData.QuestCollection].itemCount)
            {
                // next quest
                OW_QuestManager.Instance.CompleteProgress();
                LogHelper.LogYellow("Complete Quest");
                return;
            }
            var des = currentQuest.objective[QuestData.QuestCollection].questDescription;
            var des2 = des.Replace("{QuestData.QuestCollectionCount}", QuestData.QuestCollectionCount.ToString());
            LogHelper.LogPurple(des2);
        }
    }
}