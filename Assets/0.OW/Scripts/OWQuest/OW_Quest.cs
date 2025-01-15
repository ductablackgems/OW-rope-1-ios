using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest", order = 1)]
    public class OW_Quest : ScriptableObject
    {
        [FoldoutGroup("Quest Info")] public string questName; 
        [FoldoutGroup("Quest Info"),TextArea, LabelText("Mô tả nhiệm vụ")] public string questDescription; 

       
        
        [FoldoutGroup("Quest Details"), LabelText("NPC Giao nhiệm vụ")]public OW_NPC questGiver; 

      
        [FoldoutGroup("Quest Details"),LabelText("Nhiệm vụ")][ListDrawerSettings(Expanded = false, ShowIndexLabels = true)]public List<OW_QuestObjective> objective; 
        
        
        [FoldoutGroup("Quest Details"),LabelText("NPC Trả nhiệm vụ")]public OW_NPC questReceiver; 

        


        [BoxGroup("Reward Settings")]
        [InlineProperty]
        public OW_Reward reward;
    }
}