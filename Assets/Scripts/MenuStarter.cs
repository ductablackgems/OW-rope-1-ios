using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("datetime"))
        {
            if (DateTime.Compare(DateTime.Now, DateTime.Parse(PlayerPrefs.GetString("datetime"))) != 0)
            {
                PlayerPrefs.SetInt("daylogin", PlayerPrefs.GetInt("daylogin") + 1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("daylogin", PlayerPrefs.GetInt("daylogin") + 1);
        }
        PlayerPrefs.SetString("datetime", DateTime.Now.ToString());
        Application.targetFrameRate = 50;
        Time.timeScale = 1;
    }
}
