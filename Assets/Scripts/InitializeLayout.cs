﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeLayout : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        var CamRect = GameObject.Find("ImagePanel").GetComponent<RectTransform>();
        var parent = CamRect.parent.GetComponent<RectTransform>();
        var CamImage = GameObject.Find("CameraImage").GetComponent<RawImage>();

        if (CamImage.texture != null)
            Utilities.SizeToParent(CamImage);

        var bImage = GameObject.Find("BackGroundCamera").GetComponent<RawImage>();
        if (bImage.texture != null)
            Utilities.SizeToParent(bImage);

        var ImageToBind = GameObject.Find("CameraImage").GetComponent<RectTransform>();

        var corner0 = GameObject.Find("TL").GetComponent<RectTransform>();
        corner0.anchoredPosition = new Vector2(-ImageToBind.sizeDelta.x / 2, ImageToBind.sizeDelta.y / 2);
        var corner1 = GameObject.Find("TR").GetComponent<RectTransform>();
        corner1.anchoredPosition = new Vector2(ImageToBind.sizeDelta.x / 2, ImageToBind.sizeDelta.y / 2);
        var corner2 = GameObject.Find("BL").GetComponent<RectTransform>();
        corner2.anchoredPosition = new Vector2(-ImageToBind.sizeDelta.x / 2, -ImageToBind.sizeDelta.y / 2);
        var corner3 = GameObject.Find("BR").GetComponent<RectTransform>();
        corner3.anchoredPosition = new Vector2(ImageToBind.sizeDelta.x / 2, ImageToBind.sizeDelta.y / 2);


        yield break;


    }





}

