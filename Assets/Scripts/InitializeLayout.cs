using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;

public class InitializeLayout : MonoBehaviour
{

    GameObject CameraPermissionButton;
    // Start is called before the first frame update
    void Start()
    {
        CameraPermissionButton = GameObject.Find("QueryPermissionButton");
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        //enable the image panel 
        foreach (Image i in GameObject.Find("Loading Panel").GetComponentsInChildren<Image>())
            i.enabled = true;

        yield return new WaitForEndOfFrame();

        var CamRect = GameObject.Find("ImagePanel").GetComponent<RectTransform>();
        var parent = CamRect.parent.GetComponent<RectTransform>();
        var CamImage = GameObject.Find("CameraImage").GetComponent<RawImage>();

        if (CamImage.texture != null)
            Keenan_UI.Utilities.SizeToParent(CamImage);

        var bImage = GameObject.Find("BackGroundCamera").GetComponent<RawImage>();
        if (bImage.texture != null)
            Keenan_UI.Utilities.SizeToParent(bImage);

        try
        {
            var dImage = GameObject.Find("DebugTexture").GetComponent<RawImage>();
            if (dImage.texture != null)
                Keenan_UI.Utilities.SizeToParent(dImage);
        }
        catch
        {

        }

        var ImageToBind = GameObject.Find("CameraImage").GetComponent<RectTransform>();

        var corner0 = GameObject.Find("TL").GetComponent<RectTransform>();
        corner0.anchoredPosition = new Vector2(-ImageToBind.sizeDelta.x / 2, ImageToBind.sizeDelta.y / 2);
        var corner1 = GameObject.Find("TR").GetComponent<RectTransform>();
        corner1.anchoredPosition = new Vector2(ImageToBind.sizeDelta.x / 2, ImageToBind.sizeDelta.y / 2);
        var corner2 = GameObject.Find("BL").GetComponent<RectTransform>();
        corner2.anchoredPosition = new Vector2(-ImageToBind.sizeDelta.x / 2, -ImageToBind.sizeDelta.y / 2);
        var corner3 = GameObject.Find("BR").GetComponent<RectTransform>();
        corner3.anchoredPosition = new Vector2(ImageToBind.sizeDelta.x / 2, -ImageToBind.sizeDelta.y / 2);

        Keenan_UI.Utilities.BindToCorners(GameObject.Find("Mask").GetComponent<RectTransform>(), corner0, corner3);

        //Turn off the splash screen after a time.
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Loading Panel").SetActive(false);

        StartCoroutine("CheckForCamera");

        yield break;
    }

    IEnumerator CheckForCamera()
    {
        while (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            //check if we have the camera permission
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.Log("webcam found");
                CameraPermissionButton.SetActive(false);
            }
            else
            {
                Debug.Log("webcam not found");
                CameraPermissionButton.SetActive(true);
            }
            yield return null;
        }
        yield break;
    }
    public void OnRequestCameraClick()
    {
        Debug.Log("request permissions");
        NativeCamera.Permission permisso = NativeCamera.RequestPermission();
        if (permisso == NativeCamera.Permission.Granted)
        {
            GameObject.Find("QueryPermissionButton").SetActive(false);
        }

    }

}

