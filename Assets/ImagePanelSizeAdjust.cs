using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePanelSizeAdjust : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Set the height and width according to the size required to fill the scroll rect
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        var imageTransform = GetComponent<RectTransform>();
        
        float w = 0, h = 0;

            var totalHeight = 0.0f;
            //Size the other elements in the Scroll View;
            for (int i = 0; i< scrollRect.content.childCount;i++){
                var rt = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                //don't include ourself
                if(rt != imageTransform)
                    totalHeight += rt.rect.height;
            }

        //Get the bounds of the scroll rect and subtract the size of other elements in the rect
        var boundsW =scrollRectTransform.rect.width;
        var boundsH = scrollRectTransform.rect.height - totalHeight;

        var bounds = new Rect(0, 0, boundsW, boundsH);
        float ratio = scrollRectTransform.rect.width / (float)scrollRectTransform.rect.height;

        h = bounds.height;
        w = h * ratio;
        if (w > bounds.width)
        { //If it doesn't fit, fallback to width;
            w = bounds.width;
            h = w / ratio;
        }
        imageTransform.sizeDelta = new Vector2(w, h);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
