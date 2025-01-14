using System;
using System.Collections;
using System.Collections.Generic;
using BG_Library.NET;
using UnityEngine;

public class TestMREC : MonoBehaviour
{
    public string mrecID = "8064bca61b68359d";
    public RectTransform point;

    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            MaxSdk.ShowMediationDebugger();
            InitializeMRecAds();
            //ManagerExistingPrivacySettings();
        };

        MaxSdk.SetSdkKey("ZBycq6bsa9nnSSt3X0HL9oI03oIGT9DeA2FdS2lv1QShbl5gQoHW9psPsadw5y793VnIML2oCNgFH_FLNWZ8Co");
        MaxSdk.InitializeSdk();
    }

    public void InitializeMRecAds()
    {
        Debug.Log("Init MREC");
        
        var density = MaxSdkUtils.GetScreenDensity();
        var dp = 50 / density;
        
        Debug.Log(density);
        Debug.Log(Screen.width + " " + Screen.height);
        Debug.Log(point.anchoredPosition / density);
        
        // MRECs are sized to 300x250 on phones and tablets
        /*MaxSdk.CreateMRec(mrecID
            , point.anchoredPosition.x / density, point.anchoredPosition.y / density);*/
        MaxSdk.CreateMRec(mrecID
            , Screen.width / density / 2, -200 / density);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }
    
    public void CREATEMREC()
    {
        Debug.Log("CREATE MREC");
        MaxSdk.CreateMRec(mrecID, MaxSdkBase.AdViewPosition.BottomRight);
    }

    public void ShowMREC()
    {
        Debug.Log("SHOW MREC");
        MaxSdk.ShowMRec(mrecID);
    }
    
    
    public void DestroyMREC()
    {
        Debug.Log("DESTROY MREC");
        MaxSdk.DestroyMRec(mrecID);
    }
    
    public void HIDEMREC()
    {
        Debug.Log("HIDE MREC");
        MaxSdk.HideMRec(mrecID);
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MREC LOADED " + adInfo.Placement);
        
        ShowMREC();
    }

    public static void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
    {
        Debug.Log("MREC LOAD FAIL " + error.Message);
    }

    public static void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    public static void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    { 

    }

    public static void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    { 

    }

    public static void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    { 

    }
}