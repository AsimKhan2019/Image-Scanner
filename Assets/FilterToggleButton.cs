using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterToggleButton : MonoBehaviour
{
    [SerializeField]
    BoolReference FilterReference;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(OnClick);
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
