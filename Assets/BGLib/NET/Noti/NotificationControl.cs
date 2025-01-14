using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace BG_Library.NET.Noti
{
    public class NotificationControl : MonoBehaviour
    {
        public static NotificationControl Ins { get; private set; }

        [SerializeField]
        private string[] txt8h, txt12h, txt20h;

        private void Awake()
        {
            if (Ins == null)
            {
                Ins = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            MyNotification.CreateChannel();
            MyNotification.CancelAll();
            Invoke(nameof(PushNotification), 2);
        }

        void PushNotification()
        {
            System.DateTime d = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month
                , System.DateTime.Now.Day, 8, 0, 0);

            if (System.DateTime.Now.Hour < 9)
            {
                PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(0.167f), "d0");
                PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(0.5f), "d0-1");
            }
            else
            {
                if (System.DateTime.Now.Hour < 15)
                {
                    PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(0.5f), "d0-2");
                }
            }

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(1), "d1");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(1.167f), "d1-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(1.5f), "d1-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(2), "d2");
            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(3), "d3");
            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(5), "d5");
            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(7), "d7");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(7.167), "d7-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(7.5), "d7-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(14), "d14");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(14.167), "d14-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(14.5), "d14-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(21), "d21");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(21.167), "d21-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(21.5), "d21-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(30), "d30");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(30.167), "d30-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(30.5), "d30-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(40), "d40");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(40.167), "d40-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(40.5), "d40-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(50), "d50");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(50.167), "d50-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(50.5), "d50-2");

            PusNoti(txt8h[UnityEngine.Random.Range(0, txt8h.Length)], d.AddDays(60), "d60");
            PusNoti(txt12h[UnityEngine.Random.Range(0, txt12h.Length)], d.AddDays(60.167), "d60-1");
            PusNoti(txt20h[UnityEngine.Random.Range(0, txt20h.Length)], d.AddDays(60.5), "d60-2");
        }

        public void PusNoti(string t, System.DateTime d, string id)
        {
            MyNotification.SendPush(Application.productName, t, d, id);
        }
    }
}
