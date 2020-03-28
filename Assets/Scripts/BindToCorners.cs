using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindToCorners : MonoBehaviour
{
    static RectTransform[] Corners;

    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        Corners = new RectTransform[4];
        rect = GetComponent<RectTransform>();
        Corners[0] = GameObject.Find("TL").GetComponent<RectTransform>();
        Corners[2] = GameObject.Find("BR").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var offset = new Vector2(0, 0);
        
        rect.offsetMin = new Vector2(Corners[0].anchoredPosition.x + offset.x, Corners[2].anchoredPosition.y + offset.y);
        rect.offsetMax = new Vector2(Corners[2].anchoredPosition.x + offset.x, Corners[0].anchoredPosition.y + offset.y);
    }
}
