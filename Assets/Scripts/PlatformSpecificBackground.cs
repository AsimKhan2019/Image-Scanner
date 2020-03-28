using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlatformSpecificBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        GetComponent<ARCameraBackground>().useCustomMaterial=true;
        GetComponent<ARCameraBackground>().customMaterial=Resources.Load("CustomARBackground") as Material;
#endif
#if UNITY_IOS
        GetComponent<ARCameraBackground>().customMaterial = null;
        GetComponent<ARCameraBackground>().useCustomMaterial = false;

#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
