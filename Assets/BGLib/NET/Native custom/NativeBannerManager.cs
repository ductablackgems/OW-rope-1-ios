using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BG_Library.NET.Native_custom
{
    public class NativeBannerManager : NativeManager
    {
        [SerializeField] private Canvas bannerCanvas;

        private NativeUIManager ui;
        private Coroutine _waitCamCorou;

        private void Awake()
        {
            ui = bannerCanvas.GetComponentInChildren<NativeUIManager>(true);
            if (ui != null)
            {
                ui.Deactive();
            }

            isBanner = true;
        }

        public override void Init(AdsManager.NativeInfor infor)
        {
            base.Init(infor);

            if (!isEnableAd)
            {
                return;
            }

            if (ui == null)
            {
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            StartCoroutine(WaitShowAd());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_waitCamCorou != null)
            {
                StopCoroutine(_waitCamCorou);
            }

            _waitCamCorou = StartCoroutine(WaitCam());
        }

        private IEnumerator WaitCam()
        {
            yield return new WaitUntil(() => NativeCamera.IsCamReady);
            bannerCanvas.worldCamera = NativeCamera.Cam;
        }

        private IEnumerator WaitShowAd()
        {
            yield return new WaitUntil(() => NativeCamera.IsCamReady);
            bannerCanvas.worldCamera = NativeCamera.Cam;

            yield return new WaitUntil(() => ableAdsButNotShow);
            ShowNative(ui);
            
            if(debuggable) Debug.Log("SHOW NATIVE BANNER");
        }

        protected override void LoadFailAllNA()
        {
            if (debuggable) Debug.Log("Show Banner => Off Native");
            
            AdsManager.AdsEnqueueCallback(() =>
            {
                ui.Deactive();
                AdsManager.Ins.AdsCoreIns.ShowBanner();
                
                StartCoroutine(WaitShowAd());
            });
        }

        protected override void DisplayNativeOnScreen()
        {
            if (debuggable) Debug.Log("Hide Banner => Show Native");
            
            AdsManager.AdsEnqueueCallback(() =>
            {
                base.DisplayNativeOnScreen();
                AdsManager.Ins.AdsCoreIns.HiddenBanner();
            });
        }
    }
}