using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObject : IObject
{
    private float _visionRange = 5.0f;
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
