using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PictureSavedResponse : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    private void Start()
    {
        Hide();
    }
    public void SetMessage(string message)
    {
        text.text = message;
    }

    public void Hide()
    {
        GetComponentInParent<Canvas>().enabled = false;
    }
    public void Show()
    {
        GetComponentInParent<Canvas>().enabled = true;

    }
}
