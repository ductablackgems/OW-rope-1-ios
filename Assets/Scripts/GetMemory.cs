using App;
using App.SaveSystem;
using UnityEngine;

public class GetMemory : MonoBehaviour
{
	private int memory;

	private SettingsSaveEntity settingsSave;

	private void Awake()
	{
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
		memory = SystemInfo.systemMemorySize;
		settingsSave.memoryRam = memory;
		settingsSave.Save();
		if (memory < 1200 && memory > 1050)
		{
			QualitySettings.globalTextureMipmapLimit = 1;
		}
		else if (memory < 1050 && memory > 900)
		{
			QualitySettings.globalTextureMipmapLimit = 2;
		}
		else if (memory < 900)
		{
			QualitySettings.globalTextureMipmapLimit = 3;
		}
		else if (memory > 1200)
		{
			QualitySettings.globalTextureMipmapLimit = 0;
		}
	}
}
