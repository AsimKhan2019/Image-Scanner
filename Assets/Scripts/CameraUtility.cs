using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.IO;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using System.Runtime.InteropServices;
using System;

using OpenCVForUnity.ImgprocModule;

public class CameraUtility : MonoBehaviour
{
    [SerializeField]
    FloatReference contrastAlpha;
    [SerializeField]
    FloatReference contrastBeta;
    [SerializeField]
    FloatReference cannyLow;
    [SerializeField]
    FloatReference cannyHi;
    [SerializeField]
    BoolReference contrastFilter;
    [SerializeField]
    BoolReference cannyFilter;

    ARCameraBackground m_ARCameraBackground;

    RenderTexture renderTexture;
    RenderTexture rt;

    Texture2D m_LastCameraTexture;

    Vector2 startPos;
    RectTransform rect;

    Mat imgMat;
    Mat cannyMat;

    Texture2D temp;
    Texture2D TargetTexture;

    GCHandle arrayHandle;


#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
        const string LIBNAME = "__Internal";
#else
    const string LIBNAME = "opencvforunity";
#endif
    [DllImport(LIBNAME)]
    private static extern void OpenCVForUnity_ByteArrayToMatData(IntPtr byteArray, IntPtr Mat);

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        m_ARCameraBackground = GameObject.Find("AR Camera").GetComponent<ARCameraBackground>();
        temp = Resources.Load("Image") as Texture2D;

#if UNITY_EDITOR
        m_LastCameraTexture = new Texture2D(temp.width, temp.height, TextureFormat.RGBA32, 1, true);
#else
        m_LastCameraTexture =  new Texture2D(Screen.width/2,Screen.height/2,TextureFormat.RGBA32,1,true);
#endif
        var width = m_LastCameraTexture.width;
        var height = m_LastCameraTexture.height;

        //m_LastCameraTexture.Resize(width, height);
        //Create a new RenderTexture for the RawImage on the canvas
        renderTexture = new RenderTexture(width, height, 0);
        rt = new RenderTexture(width, height, 0);
        imgMat = new Mat(height, width, CvType.CV_8UC4);
        cannyMat = new Mat(height, width, CvType.CV_8UC4);

        //Assign the new render texture to the RawImage
        GetComponent<RawImage>().texture = renderTexture;
        //Assign the new render texture to the Background RawImage
        GameObject.Find("BackGroundCamera").GetComponent<RawImage>().texture = renderTexture;
        //Copy the texture
        TargetTexture = new Texture2D(width, height, TextureFormat.RGBA32, 1, true);

        contrastFilter.Value = true;
    }

    // Update is called once per frame
    void Update()
    {
        var activeRenderTexture = RenderTexture.active;
        RenderTexture.active = rt;

#if !UNITY_EDITOR
//Grab the ar camera background and put it onto a temporary render texture
        Graphics.Blit(null, rt, m_ARCameraBackground.material);
        //outside the editor, write the image back to the texture in memory
        m_LastCameraTexture.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0, false);
        m_LastCameraTexture.Apply();   
#else
        //  Graphics.Blit(null, rt, m_ARCameraBackground.material);
        Graphics.Blit(temp, rt);
        //write the targetTexture to the texture stored in memory
        m_LastCameraTexture.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0, false);
        m_LastCameraTexture.Apply();
#endif
        RenderTexture.active = activeRenderTexture;

        //This code increases the contrast of the image stored in the GC-ALLOC'd space 
        //It then converts that material into a texture -- TextureForRenderer
        //Finally it Blits the texture to the render texture on the canvas

        //create a Mat out of the texture2D - stored at address (currenlty m_LastCameraTexture)

        //read raw texture from byte[] and convert it in memory to a mat
        unsafe
        {
            byte[] buffer = m_LastCameraTexture.GetRawTextureData();
            fixed (byte* p = buffer)
            {
                IntPtr ptr = (IntPtr)p;
                // do you stuff here
                OpenCVForUnity_ByteArrayToMatData(ptr, imgMat.nativeObj); //Stored in this memory addr is the original m_LastCameraTexture
            }
        }

        //apply the contrast filter
        if (contrastFilter.Value)
            imgMat.convertTo(imgMat, -1, contrastAlpha.Value, contrastBeta.Value); //adjust the contrast of the mat

        //apply the canny filter
        OpenCVForUnity.ImgprocModule.Imgproc.Canny(imgMat, cannyMat, cannyLow, cannyHi, 3, true);
        //invert black to white
        OpenCVForUnity.CoreModule.Core.bitwise_not(cannyMat, cannyMat);

        //return canny or unfiltered mat depending on setting
        if (cannyFilter.Value)
        {
            print("Canny on");
            Utils.matToTexture2D(cannyMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }
        else
        {
            print("Canny off");
            Utils.matToTexture2D(imgMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }
        Graphics.Blit(TargetTexture, renderTexture);
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
        var gos = GameObject.FindGameObjectsWithTag("HideOnScreenCapture");

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

    private void OnApplicationQuit()
    {
    }
}
