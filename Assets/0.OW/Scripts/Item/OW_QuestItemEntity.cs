using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.Data;
using _0.OW.Scripts.OWQuest;
using UnityEngine;

namespace _0.OW.Scripts.Item
{
    public class OW_QuestItemEntity : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                CheckQuest();
            }
        }

        public void CheckQuest()
        {
            if(QuestData.QuestProgressType != OW_ProgressType.InProgress) return;
            var obt = OWManager.instance.currentQuest.objective[QuestData.QuestCollection];
            if(obt.objectiveType != OW_ObjectiveType.CollectItems) return;
            if(obt.targetQuestItem.itemName != gameObject.name) return;
            OWManager.instance.currentQuestItem = this;
            OW_UIController.instance.takeButton.ShowObject();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                OW_UIController.instance.takeButton.HideObject();
               // if(OWManager.instance.currentNpcEntity == this) OWManager.instance.currentNpcEntity = null;
            }
        }

    }
}