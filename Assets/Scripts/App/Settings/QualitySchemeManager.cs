using App.SaveSystem;
using UnityEngine;

namespace App.Settings
{
	public class QualitySchemeManager : MonoBehaviour
	{
		[SerializeField]
		private QualitySchemes schemes;

		private QualityScheme scheme;

		private SettingsSaveEntity settingsSave;

		private bool initialized;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			}
		}

		public QualityScheme GetScheme()
		{
			Init();
			if (scheme == null)
			{
				float num = Mathf.Clamp(settingsSave.graphicQuality, 0f, 3f);
				scheme = schemes.schemes[(int)num];
			}
			return scheme;
		}
	}
}
