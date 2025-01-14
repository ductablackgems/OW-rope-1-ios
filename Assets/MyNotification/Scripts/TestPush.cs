using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPush : MonoBehaviour {
    public UnityEngine.UI.Text textCallBack;


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame

    public void OnPushClick() {
        MyNotification.SendPush("This is tittle", "Day la mo ta cua noti", 5, false, "normal noti");
    }

    public void OnPushSpecialClick()
    {
        MyNotification.SendPush("This is tittle1", "Day la mo ta cua noti1", 5, true, "special noti");
    }

    public void OnCalcelDisplay() {
        MyNotification.CancelAllDisplayNotification();
    }

    public void OnCancelPlanNoti() {
        MyNotification.CancelAllScheduleNotification();
    }

    public void OnGetCallBack() {
        string tmp = MyNotification.GetNotificationCallback();
        textCallBack.text = "Call back: " + tmp;
    }
}
