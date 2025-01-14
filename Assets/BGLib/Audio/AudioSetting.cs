using BG_Library.Common;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace BG_Library.Audio
{
	[CreateAssetMenu(fileName = AssetName, menuName = "BG/Audio/settings")]
	public class AudioSetting : ScriptableObject
	{
        public const string AssetName = "Audio setting";

		[InfoBox("Chinh Volume EXPOSED => V_Name")]

		[BoxGroup("CONFIGS"), SerializeField] AudioType[] listAudioTypes;
		public AudioType[] ListAudioTypes => listAudioTypes;
		
		public void Init()
		{
			for (int i = 0; i < listAudioTypes.Length; i++)
			{
				listAudioTypes[i].ListCurrentAud.Clear();
			}
		}

		public AudioType GetAudioType(string typeName)
		{
			return System.Array.Find(listAudioTypes, l => l.Name == typeName);
		}

        [System.Serializable]
        public class AudioType
        {
            [SerializeField] string name;
			[SerializeField] bool isDestroyOnLoad;
			[SerializeField] AudioMixer audioMixer;
			[SerializeField] AudioMixerGroup audioMixerGroup;

			[Space(5)]
			[SerializeField, ReadOnly] List<Audio> listCurrentAud = new List<Audio>();

			public string Name => name;
			public bool IsDestroyOnLoad => isDestroyOnLoad;
			public AudioMixer AudioMixer => audioMixer;
			public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

			public List<Audio> ListCurrentAud => listCurrentAud;
		}

#if UNITY_EDITOR
        [MenuItem("BG/Audio/Settings")]
        static void SelectSettingAudioConfig()
        {
            Master.CreateAndSelectAssetInResource<AudioSetting>(AssetName);
        }
#endif
    }
}