using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{
    private bool _isLit = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.UNLIT_TORCH;
    private LightingSource _source;
    private Sprite _litTorchSprite;
    private Sprite _unlitTorchSprite;

    protected override void Awake()
    {
        base.Awake();
        _litTorchSprite = Resources.Load("Torch", typeof(Sprite)) as Sprite;
        _unlitTorchSprite = Resources.Load("Torch_2", typeof(Sprite)) as Sprite;
        _source = GetComponent<LightingSource>();
        _bCtrl.SetWalkableAll(false);
    }

    public void LitTorch()
    {
        _isLit = !_isLit;
        _source.IsLit = _isLit;
        if (_isLit == true)
        {
            _sprite.sprite = _litTorchSprite;
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.UNLIT_TORCH;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        }
        else
        {
            _sprite.sprite = _unlitTorchSprite;
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
