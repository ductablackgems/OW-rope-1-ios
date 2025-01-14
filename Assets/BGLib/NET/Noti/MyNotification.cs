#if UNITY_ANDROID
using Unity.Notifications.Android;
#else
using Unity.Notifications.iOS;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace BG_Library.NET.Noti
{
    public class MyNotification
    {
        private const string channelId = "default_channel";
        private const string channelName = "SetDefault Channel";
        private const string channelDes = "Generic notifications";
        private const string smallIcon = "smallicon";
        private const string largeIcon = "largeicon";

        private bool isInit = false;

        public static void CreateChannel()
        {
#if UNITY_ANDROID
        AndroidNotificationChannel a = new AndroidNotificationChannel(channelId, channelName, channelDes, Importance.Default);
        AndroidNotificationCenter.RegisterNotificationChannel(a);
#else
            NotificationControl.Ins.StartCoroutine(RequestAuthorization());
#endif
        }

#if UNITY_ANDROID
#else
        static IEnumerator RequestAuthorization()
        {
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };

                string res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log(res);
            }
        }
#endif

        // Use this for initialization
        public static void SendPush(string title, string des, DateTime date, string callBack = "")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNotificationCenter.SendNotification(new AndroidNotification()
        {
            Title = title,
            Text = des,
            FireTime = date,
            IntentData = callBack,
            SmallIcon = smallIcon,
            LargeIcon = largeIcon
        }, channelId);
        //NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(timeDelay), title, des, new Color(0, 0.6f, 1), NotificationIcon.Message);
#elif UNITY_IOS
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(date.Day,
                    date.Hour, date.Minute, date.Second),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                // You can specify a custom identifier which can be used to manage the notification later.
                // If you don't provide one, a unique string will be generated automatically.
                Identifier = "_notification_01",
                Title = title,
                Body = des,
                Subtitle = "Subtitle",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }

        public static void CancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
         AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification("_notification_01");
            iOSNotificationCenter.RemoveDeliveredNotification("_notification_01");
#endif
        }

        public static void CancelAllScheduleNotification()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification("_notification_01");
#endif
        }

        public static string GetNotificationCallback()
        {
            string callBack = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR

#endif
            return callBack;
        }
    }
}
