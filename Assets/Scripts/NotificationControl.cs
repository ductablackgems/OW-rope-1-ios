using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationControl : MonoBehaviour
{
    [SerializeField]
    string[] txt8h, txt12h, txt20h;

    void Start()
    {
        MyNotification.CancelAllDisplayNotification();
        MyNotification.CancelAllScheduleNotification();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            MyNotification.CancelAllDisplayNotification();
            MyNotification.CancelAllScheduleNotification();
        }
        else{
            PushNotification();
        }
    }

    void PushNotification()
    {
        MyNotification.CancelAllDisplayNotification();
        MyNotification.CancelAllScheduleNotification();

        System.DateTime d = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 8, 0, 0);

        if (System.DateTime.Now.Hour < 9)
        {
            MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(0.167f).Subtract(System.DateTime.Now).TotalSeconds, true, "d0");
            MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(0.5f).Subtract(System.DateTime.Now).TotalSeconds, true, "d0-1");
        }
        else
        {
            if (System.DateTime.Now.Hour < 15)
            {
                MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(0.5f).Subtract(System.DateTime.Now).TotalSeconds, true, "d0-1");
            }
        }

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(1).Subtract(System.DateTime.Now).TotalSeconds, true, "d1");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(1.167f).Subtract(System.DateTime.Now).TotalSeconds, true, "d1-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(1.5f).Subtract(System.DateTime.Now).TotalSeconds, true, "d1-2");
        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(2).Subtract(System.DateTime.Now).TotalSeconds, true, "d2");
        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(3).Subtract(System.DateTime.Now).TotalSeconds, true, "d3");
        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(5).Subtract(System.DateTime.Now).TotalSeconds, true, "d5");
        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(7).Subtract(System.DateTime.Now).TotalSeconds, true, "d7");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(7.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d7-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(7.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d7-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(14).Subtract(System.DateTime.Now).TotalSeconds, true, "d14");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(14.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d14-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(14.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d14-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(21).Subtract(System.DateTime.Now).TotalSeconds, true, "d21");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(21.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d21-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(21.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d21-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(30).Subtract(System.DateTime.Now).TotalSeconds, true, "d30");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(30.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d30-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(30.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d30-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(40).Subtract(System.DateTime.Now).TotalSeconds, true, "d40");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(40.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d40-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(40.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d40-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(50).Subtract(System.DateTime.Now).TotalSeconds, true, "d50");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(50.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d50-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(50.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d50-2");

        MyNotification.SendPush(Application.productName, txt8h[UnityEngine.Random.Range(0, txt8h.Length)], (int)d.AddDays(60).Subtract(System.DateTime.Now).TotalSeconds, true, "d60");
        MyNotification.SendPush(Application.productName, txt12h[UnityEngine.Random.Range(0, txt12h.Length)], (int)d.AddDays(60.167).Subtract(System.DateTime.Now).TotalSeconds, true, "d60-1");
        MyNotification.SendPush(Application.productName, txt20h[UnityEngine.Random.Range(0, txt20h.Length)], (int)d.AddDays(60.5).Subtract(System.DateTime.Now).TotalSeconds, true, "d60-2");
    }
}
