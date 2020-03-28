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
    RectTransform frame;

    RectTransform[] opposites;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        opposites = new RectTransform[2];

        frame = GameObject.Find("CropButton").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            var pos = Input.mousePosition;
            transform.position = new Vector2(pos.x, pos.y);

            opposites[0].anchoredPosition = new Vector2(opposites[0].anchoredPosition.x, rect.anchoredPosition.y);
            opposites[1].anchoredPosition = new Vector2(rect.anchoredPosition.x, opposites[1].anchoredPosition.y);
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
    }
}
