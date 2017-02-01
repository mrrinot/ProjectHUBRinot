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

    private e_PalletState _state;

    protected override void Awake()
    {
        base.Awake();
        _state = e_PalletState.UNUSED;
        _bCtrl.SetLightBlock(0.3f);
        _bCtrl.AddPotentialAction(ActionManager.e_Action.ARM_PALLET);
    }
}
