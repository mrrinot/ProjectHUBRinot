using UnityEngine;
using System.Collections;

public class GrassObject : IObject
{
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetLightBlock(0.5f);
    }
}
