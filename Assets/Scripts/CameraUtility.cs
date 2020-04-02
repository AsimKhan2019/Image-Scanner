using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.IO;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using System.Runtime.InteropServices;
using System;
using System.Linq;

public class CameraUtility : MonoBehaviour
{
    [SerializeField]
    FloatReference contrastAlpha;
    [SerializeField]
    FloatReference contrastBeta;
    [SerializeField]
    BoolReference contrastFilterActive;
    [SerializeField]
    BoolReference ShouldThreshold;

    ARCameraBackground m_ARCameraBackground;

    RectTransform Background;
    RectTransform Overlay;
    UnityEngine.Rect myRect;

    RenderTexture renderTexture;
    RenderTexture renderTexture2;

    RenderTexture rt;

    Texture2D m_LastCameraTexture;
    Scalar lineColor;

    Vector2 startPos;
    RectTransform rect;

    Mat imgMat;
    Mat cannyMat;
    Mat grey;
    Mat thresholdMat;
    Mat Heirarchy;

    List<MatOfPoint> contours;

    Texture2D temp;
    Texture2D TargetTexture;
    Texture2D CannyTexture;

    GCHandle arrayHandle;

    Size mySize;
    Point myPoint;

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
        float ratio = temp.width / (float)temp.height;
        float h = Screen.height;
        float w = h * ratio;

        if (w > Screen.width)
        { //If it doesn't fit, fallback to width;
            w = Screen.width;
            h = w / ratio;
        }
        m_LastCameraTexture = new Texture2D((int)w, (int)h, TextureFormat.RGBA32, 1, true);
#else
        m_LastCameraTexture =  new Texture2D(Screen.width,Screen.height,TextureFormat.RGBA32,1,true);
#endif
        var width = m_LastCameraTexture.width;
        var height = m_LastCameraTexture.height;
        CannyTexture = new Texture2D(m_LastCameraTexture.width, m_LastCameraTexture.height, TextureFormat.RGBA32, 1, true);

        //m_LastCameraTexture.Resize(width, height);
        //Create a new RenderTexture for the RawImage on the canvas
        renderTexture = new RenderTexture(width, height, 0);
        renderTexture2 = new RenderTexture(width, height, 0);

        rt = new RenderTexture(width, height, 0);
        imgMat = new Mat(height, width, CvType.CV_8UC4);
        cannyMat = new Mat(height, width, CvType.CV_8UC4);
        thresholdMat = new Mat(imgMat.height(), imgMat.width(), CvType.CV_8UC4);

        grey = new Mat();

        myRect = new UnityEngine.Rect(0, 0, rt.width, rt.height);

        //Assign the new render texture to the RawImage
        GetComponent<RawImage>().texture = renderTexture;
        //Assign the new render texture to the Background RawImage
        Background = GameObject.Find("BackGroundCamera").GetComponent<RectTransform>();

        try
        {
            Overlay = GameObject.Find("DebugTexture").GetComponent<RectTransform>();
            Overlay.GetComponent<RawImage>().texture = renderTexture2;
        }
        catch
        {

        }

        Background.GetComponent<RawImage>().texture = renderTexture;

        //Copy the texture
        TargetTexture = new Texture2D(width, height, TextureFormat.RGBA32, 1, true);

        contrastFilterActive.Value = true;

        contours = new List<MatOfPoint>();
        Heirarchy = new Mat();
        lineColor = new Scalar(0, 255, 0);
        mySize = new Size(5, 5);
        myPoint = new Point(0, 0);
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
        m_LastCameraTexture.ReadPixels(myRect,0,0,false);
        m_LastCameraTexture.Apply();   
#else
        //  Graphics.Blit(null, rt, m_ARCameraBackground.material);
        Graphics.Blit(temp, rt);
        //write the targetTexture to the texture stored in memory
        m_LastCameraTexture.ReadPixels(myRect, 0, 0, false);
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
        if (contrastFilterActive.Value)
            imgMat.convertTo(imgMat, -1, contrastAlpha.Value, contrastBeta.Value); //adjust the contrast of the mat

        //Threshold the image if "Detect Edges" is on
        if (ShouldThreshold.Value)
        {
            Imgproc.cvtColor(imgMat, grey, Imgproc.COLOR_BGR2GRAY);

            Imgproc.adaptiveThreshold(grey, grey, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 21, 15);
            grey.copyTo(thresholdMat);
            Utils.matToTexture2D(thresholdMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }
        else
        {
            Utils.matToTexture2D(imgMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }


        //TODO:
        //Canny to find contours
        //after image processing we create two render textures, one for the zoom + cropped image
        //and one that supports the regular scan
        //*************** //CANNY EDGE DETECTION
        // convert the image to grayscale and blur it slightly
        if (ShouldThreshold.Value)
            Imgproc.cvtColor(thresholdMat, grey, Imgproc.COLOR_BGR2GRAY);
        else
            Imgproc.cvtColor(imgMat, grey, Imgproc.COLOR_BGR2GRAY);

        Imgproc.GaussianBlur(grey, cannyMat, mySize, 0);


        // dilate helps to remove potential holes between edge segments
        var kernal = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, mySize);
        Imgproc.dilate(cannyMat, cannyMat, kernal);

        //apply the canny filter
        Imgproc.Canny(imgMat, cannyMat, 75, 200, 3, true);

        //invert black to white
        //Core.bitwise_not(cannyMat, cannyMat);

        /*
                //TODO: 
                //find contours in the image:
                Imgproc.findContours(cannyMat, contours, Heirarchy, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);
                List<MatOfPoint> sorted = contours.OrderByDescending(x => Imgproc.contourArea(x)).Take(5).ToList();

                for (int i = 0; i < sorted.Count; i++)
                {
                    var c = sorted[i];
                    MatOfPoint2f c2 = new MatOfPoint2f();
                    c.convertTo(c2, CvType.CV_32F);

                    // approximate the contour
                    var peri = Imgproc.arcLength(c2, true);
                    var approxCurve = new MatOfPoint2f();
                    var screenContour = new MatOfPoint2f();

                    Imgproc.approxPolyDP(c2, approxCurve, 0.02 * peri, true);

                    //if our approximated contour has four points, then we
                    //can assume that we have found our screen
                    if (approxCurve.total() == 4)
                    {
                        Imgproc.drawContours(imgMat, sorted, i, lineColor, 2, Imgproc.LINE_AA, Heirarchy, 0, myPoint);
                        screenContour = approxCurve;
                        break;
                    }
                }
                        Graphics.Blit(CannyTexture, renderTexture2);
                //END CANNY EDGE DETECTION
        */

        Utils.matToTexture2D(cannyMat, CannyTexture, true, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        Graphics.Blit(TargetTexture, renderTexture);
    }

    private void LateUpdate()
    {
        rect.position = transform.parent.parent.position;
        Background.position = transform.parent.parent.position;

        if (Overlay != null)
            Overlay.position = transform.parent.parent.position;
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
            gos[i].GetComponent<Image>().enabled = false;
        }


        //save the contrast filter state
        if (contrastFilterActive.Value)
            Camera.main.SendMessage("ReactivateFilter");

        //Deactivate the filter to remove the slider from the screen.
        contrastFilterActive.Value = false;

        //grab the camera image
        Camera.main.GetComponent<ReadPixels>().grab = true;

        yield break;
    }

    private void OnApplicationQuit()
    {
    }
}
