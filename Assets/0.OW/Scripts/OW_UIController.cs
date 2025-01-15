using _0.DucLib.Scripts.Common;
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
        public GameObject talkPanel;
        public GameObject joystick;

        public void OnClickDialogButton()
        {
            OWManager.instance.StartTalkPanel();
        }

        public void ActivePanel(PanelType type)
        {
            dialogButton.ActiveObject(type == PanelType.Gameplay && OWManager.instance.currentNpcEntity);
            joystick.SetActive(type == PanelType.Gameplay);
            talkPanel.SetActive(type == PanelType.TalkPanel);
            
        }
    }
}