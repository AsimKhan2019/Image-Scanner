using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToggleButton : MonoBehaviour
{
    SliderSetFloatVariable SliderScript;
    // Start is called before the first frame update
    void Start()
    {
        SliderScript = GetComponentInChildren<SliderSetFloatVariable>();
        SliderScript.HideSlider(true);

        GetComponent<Button>().onClick.AddListener(SliderScript.ToggleSlider);
    }
}
