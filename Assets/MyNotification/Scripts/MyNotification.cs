using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNotification
{
    private const string MainActivity = "com.unity3d.player.UnityPlayerActivity";
    private const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";
    private const string PluginActivityClassName = "com.example.abc.pushnoty2.PushUtil";
    private static AndroidJavaObject activity;
    private static AndroidJavaClass pluginActivity;
    // Use this for initialization
    public static void SendPush(string title, string des, int timeDelay, bool isSpecial, string callBack = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        int id = Random.Range(0, int.MaxValue);
        NotificationData data = NotificationData.GetInstance();
        data.notificationID.Add(id);
        data.SaveInstance();

        if (activity == null)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(UnityActivityClassName);
            activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            pluginActivity = new AndroidJavaClass(PluginActivityClassName);

        }

        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        if (!isSpecial) pluginActivity.CallStatic("start", context,MainActivity, id, title, des, timeDelay, callBack);
        else pluginActivity.CallStatic("startSpecial", context,MainActivity, id, title, des,  timeDelay, callBack);
#elif UNITY_IOS
        UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();

        notification.fireDate = System.DateTime.Now.AddSeconds(timeDelay);
        notification.alertTitle = Application.productName;
        notification.alertBody = des;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification); 
#endif
    }

    public static void CancelAllDisplayNotification()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (activity == null)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(UnityActivityClassName);
            activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            pluginActivity = new AndroidJavaClass(PluginActivityClassName);

        }

        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        pluginActivity.CallStatic("clearAllNotification", context);
#elif UNITY_IOS
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
    }

    public static void CancelAllScheduleNotification()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        NotificationData data = NotificationData.GetInstance();

        if (activity == null)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(UnityActivityClassName);
            activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            pluginActivity = new AndroidJavaClass(PluginActivityClassName);

        }

        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        Debug.Log("Cancel Noti - Count " + data.notificationID.Count);
        for (int i = 0; i < data.notificationID.Count; i++)
        {
            pluginActivity.CallStatic("CancelScheduledNotification", activity, data.notificationID[i]);
            Debug.Log("Cancel Noti - " + data.notificationID[i]);
        }

        data.notificationID.Clear();
        data.SaveInstance();
#elif UNITY_IOS
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif

    }

    public static string GetNotificationCallback()
        {
            string callBack = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR

        var currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        var hasExtra = intent.Call<bool>("hasExtra", "call_back");

        if (hasExtra)
        {
            callBack = intent.Call<string>("getStringExtra", "call_back");
            intent.Call("removeExtra", "call_back");
        }

#endif
            return callBack;
        }
    }
