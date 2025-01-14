using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BG_Library.Common;

#if UNITY_ANDROID
using BG_Library.NET;
using Google.Play.Review;
#endif

namespace BG_Library.IAR
{
    public static class InAppReviewManager
    {
        public static void ShowReview()
        {
#if UNITY_ANDROID
            AppReviewAndroid.ShowReview();
#elif UNITY_IPHONE
            UnityEngine.iOS.Device.RequestStoreReview();
#endif
        }
    }

#if UNITY_ANDROID
    public static class AppReviewAndroid
    {
        // Create instance of ReviewManager
        private static ReviewManager _reviewManager;
        private static PlayReviewInfo _playReviewInfo;

        public static void ShowReview()
        {
            AdsManager.OpenAdAction = EOAAction.RATE;
            DDOL.Instance.StartCoroutine(RequestReviewPlayStore());
        }

        private static IEnumerator RequestReviewPlayStore()
        {
            _reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.LogError(requestFlowOperation.Error.ToString());
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.LogError(requestFlowOperation.Error.ToString());
                yield break;
            }
            Debug.Log("Complete review!");
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
        }
    }
#endif
}