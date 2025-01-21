using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_QuestItem
    {
        public string itemName;

        [MinValue(0)]
        public int itemValue;
    }
}