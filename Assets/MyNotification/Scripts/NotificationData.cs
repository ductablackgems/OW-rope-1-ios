using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotificationData {
    public List<int> notificationID;

    public NotificationData() {
        notificationID = new List<int>();
    }

    public string ToJson() {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

    public static NotificationData GetInstance() {
        string data = PlayerPrefs.GetString("notification_data", string.Empty);
        if (string.IsNullOrEmpty(data))
        {
            return new NotificationData();
        }
        else {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationData>(data);
        }
    }

    public void SaveInstance() {
        string data = this.ToJson();
        PlayerPrefs.SetString("notification_data", data);
    }
}
