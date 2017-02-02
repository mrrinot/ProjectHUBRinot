using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObject : IObject
{

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnBlockAdd()
    {
        base.OnBlockAdd();
        _bCtrl.AddPotentialActionAll(ActionManager.e_Action.DAMAGE_PLAYER);
    }

    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        _bCtrl.RemovePotentialActionAll(ActionManager.e_Action.DAMAGE_PLAYER);
    }
}