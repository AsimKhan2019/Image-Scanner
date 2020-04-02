using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class ControlLookAndFeel : MonoBehaviour
{
    [SerializeField]
    public LookAndFeelObject lookAndFeel;

    ColorBlock colorBlock;

    private void Update()
    {
        if (Application.isPlaying)
            return;

        colorBlock = new ColorBlock();
        colorBlock.normalColor = new Color(1,1,1,1);
        colorBlock.selectedColor = lookAndFeel.UI_Color1;
        colorBlock.highlightedColor = lookAndFeel.UI_Color1;
        colorBlock.disabledColor = lookAndFeel.UI_Color1;

        //Assign background images
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("UI_Color0"))
        {
            go.GetComponent<Image>().color = lookAndFeel.UI_Color0;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("UI_Color1"))
        {
            go.GetComponent<Image>().color = lookAndFeel.UI_Color1;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("UI_Color2"))
        {
            go.GetComponent<Image>().color = lookAndFeel.UI_Color2;
        }

        //Assign button images
        foreach (Button b in GameObject.Find("ToolbarPanel").GetComponentsInChildren<Button>())
        {
            b.GetComponent<Image>().color = lookAndFeel.UI_Color1;
        }


        //Assign text
        foreach(TextMeshProUGUI tmp in GameObject.FindObjectsOfType<TextMeshProUGUI>()){
            tmp.color = lookAndFeel.TextColor;
        }

        //Assign final color for button
        GameObject.Find("ScanButton").transform.GetChild(0).GetComponent<Image>().color = lookAndFeel.TextColor;
    }
}
