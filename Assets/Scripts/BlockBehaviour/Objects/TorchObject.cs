using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{
    private bool _isLit = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.UNLIT_TORCH;
    private LightingSource _source;
    protected override void Awake()
    {
        base.Awake();
        _source = GetComponent<LightingSource>();
        _bCtrl.SetWalkableAll(false);
    }

    public void LitTorch()
    {
        _isLit = !_isLit;
        _source.IsLit = _isLit;
        if (_isLit == true)
        {
            _sprite.color = Color.white; // TODO CHANGE SPRITE INSTEAD OF COLOR
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.UNLIT_TORCH;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        }
        else
        {
            _sprite.color = Color.black;
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.LIT_TORCH;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        }
    }

    protected override void OnBlockAdd()
    {
        _bCtrl.AddPotentialActionAll(_potAct);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialActionAll(_potAct);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
