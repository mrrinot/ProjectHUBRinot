using UnityEngine;
using System.Collections;

public class ChestObject : IObject
{

    private bool _isOpen = false;
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetWalkableAll(false);
    }

    protected override void OnBlockAdd()
    {
        _bCtrl.AddPotentialAction(e_Player.PLAYER, ActionManager.e_Action.OPEN_CHEST);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, ActionManager.e_Action.OPEN_CHEST);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
