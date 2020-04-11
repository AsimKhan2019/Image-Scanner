using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeView : MonoBehaviour
{
    [SerializeField]
    Canvas[] Views;

    public void ChangeViewViaIndex(int view)
    {
        foreach (Canvas go in Views)
        {
            go.enabled = false;
        }
        Views[view].enabled = true;
    }
}
