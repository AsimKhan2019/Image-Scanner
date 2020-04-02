using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadPixels : MonoBehaviour
{
    public bool grab;
    public Texture2D texture;

    [SerializeField]
    RectTransform maskRect;

    [SerializeField]
    BoolReference FilterActive = null;
    bool Reactivate;

    private void OnPostRender()
    {
        if (grab)
        {
            StartCoroutine("ReadPixelsAtEndOfFrame");
        }
    }

    IEnumerator ReadPixelsAtEndOfFrame()
    {
        var gos = GameObject.FindGameObjectsWithTag("HideOnScreenCapture");
        GameObject BL = null;
        GameObject BR = null;
        GameObject TL = null;

        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].name == "BL")
                BL = gos[i];
            if (gos[i].name == "BR")
                BR = gos[i];
            if (gos[i].name == "TL")
                TL = gos[i];
        }

        var tPos = BL.transform.position;

        var rect_x = tPos.x;
        var rect_y = tPos.y;
        var rect_width = BR.transform.position.x - BL.transform.position.x;
        var rect_height = TL.transform.position.y - BL.transform.position.y;

        yield return new WaitForEndOfFrame();
        texture = new Texture2D(Mathf.FloorToInt(rect_width), Mathf.FloorToInt(rect_height),
      TextureFormat.ARGB32, false);

        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(rect_x, rect_y, rect_width, rect_height)
                , 0, 0, false);

        texture.Apply();
        byte[] _bytes = texture.EncodeToPNG();

#if UNITY_EDITOR
        //in unity editor we will save it
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/image.png", _bytes);
#endif
        NativeGallery.SaveImageToGallery(_bytes, "Scans", "Image.png", null);

        grab = false;

        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].GetComponent<Image>().enabled = true;
        }

        if (Reactivate)
        {
            FilterActive.Value = true;
            Reactivate = false;
        }

        yield break;
    }

    void ReactivateFilter()
    {
        Reactivate = true;
    }
}
