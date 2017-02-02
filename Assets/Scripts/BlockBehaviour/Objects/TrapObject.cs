using UnityEngine;
using System.Collections;

public class TrapObject : IObject
{
    private bool _isArmed = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.DISARM_TRAP;

    protected override void OnBlockAdd()
    {
        base.OnBlockAdd();
        _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.AddPotentialAction(e_Player.TRAPER, ActionManager.e_Action.PICKUP_TRAP);
    }

    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.RemovePotentialAction(e_Player.TRAPER, ActionManager.e_Action.PICKUP_TRAP);
    }
}
