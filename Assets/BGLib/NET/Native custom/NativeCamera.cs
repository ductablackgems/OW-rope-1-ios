using UnityEngine;

namespace BG_Library.NET.Native_custom
{
    [RequireComponent(typeof(Camera))]
    public class NativeCamera : MonoBehaviour
    {
        public static Camera Cam { get; private set; }
        public static bool IsCamReady { get; private set; }

        private void Awake()
        {
            if (Cam != null)
            {
                return;
            }

            IsCamReady = true;
            Cam = GetComponent<Camera>();
        }

        private void OnDestroy()
        {
            Cam = null;
            IsCamReady = false;
        }
    }
}