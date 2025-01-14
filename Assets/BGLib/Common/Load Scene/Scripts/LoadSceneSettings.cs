using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BG_Library.Common
{
    [CreateAssetMenu(fileName = AssetName, menuName = "BG/LoadScene")]
    public class LoadSceneSettings : ScriptableObject
    {
        public const string AssetName = "Load scene settings";

        [Header("Custom")]
        //[SerializeField] string layer;
        [SerializeField] int orderInLayer;

        [SerializeField] AnimLoadSceneBase loadSceneAnimDefault;

        //public string Layer => layer;
        public int OrderInLayer => orderInLayer;
        public AnimLoadSceneBase LoadSceneAnimDefault => loadSceneAnimDefault;

        public void Preload()
		{
            LoadSceneManager.Instance.Preload();
		}

#if UNITY_EDITOR
        [MenuItem("BG/Load scene/Settings")]
        static void SelectSettingLoadSceneConfig()
        {
            Master.CreateAndSelectAssetInResource<LoadSceneSettings>(AssetName);
        }
#endif
    }
}