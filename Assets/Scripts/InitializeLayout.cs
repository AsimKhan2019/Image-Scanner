using System.Collections;
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

        var CamRect = GameObject.Find("CameraImage").GetComponent<RectTransform>();
        var parent = CamRect.parent.GetComponent<RectTransform>();
        var CamImage = GameObject.Find("CameraImage").GetComponent<RawImage>();

        if (CamImage.texture != null)
            Utilities.SizeToParent(CamImage);

        GameObject.Find("BackGroundCamera").GetComponent<RectTransform>().offsetMin = CamRect.offsetMin
            + CamRect.parent.GetComponent<RectTransform>().anchoredPosition;
        GameObject.Find("BackGroundCamera").GetComponent<RectTransform>().offsetMax = CamRect.offsetMax
            + CamRect.parent.GetComponent<RectTransform>().anchoredPosition;

        GameObject.Find("GreyOverlay").GetComponent<RectTransform>().offsetMin = CamRect.offsetMin
     + CamRect.parent.GetComponent<RectTransform>().anchoredPosition;
        GameObject.Find("GreyOverlay").GetComponent<RectTransform>().offsetMax = CamRect.offsetMax
            + CamRect.parent.GetComponent<RectTransform>().anchoredPosition;

        yield return new WaitForEndOfFrame();
        var ImageToBind = GameObject.Find("CameraImage").GetComponent<RectTransform>();

        var corner0 = GameObject.Find("TL").GetComponent<RectTransform>();
        corner0.anchoredPosition= new Vector2(ImageToBind.offsetMin.x, ImageToBind.offsetMax.y);
        var corner1 = GameObject.Find("TR").GetComponent<RectTransform>();
        corner1.anchoredPosition = new Vector2(ImageToBind.offsetMax.x, ImageToBind.offsetMax.y);
        var corner2 = GameObject.Find("BL").GetComponent<RectTransform>();
        corner2.anchoredPosition = new Vector2(ImageToBind.offsetMin.x, ImageToBind.offsetMin.y);
        var corner3 = GameObject.Find("BR").GetComponent<RectTransform>();
        corner3.anchoredPosition = new Vector2(ImageToBind.offsetMax.x, ImageToBind.offsetMin.y);

        Utilities.BindToCorners(GameObject.Find("Mask").GetComponent<RectTransform>(), GameObject.Find("TL").GetComponent<RectTransform>()
        , GameObject.Find("BR").GetComponent<RectTransform>());
        yield break;


    }





}

