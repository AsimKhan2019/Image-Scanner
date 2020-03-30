using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropArea : MonoBehaviour
{
    [SerializeField]
    RectTransform imageRect;

    RectTransform rect;
    bool isMoving;

    RectTransform Corner0;
    RectTransform Corner1;
    RectTransform maskRect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();

        Corner0 = GameObject.Find("TL").GetComponent<RectTransform>();
        Corner1 = GameObject.Find("BR").GetComponent<RectTransform>();
        maskRect = GameObject.Find("Mask").GetComponent<RectTransform>();

    }

    void Update()
    {
        var pos = Input.mousePosition;

        if (isMoving)
        {
            transform.position = new Vector2(pos.x, pos.y);

            var parent = imageRect.parent.GetComponent<RectTransform>();

            var tlExtents = new Vector2(Mathf.Max(-rect.sizeDelta.x / 2 + rect.anchoredPosition.x, -imageRect.sizeDelta.x / 2),
                Mathf.Min(rect.sizeDelta.y / 2 + rect.anchoredPosition.y, imageRect.sizeDelta.y / 2));

            var trExtents = new Vector2(Mathf.Min(rect.sizeDelta.x / 2 + rect.anchoredPosition.x, imageRect.sizeDelta.x / 2),
                Mathf.Min(rect.sizeDelta.y / 2 + rect.anchoredPosition.y, imageRect.sizeDelta.y / 2));

            var blExtents = new Vector2(Mathf.Max(-rect.sizeDelta.x / 2 + rect.anchoredPosition.x, -imageRect.sizeDelta.x / 2),
                Mathf.Max(-rect.sizeDelta.y / 2 + rect.anchoredPosition.y, -imageRect.sizeDelta.y / 2));

            var brExtents = new Vector2(Mathf.Min(rect.offsetMax.x, imageRect.offsetMax.x + parent.anchoredPosition.x),
                Mathf.Max(rect.offsetMin.y, imageRect.offsetMin.y + parent.anchoredPosition.y));

            GameObject.Find("TL").GetComponent<RectTransform>().anchoredPosition = new Vector2(tlExtents.x, tlExtents.y);
            GameObject.Find("TR").GetComponent<RectTransform>().anchoredPosition = trExtents;
            GameObject.Find("BL").GetComponent<RectTransform>().anchoredPosition = blExtents;
            GameObject.Find("BR").GetComponent<RectTransform>().anchoredPosition = brExtents;

            Utilities.BindToCorners(maskRect, Corner0, Corner1);
        }
    }

    public void onButtonPressed()
    {
        isMoving = true;
    }

    public void onButtonReleased()
    {
        isMoving = false;

        Utilities.BindToCorners(rect, Corner0, Corner1);
        Utilities.BindToCorners(maskRect, Corner0, Corner1);

    }
    // Update is called once per frame
    void LateUpdate()
    {
    }
}
