using System;
using _0.DucLib.Scripts.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest.Entity
{
    public class OW_NPCEntity : MonoBehaviour
    {
        [LabelText("Dữ liệu NPC")]
        [ReadOnly]public OW_NPC npcData; 

        [LabelText("Hiện tại đang nói chuyện?")]
        [ReadOnly]
        public bool isInteracting = false;

        public string npcName;
        private void Start()
        {
           
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                OWManager.instance.dialogButton.ShowObject();
                LogHelper.LogYellow("Show dialog");
            }
        }
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                OWManager.instance.dialogButton.HideObject();
            }
        }

        [Button("Bắt đầu giao tiếp")]
        public void Interact()
        {
            if (npcData == null)
            {
                Debug.LogWarning($"NPC {name} không có dữ liệu liên kết!");
                return;
            }

            isInteracting = true;

            foreach (var dialogue in npcData.dialogues)
            {
                Debug.Log($"NPC {npcData.npcName} nói: {dialogue}");
            }

            isInteracting = false;
        }

        [Button("Cập nhật vị trí NPC")]
        public void UpdateLocationFromData()
        {
            if (npcData != null)
            {
                Debug.Log($"Vị trí của NPC {npcData.npcName} đã được cập nhật theo dữ liệu.");
            }
            else
            {
                Debug.LogWarning("Chưa gán dữ liệu NPC!");
            }
        }

        [Button("Lưu vị trí vào dữ liệu")]
        public void SaveLocationToData()
        {
            if (npcData != null)
            {
                Debug.Log($"Vị trí của NPC {npcData.npcName} đã được lưu vào dữ liệu.");
            }
            else
            {
                Debug.LogWarning("Chưa gán dữ liệu NPC!");
            }
        }
    }
}