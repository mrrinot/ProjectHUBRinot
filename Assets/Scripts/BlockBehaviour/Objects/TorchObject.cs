using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{

    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetWalkable(e_Player.PLAYER, false);
    }

    void Start()
    {
    }
}
