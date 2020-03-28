using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropArea : MonoBehaviour
{
    [SerializeField]
    RectTransform imageRect;

    RectTransform rect;
    bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        var pos = Input.mousePosition;

        if (isMoving)
        {
            GetComponent<BindToCorners>().enabled = false;
            transform.position = new Vector2(pos.x, pos.y);

            GameObject.Find("TL").GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.offsetMin.x, rect.offsetMax.y);
            GameObject.Find("TR").GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.offsetMax.x, rect.offsetMax.y);
            GameObject.Find("BL").GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.offsetMin.x, rect.offsetMin.y);
            GameObject.Find("BR").GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.offsetMax.x, rect.offsetMin.y);

        }
        else
        {
            GetComponent<BindToCorners>().enabled = true;
        }
    }

    public void onButtonPressed()
    {
        isMoving = true;
    }

    public void onButtonReleased()
    {
        isMoving = false;
    }
    // Update is called once per frame
    void LateUpdate()
    {
    }
}
