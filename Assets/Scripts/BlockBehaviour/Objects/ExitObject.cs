using UnityEngine;
using System.Collections;

public class ExitObject : IObject
{
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetWalkableAll(false);
    }
    protected override void OnBlockAdd()
    {
        base.OnBlockAdd();
        _bCtrl.AddPotentialAction(e_Player.PLAYER, ActionManager.e_Action.USE_EXIT);
    }

    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, ActionManager.e_Action.USE_EXIT);
    }
}
