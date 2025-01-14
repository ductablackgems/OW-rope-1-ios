using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerElement : MonoBehaviour
{
    public MarkerInfor uiMarker;

    void Start()
    {
        Marker.instance.AddMarker(this, transform);
    }

    void OnDisable()
    {
        Marker.instance.RemoveMarker(uiMarker);
    }
}
