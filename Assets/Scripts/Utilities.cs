using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        var parent = image.transform.parent.parent.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();
        if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
        padding = 1 - padding;
        float w = 0, h = 0;
        float ratio = image.texture.width / (float)image.texture.height;
        var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
        if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
        {
            //Invert the bounds if the image is rotated
            bounds.size = new Vector2(bounds.height, bounds.width);
        }
        //Size by height first
        h = bounds.height * padding;
        w = h * ratio;
        if (w > bounds.width * padding)
        { //If it doesn't fit, fallback to width;
            w = bounds.width * padding;
            h = w / ratio;
        }
        imageTransform.sizeDelta = new Vector2(w, h);
        return imageTransform.sizeDelta;
    }

    public static Texture2D GetTextureWithMask(Texture2D img, Texture2D mask)
    {
        if (img == null)
        {
            img = GameObject.Find("CamDebugPanel").GetComponent<Image>().sprite.texture;
        }

        Texture2D output = new Texture2D(img.width, img.height);

        for (int i = 0; i < img.width; i++)
        {
            for (int j = 0; j < img.height; j++)
            {
                if (mask.GetPixel(i, j).a > 0.5f)
                {
                    output.SetPixel(i, j, img.GetPixel(i, j));
                }
                else
                {
                    output.SetPixel(i, j, new Color(1f, 0f, 0f, 1f));
                }
            }
        }
        output.Apply();

        return output;
    }

    public static void BindToCorners(RectTransform rect, RectTransform Corner1, RectTransform Corner2){
         var offset = new Vector2(0, 0);
        
        rect.offsetMin = new Vector2(Corner1.anchoredPosition.x + offset.x, Corner2.anchoredPosition.y + offset.y);
        rect.offsetMax = new Vector2(Corner2.anchoredPosition.x + offset.x, Corner1.anchoredPosition.y + offset.y);
    }
}
