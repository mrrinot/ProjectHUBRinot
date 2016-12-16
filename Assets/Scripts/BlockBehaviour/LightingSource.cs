using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightingSource : MonoBehaviour
{
    [SerializeField]
    private float _lightRange;

    private bool _isLit = true;

    public float LightRange
    {
        get { return _lightRange; }
        set { _lightRange = value; }
    }

    public bool IsLit
    {
        get { return _isLit; }
        set { _isLit = value; }
    }
}
