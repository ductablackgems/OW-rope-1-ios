using _0.OW.Scripts.OWQuest;
using UnityEngine;

namespace _0.OW.Scripts.Data
{
    public static class QuestData
    {
        public static void StartNewQuestion()
        {
            QuestCollection = 0;
            QuestIndex += 1;
            QuestProgressType = OW_ProgressType.QuestGiver;
        }
        public static int QuestIndex
        {
            get => PlayerPrefs.GetInt("QuestIndex", 0);
            set => PlayerPrefs.SetInt("QuestIndex", value);
        }


        public static int QuestCollection
        {
            get => PlayerPrefs.GetInt("QuestCollection", 0);
            set => PlayerPrefs.SetInt("QuestCollection", value);
        }

        public static OW_ProgressType QuestProgressType
        {
            get => (OW_ProgressType)PlayerPrefs.GetInt("QuestProgressType", 0);
            set => PlayerPrefs.SetInt("QuestProgressType", (int)value);
        }
    }
}