using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_NPC
    {
        [LabelText("Tên NPC")] public string npcName; 

        [Title("Danh sách thoại khi giao tiếp với NPC")] [ListDrawerSettings(Expanded = false, ShowIndexLabels = true)]
        public List<string> dialogues; 

        [PreviewField] [LabelText("Chân dung NPC")]
        public Sprite portrait; 
    }
}