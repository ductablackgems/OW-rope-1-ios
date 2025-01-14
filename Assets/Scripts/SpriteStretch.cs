using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteStretch : MonoBehaviour
{

    void Start()
    {
        transform.localScale = new Vector3((float)Screen.width * 1080 / (Screen.height * 1920), (float)Screen.width * 1080 / (Screen.height * 1920), 1) * 1.05f;
    }
}