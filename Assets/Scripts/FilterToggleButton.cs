using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterToggleButton : MonoBehaviour
{
    [SerializeField]
    BoolReference FilterReference = null;

    Button myButton;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        var activeColors = GetComponent<Button>().colors;
        var offColors = GetComponent<Button>().colors;

        activeColors.selectedColor = new Color(0, 255, 0, 255);
        activeColors.normalColor = new Color(0, 255, 0, 255);

        offColors.selectedColor = new Color(255, 0, 0, 255);
        offColors.normalColor = new Color(255, 0, 0, 255);

        if (FilterReference.Value)
        {
            myButton.colors = activeColors;
        }
        else
        {
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
