using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.OWQuest
{
    [System.Serializable]
    public class OW_Enemy
    {
        [LabelText("Tên quái vật")] public string name; 

        [LabelText("Máu của quái vật")] [MinValue(1)]
        public int health; 

        [PreviewField] [LabelText("Hình đại diện quái vật")]
        public Sprite icon;
    }
}