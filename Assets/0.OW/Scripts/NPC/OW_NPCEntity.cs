using System;
using _0.DucLib.Scripts.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest.Entity
{
    public class OW_NPCEntity : MonoBehaviour
    {
        public OW_NPCData npcData;
        public GameObject noti;
        public GameObject notiComplete;
        [ReadOnly] public OW_Quest npcQuestData;
        [ReadOnly] public OW_ProgressType questType;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                OW_UIController.instance.dialogButton.ShowObject();
                OWManager.instance.currentNpcEntity = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                OW_UIController.instance.dialogButton.HideObject();
                if(OWManager.instance.currentNpcEntity == this) OWManager.instance.currentNpcEntity = null;
            }
        }

    }
}