using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.IO;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;

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
        GameObject.Find("BackGroundCamera").GetComponent<RawImage>().texture = renderTexture;

        // startPos = new Vector3(Mathf.FloorToInt(rect.position.x), Mathf.FloorToInt(rect.position.y), 0);
        Utils.setDebugMode(true);

    }

    // Update is called once per frame
    void Update()
    {
        Texture2D TestImage = null;

        // Copy the camera background to a RenderTexture
        try
        {
            Graphics.Blit(null, renderTexture, m_ARCameraBackground.material);
        }
        catch
        {
            TestImage = Resources.Load("Image") as Texture2D;
            Graphics.Blit(TestImage, renderTexture);
        }

        // Copy the RenderTexture from GPU to CPU
        var activeRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        if (m_LastCameraTexture == null)
            m_LastCameraTexture = new Texture2D(TestImage.width, TestImage.height, TextureFormat.RGB24, true);

        Mat imgMat = new Mat(TestImage.height, TestImage.width, CvType.CV_8UC4);

        Utils.texture2DToMat(TestImage, imgMat);

     imgMat.convertTo(imgMat,-1,2,0);

        Texture2D texture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);

        Utils.matToTexture2D(imgMat, texture);
        Graphics.Blit(texture, renderTexture);


        texture.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = activeRenderTexture;



    }
    private void LateUpdate()
    {
        rect.position = transform.parent.parent.position;
    }

    public void OnPhotoButtonPressed()
    {
        StartCoroutine("CaptureImage");
    }

    IEnumerator CaptureImage()
    {
        var gos = GameObject.FindGameObjectsWithTag("CropButton");

        //disable the buttons before capturing the image
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].GetComponentInChildren<Image>().enabled = false;
        }

        //Get the mask component
        var mask = GameObject.Find("Mask").GetComponent<Image>();
        mask.GetComponent<MaskUtility>().GenerateMaskedTexture();
        yield break;
    }

}
