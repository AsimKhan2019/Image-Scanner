using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue = 0;
    public FloatVariable Variable = null;

    public FloatReference()
    {

    }
    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

      public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }

}
