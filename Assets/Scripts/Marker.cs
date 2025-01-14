using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    public static Marker instance;

    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 offset;

    [SerializeField] GameObject uiMarker;
    [SerializeField] Transform canvas;

    public Camera camMain;
    Transform m_Transform;

    [SerializeField] float minX, minY;
    float maxX, maxY;

    bool isShow = true;

    public List<MarkerInfor> markers;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        //camMain = Camera.main;
        m_Transform = transform;

        maxX = Screen.width - minX;

        maxY = Screen.height - minY;
    }

    //public void ShowAllMarker(bool active)
    //{
    //    if (active)
    //    {
    //        isShow = true;
    //    }

    //    int count = markers.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        markers[i].uiMarker.SetActive(active);
    //    }
    //}

    void Update()
    {
        int count = markers.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = camMain.WorldToScreenPoint(markers[i].target.position + offset);
                if (Vector3.Dot((markers[i].target.position - m_Transform.position), m_Transform.forward) < 0)
                {
                    if (pos.x < Screen.width / 2)
                    {
                        pos.x = maxX;
                    }
                    else
                    {
                        pos.x = minX;
                    }
                }

                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                markers[i].img.transform.position = pos;
                markers[i].meter.text = ((int)(Vector3.Distance(markers[i].target.position, playerTransform.position))) + " m";
            }
        }
    }

    public void AddMarker(MarkerElement marker, Transform target)
    {
        MarkerInfor markerInfor = new MarkerInfor();
        marker.uiMarker = markerInfor;

        markerInfor.target = target;

        GameObject uiMarkerPre = Instantiate(uiMarker, canvas.transform);

        markerInfor.uiMarker = uiMarkerPre;
        markerInfor.img = uiMarkerPre.GetComponent<Image>();
        markerInfor.meter = uiMarkerPre.transform.GetChild(0).GetComponent<Text>();

        markers.Add(markerInfor);
    }

    public void RemoveMarker(MarkerInfor marker)
    {
        if (marker != null)
        {
            if (marker.uiMarker.gameObject != null)
            {
                marker.uiMarker.gameObject.SetActive(false);
                markers.Remove(marker);
            }
        }
    }
}

[System.Serializable]
public class MarkerInfor
{
    public Transform target;
    public GameObject uiMarker;
    public Image img;
    public Text meter;
}
