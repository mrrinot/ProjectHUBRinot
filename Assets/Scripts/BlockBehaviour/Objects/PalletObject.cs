using UnityEngine;
using System.Collections;

public class PalletObject : IObject
{
    public enum e_PalletState : byte
    {
        UNUSED = 0,
        BLOCK,
        DESTROYED
    }

    private ActionManager.e_Action _potAct = ActionManager.e_Action.ARM_PALLET;
    private e_PalletState _state;

    protected override void Awake()
    {
        base.Awake();
        _state = e_PalletState.UNUSED;
    }

    protected override void OnBlockAdd()
    {
        base.OnBlockAdd();
        _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
    }
    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        if (_state == e_PalletState.UNUSED)
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        else if (_state == e_PalletState.BLOCK)
            _bCtrl.RemovePotentialActionAll(_potAct);
    }
}
