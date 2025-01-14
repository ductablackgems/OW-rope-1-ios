using App.SaveSystem;
using UnityEngine;

namespace App.GUI
{
	public class OptionsManager : MonoBehaviour
	{
		public UISlider soundSlider;

		public UISlider musicSlider;

		private AudioSource MusicPlayer;

		private SettingsSaveEntity settingsSave;

		private void Awake()
		{
			settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			if (GameObject.FindGameObjectWithTag("MusicPlayer") != null)
			{
				MusicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
				MusicPlayer.ignoreListenerVolume = true;
				soundSlider.value = settingsSave.soundVolume;
				musicSlider.value = settingsSave.musicVolume;
				AudioListener.volume = soundSlider.value;
				MusicPlayer.volume = musicSlider.value;
			}
		}

		public void SoundVolume()
		{
			AudioListener.volume = soundSlider.value;
			settingsSave.soundVolume = soundSlider.value;
			settingsSave.Save();
		}

		public void MusicVolume()
		{
			if (MusicPlayer != null)
			{
				MusicPlayer.volume = musicSlider.value;
			}
			settingsSave.musicVolume = musicSlider.value;
			settingsSave.Save();
		}
	}
}
