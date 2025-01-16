using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_NPC
    {
        public string npcName;

        [ListDrawerSettings(Expanded = false, ShowIndexLabels = true)]
        public List<string> dialogues;
    }
}