using System;
using _0.DucLib.Scripts.Common;
using BG_Library.NET;
using UnityEngine;

namespace _0.Custom.Scripts.Game
{
    [Serializable]
    public class CustomConfigValue
    {
        public int startGem = 0;
    }
    
    public class CustomConfig : MonoBehaviour
    {
        public static CustomConfigValue CustomConfigValue;
        private void Awake()
        {
            RemoteConfig.OnFetchComplete += FetchComplete;
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            RemoteConfig.OnFetchComplete -= FetchComplete;
        }

        private void FetchComplete()
        {
            CustomConfigValue = JsonUtility.FromJson<CustomConfigValue>(RemoteConfig.Ins.custom_config);
        }
    }
}