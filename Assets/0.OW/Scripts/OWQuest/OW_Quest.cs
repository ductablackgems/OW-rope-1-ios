using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest", order = 1)]
    public class OW_Quest : ScriptableObject
    {
        public string questName;

        [FoldoutGroup("Quest Details"), LabelText("NPC Giao nhiệm vụ")]
        public OW_NPC questGiver;


        [FoldoutGroup("Quest Details"), LabelText("Nhiệm vụ")]
        [ListDrawerSettings(Expanded = false, ShowIndexLabels = true)]
        public List<OW_QuestObjective> objective;


        [FoldoutGroup("Quest Details"), LabelText("NPC Trả nhiệm vụ")]
        public OW_NPC questReceiver;


        [BoxGroup("Reward Settings")] public OW_Reward reward;
    }
}