using UnityEngine;
using System.Collections;

public class PalletObject : IObject
{
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetLightBlock(0.3f);
    }
}
