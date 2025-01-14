using System;
using System.Collections;
using System.Collections.Generic;
using BG_Library.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace  BG_Library.NET
{
    /// <summary>
    /// Gan script o button xem Reward, sau do custom tuy truong hop
    /// </summary>
    public class CheckReadyRewardBtn : MonoBehaviour
    {
        [BoxGroup("Config")] public UnityEvent onReadyAds;
        [BoxGroup("Config")] public UnityEvent onNotReadyAds;
        [BoxGroup("Config")] public float refreshTime = 0.5f;

        [SerializeField] private bool debuggable;

        private Coroutine _checkNotReadyCorou;
        private Coroutine _checkReadyCorou;

        private void OnEnable()
        {
            if (AdsManager.IsRewardedReady())
            {
                DoWhenReadyAds();
            }
            else
            {
                DoWhenNotReadyAds();
            }
        }

        private void OnDisable()
        {
            if (_checkNotReadyCorou != null)
            {
                StopCoroutine(_checkNotReadyCorou);
            }

            if (_checkReadyCorou != null)
            {
                StopCoroutine(_checkReadyCorou);
            }
        }
        
        private void DoWhenNotReadyAds()
        {
            if(debuggable) Debug.Log("On not ready RW");
            onNotReadyAds?.Invoke();

            if (_checkReadyCorou != null)
            {
                StopCoroutine(_checkReadyCorou);
            }

            _checkReadyCorou = StartCoroutine(CheckReadyCorou());
        }
        
        private IEnumerator CheckReadyCorou()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(refreshTime);
                if (AdsManager.IsRewardedReady())
                {
                    DoWhenReadyAds();
                    break;
                }
                
                yield return null;
            }
        }

        private void DoWhenReadyAds()
        {
            if(debuggable) Debug.Log("On ready RW");
            onReadyAds?.Invoke();

            if (_checkNotReadyCorou != null)
            {
                StopCoroutine(_checkNotReadyCorou);
            }

            _checkNotReadyCorou = StartCoroutine(CheckNotReadyCorou());
        }
        
        private IEnumerator CheckNotReadyCorou()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(refreshTime);
                if (!AdsManager.IsRewardedReady())
                {
                    DoWhenNotReadyAds();
                    break;
                }
                
                yield return null;
            }
        }

        
    }
}