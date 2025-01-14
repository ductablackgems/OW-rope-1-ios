using System;
using BG_Library.Common;
using UnityEngine;


    public class ResourceSingleton <T> : MonoBehaviour where T : MonoBehaviour
    {
        private static string AssetName = $"{typeof(T).Name}";
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    SetUp();
                }
                return _instance;
            }
        }
        private static void SetUp()
        {
            var prefabs = LoadSource.LoadObject<T>(AssetName);
            _instance = Instantiate(prefabs, DDOL.Instance.transform);
        }

        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            
        }
    }
