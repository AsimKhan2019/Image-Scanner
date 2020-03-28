﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.IO;

public class CameraUtility : MonoBehaviour
{
    ARCameraBackground m_ARCameraBackground;
    RenderTexture renderTexture;
    Texture2D m_LastCameraTexture;

    Vector2 startPos;
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        m_ARCameraBackground = GameObject.Find("AR Camera").GetComponent<ARCameraBackground>();
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        GetComponent<RawImage>().texture = renderTexture;

        startPos = new Vector3(Mathf.FloorToInt(rect.position.x), Mathf.FloorToInt(rect.position.y),0);
    }

    // Update is called once per frame
    void Update()
    {

        // Copy the camera background to a RenderTexture
        Graphics.Blit(null, renderTexture, m_ARCameraBackground.material);

        // Copy the RenderTexture from GPU to CPU
        var activeRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        if (m_LastCameraTexture == null)
            m_LastCameraTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, true);
        m_LastCameraTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        m_LastCameraTexture.Apply();
        RenderTexture.active = activeRenderTexture;
    }
    private void LateUpdate()
    {
        rect.position = startPos;
        Utilities.SizeToParent(GetComponent<RawImage>());
    }

    public void OnPhotoButtonPressed()
    {
        StartCoroutine("CaptureImage");
    }

    IEnumerator CaptureImage()
    {
        var mask = GameObject.Find("Mask").GetComponent<Image>();
        mask.GetComponent<MaskUtility>().GenerateMaskedTexture();
        yield break;
/*
        var maskedTexture = Utilities.GetTextureWithMask(m_LastCameraTexture, mask.sprite.texture);

#if UNITY_EDITOR
        //in the unity editor we assign a placeholder image instead of saving to gallery
        var temp = GameObject.Find("TempImage");
        temp.GetComponent<Image>().sprite = Sprite.Create(maskedTexture,
        new Rect(0, 0, maskedTexture.width, maskedTexture.height), new Vector2(0.5f, 0.5f), 1);
#endif

        // Write to file
        var bytes = maskedTexture.EncodeToPNG();

        NativeGallery.SaveImageToGallery(bytes, "Scans", "Image.png", null);
        yield break;
        */
    }

}