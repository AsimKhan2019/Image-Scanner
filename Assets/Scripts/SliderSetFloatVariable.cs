using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetFloatVariable : MonoBehaviour
{
    private void Start()
    {
        contrastBeta.SetValue(0);
        GetComponent<Slider>().value = .5f;
    }
    //  [SerializeField]
    //  FloatReference contrastAlpha;
    [SerializeField]
    FloatVariable contrastBeta;
    public void OnValueChanged()
    {
        contrastBeta.SetValue(Mathf.Lerp(-50, 50, GetComponent<Slider>().value));
    }

    public void HideSlider()
    {
        foreach (Image i in transform.GetComponentsInChildren<Image>())
        {
            if (i.enabled)
                i.enabled = false;
            else
                i.enabled = true;
        }
    }
}
