using System;
using System.Collections;
using _0.DucLib.Scripts.Common;
using BG_Library.NET;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _0.Custom.Scripts.Game
{
    public class SplashScene : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(WaitToLoadScene());
        }
        IEnumerator WaitToLoadScene()
        {
            yield return new WaitForSeconds(6);
            yield return new WaitUntil(() => RemoteConfig.Ins.isDataFetched);
            SceneManager.LoadScene(1);
        }

    }
}