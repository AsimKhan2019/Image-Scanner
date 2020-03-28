using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUtility : MonoBehaviour
{
    RectTransform baseImgRect;
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        baseImgRect = GameObject.Find("CameraImage").GetComponent<RectTransform>();
        GenerateEmptyMask();
    }

    public void GenerateEmptyMask()
    {
        //set up a new texture with full transparency, this is our new mask
        var newTexture = new Texture2D((int)rect.rect.width, (int)rect.rect.height);

        var fillColor = new Color(0, 0, 0, 1);
        var fillColorArray = newTexture.GetPixels();
        var length = newTexture.GetPixels().Length;

        int k = 0;
        for (int i = 0; i < newTexture.width; i++)
        {
            for (int j = 0; j < newTexture.height; j++)
            {
                fillColorArray[k] = fillColor;
                k++;
            }
        }

        newTexture.SetPixels(fillColorArray);
        newTexture.Apply();

        var newMask = Sprite.Create(newTexture,
        new Rect(0, 0, (int)rect.rect.width, (int)rect.rect.height), new Vector2(0.5f, 0.5f), 1);

        GetComponent<Image>().sprite = newMask;
    }

    public void GenerateMaskedTexture()
    {
        Camera.main.GetComponent<ReadPixels>().grab = true;
    }

    IEnumerator SetNewTexture()
    {
        var newMask = Sprite.Create(Camera.main.GetComponent<ReadPixels>().texture,
      new Rect(0, 0, (int)rect.rect.width, (int)rect.rect.height), new Vector2(0.5f, 0.5f), 1);
        GetComponent<Image>().sprite = newMask;
        yield break;
    }


}
