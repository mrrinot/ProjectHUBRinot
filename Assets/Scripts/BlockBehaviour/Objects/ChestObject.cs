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
        base.OnBlockAdd();
        _bCtrl.AddPotentialAction(e_Player.PLAYER, ActionManager.e_Action.OPEN_CHEST);
    }

    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, ActionManager.e_Action.OPEN_CHEST);
    }
}
