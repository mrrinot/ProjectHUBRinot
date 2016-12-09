using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{
    private float _visionRange = 3.0f;
    private bool _lit = true;
    private LightingSource _source;

    protected override void Awake()
    {
        base.Awake();
        _source = GetComponent<LightingSource>();
    }

    void Start()
    {
        _source.UpdateVision(_visionRange);
    }
}
