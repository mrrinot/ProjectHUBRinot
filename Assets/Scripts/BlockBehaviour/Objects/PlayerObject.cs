using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObject : IObject
{

    protected override void Awake()
    {
        base.Awake();
        _bCtrl.AddPotentialAction(ActionManager.e_Action.DAMAGE_PLAYER);
    }
}