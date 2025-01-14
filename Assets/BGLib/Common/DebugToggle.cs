using UnityEngine;

namespace BG_Library.Common
{
    public class DebugToggle : MonoBehaviour
    {
        private void Awake()
        {
            bool isDebug = Debug.isDebugBuild;
#if !UNITY_EDITOR
        if(!isDebug) Destroy(gameObject);
#endif
        }
    }
}