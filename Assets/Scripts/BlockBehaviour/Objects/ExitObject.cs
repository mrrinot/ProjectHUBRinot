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
        _bCtrl.AddPotentialAction(e_Player.PLAYER, ActionManager.e_Action.USE_EXIT);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, ActionManager.e_Action.USE_EXIT);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
