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
    private e_PalletState _state = e_PalletState.UNUSED;
    private Sprite _usedPalletSprite;

    protected override void Awake()
    {
        base.Awake();
        _usedPalletSprite = Resources.Load("Pallet_2", typeof(Sprite)) as Sprite;
    }

    public void DestroyPallet()
    {
        _state = e_PalletState.DESTROYED;
        _bCtrl.SetWalkableAll(true);
        _bCtrl.RemovePotentialActionAll(ActionManager.e_Action.DESTROY_PALLET);
        OnBlockRemove();
        GameObject.Destroy(this.gameObject);
    }

    public void BlockPallet()
    {
        _state = e_PalletState.BLOCK;
        _sprite.sprite = _usedPalletSprite;
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        _potAct = ActionManager.e_Action.DESTROY_PALLET;
        _bCtrl.AddPotentialActionAll(_potAct);
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.SetWalkableAll(false);
        _bCtrl.SetWalkable(e_Player.PLAYER, true);
    }

    protected override void OnBlockAdd()
    {
        _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.AddPotentialActionAll(ActionManager.e_Action.DESTROY_PALLET);
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, ActionManager.e_Action.DESTROY_PALLET);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
    protected override void OnBlockRemove()
    {
        if (_state == e_PalletState.UNUSED)
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        else if (_state == e_PalletState.BLOCK)
            _bCtrl.RemovePotentialActionAll(_potAct);
        _bCtrl.RemovePotentialActionAll(ActionManager.e_Action.DESTROY_PALLET);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
