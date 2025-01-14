using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using BG_Library.NET;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CallAdsManager
{


    public enum PositionAds
    {
        weapon_buy,
        weapon_next,
        weapon_back,
        character_next,
        character_buy,
        character_skin,
        character_back,
        menu_weapon,
        menu_character,
        menu_options,
        menu_options_button,
        menu_play,
        gameplay_minimap,
        gameplay_minimap_back,
        gameplay_pause,
        gameplay_changegun,
        gameplay_vehicle_button,
        gameplay_vehicle_dump,
        gameplay_garage,
        gameplay_shopping,
        gameplay_Interact,
        gameplay_quest_continue,
        gameplay_quest_exit,
        gameplay_quest_close,
        gameplay_atm_button,
        gameplay_skill_1,
        gameplay_die
    }

    public static void InitDataShow(string value)
    {
        // try
        // {
        //     StructAds = JsonConvert.DeserializeObject<StructAdsConfig>(value);
        //     if (StructAds == null)
        //     {
        //         StructAds = new StructAdsConfig();
        //     }
        //     else
        //     {
        //         if (StructAds.StructAds == null)
        //         {
        //             StructAds.StructAds = new Dictionary<string, StructInfoAds>();
        //         }
        //     }
        // }
        // catch
        // {
        //     StructAds = new StructAdsConfig();
        // }
    }


 

    // Start is called before the first frame update
  



   
    public static void ShowInterstial(PositionAds pos, Action actionDone)
    {
#if IGNORE_ADS
        actionDone?.Invoke();
        return;
#endif
        AdsManager.ShowInterstitial(pos.ToString());
        actionDone?.Invoke();
    }

    public static bool RewardedIsReady()
    {
        return AdsManager.IsRewardedReady();
    }

    public static void ShowRewardVideo(Action actionDone, string pos = "")
    {
#if IGNORE_ADS
        actionDone?.Invoke();
        return;
#endif
        AdsManager.ShowRewardVideo(pos, actionDone);
    }

    public static void ShowBanner()
    {
        
    }

    public static void HideBanner()
    {
       
    }

    public static void ChangeMediation()
    {
       
    }



    public static void LogEventFirebase(string str)
    {
        Debug.Log("Push Firebase Event: " + str);
    }

    public static bool CheckInternet()
    {
        return true;
    }

    //static AndroidJavaClass javaUnityPlayer;
    //static AndroidJavaObject currentActivity;
    //public void ActiveSaga()
    //{
    //    javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //    AndroidJavaClass cls = new AndroidJavaClass("com.twi.saga.Saga");

    //    cls.CallStatic("initFirebase", currentActivity);
    //}
}