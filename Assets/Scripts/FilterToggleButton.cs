using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterToggleButton : MonoBehaviour
{
    [SerializeField]
    BoolReference FilterReference = null;

    Button myButton;

    LookAndFeelObject buttonLook;

    ColorBlock activeColors;
    ColorBlock offColors;

    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClick);

        buttonLook = GameObject.FindObjectOfType<ControlLookAndFeel>().lookAndFeel;
        activeColors = GetComponent<Button>().colors;
        offColors = GetComponent<Button>().colors;

        activeColors.selectedColor = buttonLook.Positive;
        activeColors.normalColor = buttonLook.Positive;
        activeColors.pressedColor = buttonLook.Positive;
        activeColors.highlightedColor = buttonLook.Positive;
        activeColors.disabledColor = buttonLook.Positive;

        offColors.selectedColor = buttonLook.Negative;
        offColors.normalColor = buttonLook.Negative;
        offColors.pressedColor = buttonLook.Negative;
        offColors.highlightedColor = buttonLook.Negative;
        offColors.disabledColor = buttonLook.Negative;
    }

    private void Update()
    {

        if (FilterReference.Value)
        {
            GetComponent<Image>().color = buttonLook.Positive;
            myButton.colors = activeColors;
        }
        else
        {
            GetComponent<Image>().color = buttonLook.Negative;
            myButton.colors = offColors;
        }

    }

    void OnClick()
    {
        ToggleFilter(FilterReference);
    }

    void ToggleFilter(BoolReference b00l)
    {
        if (b00l.Value)
            b00l.Value = false;
        else
            b00l.Value = true;
    }
}
