using UnityEngine;
using System.Collections;

public class TrapObject : IObject
{
    private bool _isArmed = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.DISARM_TRAP;
    private Sprite _armedTrap;
    private Sprite _disarmedTrap;

    protected override void Awake()
    {
        base.Awake();
        _bCtrl.OnObjectAdded += DetectPlayers;
        _armedTrap = Resources.Load("Trap", typeof(Sprite)) as Sprite;
        _disarmedTrap = Resources.Load("Trap_2", typeof(Sprite)) as Sprite;
    }
    public void ArmTrap()
    {
        _isArmed = !_isArmed;
        if (_isArmed)
        {
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.DISARM_TRAP;
            _sprite.sprite = _armedTrap;
            _bCtrl.OnObjectAdded += DetectPlayers;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        }
        else
        {
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.ARM_TRAP;
            _sprite.sprite = _disarmedTrap;
            _bCtrl.OnObjectAdded -= DetectPlayers;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        }
    }

    public void DetectPlayers()
    {
        ControllableEntity ent = _bCtrl.GetComponentInChildren<ControllableEntity>();
        if (ent != null)
        {
            // SOUND
            ent.MP_Current = 0;
            ArmTrap();
        }
    }

    protected override void OnBlockAdd()
    {
        _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.AddPotentialAction(e_Player.TRAPER, ActionManager.e_Action.PICKUP_TRAP);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.RemovePotentialAction(e_Player.TRAPER, ActionManager.e_Action.PICKUP_TRAP);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
