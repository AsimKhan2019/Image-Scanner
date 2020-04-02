using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetFloatVariable : MonoBehaviour
{

    [SerializeField]
    BoolReference FilterActive = null;

    [SerializeField]

    private void Start()
    {
        contrastBeta.SetValue(0);
        GetComponent<Slider>().value = .5f;
    }
    //  [SerializeField]
    //  FloatReference contrastAlpha;
    [SerializeField]
    FloatVariable contrastBeta = null;
    public void OnValueChanged()
    {
        contrastBeta.SetValue(Mathf.Lerp(-150, 150, GetComponent<Slider>().value));
    }

    private void Update()
    {
        if (!FilterActive.Value)
        {
            HideSlider(true);
        }
    }

    public void HideSlider(bool hidden)
    {
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            i.enabled = !hidden;
        }
    }

    public void ToggleSlider()
    {
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            if (FilterActive.Value)
                i.enabled = !i.enabled;
        }
    }
}
