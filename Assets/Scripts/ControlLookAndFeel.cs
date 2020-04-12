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

        AssignPaletteColors();

    }

    public void AssignPaletteColors()
    {
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

        var list = GameObject.Find("ToolbarPanel").GetComponentsInChildren<FilterToggleButton>();
        for (int i = 0; i < list.Length; i++)
        {
            list[0].GetComponent<Image>().color = lookAndFeel.Positive;
            list[1].GetComponent<Image>().color = lookAndFeel.Negative;
        }

        //Assign text
        foreach (TextMeshProUGUI tmp in GameObject.FindObjectsOfType<TextMeshProUGUI>())
        {
            tmp.color = lookAndFeel.TextColor;
        }

        //Assign final color for button
        GameObject.Find("ScanButton").transform.GetChild(0).GetComponent<Image>().color = lookAndFeel.TextColor;

        //Assign slider colors
        var slider = GameObject.Find("ContrastSlider");
        //slider background
        slider.transform.GetChild(0).GetComponent<Image>().color = lookAndFeel.UI_Color0;
        //slider fill
        slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = lookAndFeel.Positive;
        //slider handle
        slider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = lookAndFeel.UI_Color1;
    }

    public void LoadFromPlayerPrefs()
    {
        //get the player pref
        if (PlayerPrefs.HasKey("LookAndFeel"))
        {
            var ppp = PlayerPrefs.GetString("LookAndFeel");

            //get the buttons from the settings menu;
            var ppg = GameObject.Find("PaletteGrid");
            foreach (PaletteRenderer pr in ppg.GetComponentsInChildren<PaletteRenderer>())
            {

                if (pr.GetName() == ppp)
                {
                    lookAndFeel = pr.GetLookAndFeel();
                    AssignPaletteColors();
                }
            }
        }
    }
}
