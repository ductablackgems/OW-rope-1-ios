using System;
using App;
using App.Camera;
using UnityEngine;

namespace _0.Custom.Scripts
{
    public class MenuSceneController : MonoBehaviour
    { private CameraManager cameraManager;
        private void Awake()
        {
            cameraManager = ServiceLocator.Get<CameraManager>();
        }

        private void Start()
        {
            cameraManager.SetMenuCamera();
        }
    }
}