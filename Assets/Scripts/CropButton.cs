using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropButton : MonoBehaviour
{
    enum Corner
    {
        TOP_LEFT,
        BOT_LEFT,
        TOP_RIGHT,
        BOT_RIGHT
    }

    [SerializeField]
    Corner m_Corner;

    public bool isMoving;

    RectTransform rect;
    RectTransform MoveCropButton;

    RectTransform[] opposites;

    RectTransform Corner0;
    RectTransform Corner1;
    RectTransform maskRect;

    Vector2 Extents;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        opposites = new RectTransform[2];

        MoveCropButton = GameObject.Find("CropButton").GetComponent<RectTransform>();

        Corner0 = GameObject.Find("TL").GetComponent<RectTransform>();
        Corner1 = GameObject.Find("BR").GetComponent<RectTransform>();
        maskRect = GameObject.Find("Mask").GetComponent<RectTransform>();

        StartCoroutine(initPosition());


    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            var imageRect = GameObject.Find("CameraImage").GetComponent<RectTransform>();
            var parent = imageRect.parent.GetComponent<RectTransform>();

            var pos = new Vector2(0, 0);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, Input.mousePosition, null, out pos);

            switch (m_Corner)
            {
                case Corner.TOP_LEFT:
                    pos = new Vector3(Mathf.Max(pos.x, imageRect.offsetMin.x + parent.anchoredPosition.x),
                    Mathf.Min(pos.y, imageRect.offsetMax.y + parent.anchoredPosition.y),
                    0);
                    break;
                case Corner.TOP_RIGHT:
                    pos = new Vector3(Mathf.Min(pos.x, imageRect.offsetMax.x + parent.anchoredPosition.x),
                      Mathf.Min(pos.y, imageRect.offsetMax.y + parent.anchoredPosition.y),
                      0);
                    break;
                case Corner.BOT_LEFT:
                pos = new Vector3(Mathf.Max(pos.x, imageRect.offsetMin.x + parent.anchoredPosition.x),
                      Mathf.Max(pos.y, imageRect.offsetMin.y + parent.anchoredPosition.y),
                      0);
                    break;
                case Corner.BOT_RIGHT:
                  pos = new Vector3(Mathf.Min(pos.x, imageRect.offsetMax.x + parent.anchoredPosition.x),
                      Mathf.Max(pos.y, imageRect.offsetMin.y + parent.anchoredPosition.y),
                      0);
                    break;
                default:
                    break;
            }


            rect.anchoredPosition = new Vector2(pos.x, pos.y);

            opposites[0].anchoredPosition = new Vector2(opposites[0].anchoredPosition.x, rect.anchoredPosition.y);
            opposites[1].anchoredPosition = new Vector2(rect.anchoredPosition.x, opposites[1].anchoredPosition.y);

            Utilities.BindToCorners(MoveCropButton, Corner0, Corner1);
            Utilities.BindToCorners(maskRect, Corner0, Corner1);

        }
    }

    public void onButtonPress()
    {
        //assign opposites
        switch (m_Corner)
        {
            case Corner.TOP_LEFT:
                opposites[0] = GameObject.Find("TR").GetComponent<RectTransform>();
                opposites[1] = GameObject.Find("BL").GetComponent<RectTransform>();
                break;
            case Corner.TOP_RIGHT:
                opposites[0] = GameObject.Find("TL").GetComponent<RectTransform>();
                opposites[1] = GameObject.Find("BR").GetComponent<RectTransform>();
                break;
            case Corner.BOT_LEFT:
                opposites[1] = GameObject.Find("TL").GetComponent<RectTransform>();
                opposites[0] = GameObject.Find("BR").GetComponent<RectTransform>();
                break;
            case Corner.BOT_RIGHT:
                opposites[1] = GameObject.Find("TR").GetComponent<RectTransform>();
                opposites[0] = GameObject.Find("BL").GetComponent<RectTransform>();
                break;
            default:
                break;
        }
        isMoving = true;
    }

    public void onButtonReleased()
    {
        isMoving = false;
        Utilities.BindToCorners(maskRect, Corner0, Corner1);
    }

    IEnumerator initPosition()
    {
        yield return new WaitForSeconds(1.0f);
        var ImageToBind = GameObject.Find("CameraImage").GetComponent<RectTransform>();
        switch (m_Corner)
        {
            case Corner.TOP_LEFT:
                rect.anchoredPosition = new Vector2(ImageToBind.offsetMin.x, ImageToBind.offsetMax.y);
                break;
            case Corner.TOP_RIGHT:
                rect.anchoredPosition = new Vector2(ImageToBind.offsetMax.x, ImageToBind.offsetMax.y);

                break;
            case Corner.BOT_LEFT:
                rect.anchoredPosition = new Vector2(ImageToBind.offsetMin.x, ImageToBind.offsetMin.y);

                break;
            case Corner.BOT_RIGHT:
                rect.anchoredPosition = new Vector2(ImageToBind.offsetMax.x, ImageToBind.offsetMin.y);

                break;
            default:
                break;
        }
        yield break;
    }
}
