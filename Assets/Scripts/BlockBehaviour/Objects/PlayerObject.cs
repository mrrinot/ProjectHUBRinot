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
        _bCtrl.AddPotentialActionAll(ActionManager.e_Action.DAMAGE_PLAYER);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialActionAll(ActionManager.e_Action.DAMAGE_PLAYER);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}