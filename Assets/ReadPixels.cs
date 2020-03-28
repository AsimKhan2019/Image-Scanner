using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadPixels : MonoBehaviour
{
    public bool grab;
    public Texture2D texture;

    [SerializeField]
    RectTransform maskRect;

    private void OnPostRender()
    {
        if (grab)
        {
            StartCoroutine("ReadPixelsAtEndOfFrame");
        }
    }

    IEnumerator ReadPixelsAtEndOfFrame()
    {
        var tPos = GameObject.Find("BL").transform.position;

        var rect_x = tPos.x;
        var rect_y = tPos.y;
        var rect_width = GameObject.Find("BR").transform.position.x - GameObject.Find("BL").transform.position.x;
        var rect_height = GameObject.Find("TL").transform.position.y - GameObject.Find("BL").transform.position.y;

        yield return new WaitForEndOfFrame();
        texture = new Texture2D(Mathf.FloorToInt(rect_width), Mathf.FloorToInt(rect_height),
      TextureFormat.ARGB32, false);

        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        Debug.Log("rect " + rect_x + ", " + rect_y + ":" + rect_width + ", " + rect_height);

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
        yield break;
    }
}
