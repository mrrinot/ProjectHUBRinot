using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{
    private bool _isLit = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.UNLIT_TORCH;
    private LightingSource _source;
    private SpriteRenderer _renderer;
    protected override void Awake()
    {
        base.Awake();
        _source = GetComponent<LightingSource>();
        _renderer = GetComponent<SpriteRenderer>();
        _bCtrl.SetWalkableAll(false);
    }

    public void LitTorch()
    {
        _isLit = !_isLit;
        _source.IsLit = _isLit;
        if (_isLit == true)
        {
            _renderer.color = Color.white; // TODO CHANGE SPRITE INSTEAD OF COLOR
            _potAct = ActionManager.e_Action.UNLIT_TORCH;
        }
        else
        {
            _renderer.color = Color.black;
            _potAct = ActionManager.e_Action.LIT_TORCH;
        }
    }

    protected override void OnBlockAdd()
    {
        base.OnBlockAdd();
        _bCtrl.AddPotentialActionAll(_potAct);
    }

    protected override void OnBlockRemove()
    {
        base.OnBlockRemove();
        _bCtrl.RemovePotentialActionAll(_potAct);
    }
}
