using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PaletteRenderer : MonoBehaviour
{
    string PaletteName;
    [SerializeField]
    LookAndFeelObject Palette;

    Transform horizontal;

    private void Awake()
    {
        horizontal = transform.GetChild(0);
        PaletteName = Palette.paletteName;

        GetComponentInChildren<TextMeshProUGUI>().text = name;

        horizontal.GetChild(0).GetComponent<Image>().color = Palette.Negative;
        horizontal.GetChild(1).GetComponent<Image>().color = Palette.UI_Color0;
        horizontal.GetChild(2).GetComponent<Image>().color = Palette.UI_Color1;
        horizontal.GetChild(3).GetComponent<Image>().color = Palette.UI_Color2;
        horizontal.GetChild(4).GetComponent<Image>().color = Palette.Positive;

    }

    private void Update()
    {
        horizontal = transform.GetChild(0);
        PaletteName = Palette.paletteName;
        if (!Application.isPlaying)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = PaletteName;

            horizontal.GetChild(0).GetComponent<Image>().color = Palette.Negative;
            horizontal.GetChild(1).GetComponent<Image>().color = Palette.UI_Color0;
            horizontal.GetChild(2).GetComponent<Image>().color = Palette.UI_Color1;
            horizontal.GetChild(3).GetComponent<Image>().color = Palette.UI_Color2;
            horizontal.GetChild(4).GetComponent<Image>().color = Palette.Positive;

        }
    }

    public void onclick()
    {
        GameObject.Find("AssignLookAndFeel").GetComponent<ControlLookAndFeel>().lookAndFeel = Palette;
        GameObject.Find("AssignLookAndFeel").GetComponent<ControlLookAndFeel>().AssignPaletteColors();

        PlayerPrefs.SetString("LookAndFeel", PaletteName);
    }

    public string GetName()
    {
        return PaletteName;
    }

    public LookAndFeelObject GetLookAndFeel()
    {
        return Palette;
    }

}
