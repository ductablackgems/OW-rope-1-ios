using UnityEngine;

namespace _0.OW.Scripts.OWQuest.Entity
{
    [CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPCData", order = 1)]
    public class OW_NPCData : ScriptableObject
    {
        public string npcName;
        public string defaultTalk;
        public string job;
    }
}