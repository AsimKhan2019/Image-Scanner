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
    FloatReference contrastAlpha = null;
    [SerializeField]
    FloatReference contrastBeta = null;
    [SerializeField]
    BoolReference contrastFilterActive = null;
    [SerializeField]
    BoolReference ShouldThreshold = null;

    [SerializeField]
    GameObject ConfirmationDialogue;

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
    Mat grey;
    Mat thresholdMat;
    Mat imageMaskMat;
    Mat invertedMask;
    Mat finalMat;
    Mat coloredPieceMat;
    Mat greyPieceMat;

    //unused Mats
    Mat cannyMat;
    Mat Heirarchy;
    Mat tmp1;
    Mat tmp2;
    Mat white;

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
        thresholdMat = new Mat(height, width, CvType.CV_8UC4);
        imageMaskMat = new Mat(height, width, CvType.CV_8UC4);
        invertedMask = new Mat(height, width, CvType.CV_8UC4);
        finalMat = new Mat(height, width, CvType.CV_8UC4);
        coloredPieceMat = new Mat(height, width, CvType.CV_8UC4);
        greyPieceMat = new Mat(height, width, CvType.CV_8UC4);
        tmp1 = new Mat(height, width, CvType.CV_8UC4);
        tmp2 = new Mat(height, width, CvType.CV_8UC4);
        white = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));

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
            //Convert image to grey
            Imgproc.cvtColor(imgMat, grey, Imgproc.COLOR_BGR2GRAY);

            //Threshold to get outlines
            Imgproc.adaptiveThreshold(grey, thresholdMat, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 21, 15);

            //using bitwise_not we turn this thresholded Mat into a Mask
            Core.bitwise_not(thresholdMat, imageMaskMat);

            //Get the inverted mask
            Core.bitwise_not(imageMaskMat, invertedMask);

            //Then we mask the original image using bitwise and the mask
            Core.bitwise_or(imgMat, imgMat, tmp1, imageMaskMat);
            tmp1.copyTo(coloredPieceMat);

            //Then use the inverted mask and the thersholded image
            Core.bitwise_or(white, white, tmp1, invertedMask);
            tmp1.copyTo(greyPieceMat);

            //add the pieces
            Core.bitwise_and(coloredPieceMat, greyPieceMat, tmp2);
            tmp2.copyTo(finalMat);

            Utils.matToTexture2D(finalMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }
        else
        {
            Utils.matToTexture2D(imgMat, TargetTexture, false, -1, true); //Copy the mat into targetTex to avoid overwriting m_LastCameraTexture
        }

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

        yield return new WaitForSeconds(.2f);
#if UNITY_IOS
        //check if we have photogallery permission
        if (NativeGallery.CheckPermission() == NativeGallery.Permission.Granted)
        {
            Debug.Log("Saving image");
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().SetMessage("The scan has been saved. Check your photo gallery");
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().Show();
            yield return new WaitForSeconds(3.0f);
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().Hide();
        }
        else
        {
            Debug.Log("Could not save image");
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().SetMessage("Unable to save image: Please enable photo gallery permission");
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().Show();
            yield return new WaitForSeconds(4.0f);
            ConfirmationDialogue.GetComponent<PictureSavedResponse>().Hide();
        }
#endif
        yield break;
    }

    private void OnApplicationQuit()
    {
    }
}
