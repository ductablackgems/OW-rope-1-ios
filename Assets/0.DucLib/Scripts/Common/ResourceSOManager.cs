using BG_Library.Common;
using UnityEngine;


    public class ResourceSOManager<T> : ScriptableObject where T : ScriptableObject
    {
        static T ins;
        
        public static T Instance
        {
            get
            {
                if (ins == null)
                {
                    Setup();
                }

                return ins;
            }
        }
        static void Setup()
        {
            ins = LoadSource.LoadObject<T>($"{typeof(T).Name}");
        }
    }
