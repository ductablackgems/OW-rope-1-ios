using _0.DucLib.Scripts.Common;
using _0.OW.Scripts.OWQuest.Entity;
using _0.OW.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _0.OW.Scripts
{
    public class OW_UIController : SingletonMono<OW_UIController>
    {
        public enum PanelType
        {
            TalkPanel, Gameplay
        }
        public Button dialogButton;
        public Button takeButton;
        public OW_TalkPanel talkPanel;
        public GameObject joystick;

        public void OnClickDialogButton()
        {
            OWManager.instance.StartTalkPanel();
        }

        public void ActiveTalkPanel(OW_NPCEntity npc)
        {
            dialogButton.ActiveObject(false);
            joystick.SetActive(false);
            talkPanel.ActiveObject(true);
            talkPanel.SetUp(npc);
        }

        public void HideTalkPanel()
        {
            joystick.SetActive(true);
            dialogButton.ActiveObject(true);
            talkPanel.ActiveObject(false);
        }
    }
}