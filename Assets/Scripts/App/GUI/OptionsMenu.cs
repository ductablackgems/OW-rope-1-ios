using App.SaveSystem;
using UnityEngine;

namespace App.GUI
{
    public class OptionsMenu : MonoBehaviour
    {
        private PanelsManager panelsManager;

        public UILabel status;

        private SettingsSaveEntity settingsSave;

        private void Start()
        {
            settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
            panelsManager = ServiceLocator.Get<PanelsManager>();
            Screen.sleepTimeout = -1;
            if (!settingsSave.firstLaunch)
            {
                panelsManager.defaultPanelType = PanelType.Options;
                settingsSave.firstLaunch = true;
                settingsSave.Save();
            }
            if (settingsSave.memoryRam < 1200f)
            {
                status.text = LocalizationManager.Instance.GetText(1001);
            }
            else
            {
                status.text = LocalizationManager.Instance.GetText(1009);
            }
            if (settingsSave.graphicQuality == 1f)
            {
                QualitySettings.lodBias = 1.5f;
            }
            else if (settingsSave.graphicQuality == 2f)
            {
                QualitySettings.lodBias = 2f;
            }
            else if (settingsSave.graphicQuality == 0f)
            {
                QualitySettings.lodBias = 1f;
            }
            else if (settingsSave.graphicQuality == 3f)
            {
                QualitySettings.lodBias = 2.5f;
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.antiAliasing = 2;
            }
        }

        public void Options(GameObject name)
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.weapon_buy, () =>
            {
                if (name.name == "medium")
                {
                    settingsSave.graphicQuality = 1f;
                    status.text = "Medium Cars/Humans & Deformation vehicles off (MEDIUM)";
                    QualitySettings.lodBias = 1.5f;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    QualitySettings.antiAliasing = 0;
                }
                else if (name.name == "high")
                {
                    settingsSave.graphicQuality = 2f;
                    status.text = "Many Cars/Humans & Deformation vehicles on (HIGH)";
                    QualitySettings.lodBias = 2f;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    QualitySettings.antiAliasing = 0;
                }
                else if (name.name == "veryhigh")
                {
                    settingsSave.graphicQuality = 3f;
                    status.text = "Max Cars/Humans & Shadows on & SSAA (VERY HIGH)";
                    QualitySettings.lodBias = 2.5f;
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.antiAliasing = 2;
                }
                else
                {
                    settingsSave.graphicQuality = 0f;
                    status.text = "Less Cars/Humans & Deformation vehicles off (LOW)";
                    QualitySettings.lodBias = 1f;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    QualitySettings.antiAliasing = 0;
                }
                settingsSave.Save();
            });
        }
    }
}
