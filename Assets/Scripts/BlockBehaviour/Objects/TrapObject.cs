using UnityEngine;
using System.Collections;

public class TrapObject : IObject
{
    private bool _isArmed = true;
    private ActionManager.e_Action _potAct = ActionManager.e_Action.DISARM_TRAP;
    private SoundManager _soundManager;
    private Sprite _armedTrap;
    private Sprite _disarmedTrap;

    protected override void Awake()
    {
        base.Awake();
        _bCtrl.OnObjectAdded += DetectPlayers;
        _armedTrap = Resources.Load("Trap", typeof(Sprite)) as Sprite;
        _disarmedTrap = Resources.Load("Trap_2", typeof(Sprite)) as Sprite;
        _soundManager = GameObject.Find("TurnManager").GetComponent<SoundManager>();
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
            _minAlpha = 0.95f;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
            _bCtrl.AddMovementCostAll(100);
        }
        else
        {
            _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
            _potAct = ActionManager.e_Action.ARM_TRAP;
            _sprite.sprite = _disarmedTrap;
            _minAlpha = 0.5f;
            _bCtrl.OnObjectAdded -= DetectPlayers;
            _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
            _bCtrl.AddMovementCostAll(-100);
        }
    }

    public void DetectPlayers()
    {
        ControllableEntity ent = _bCtrl.GetComponentInChildren<ControllableEntity>();
        if (ent != null)
        {
            _soundManager.ProduceSound(ent, _bCtrl, 6);
            ent.MP_Current = 0;
            ArmTrap();
        }
    }

    protected override void OnBlockAdd()
    {
        _bCtrl.AddPotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.AddPotentialAction(e_Player.TRAPPER, ActionManager.e_Action.PICKUP_TRAP);
        _bCtrl.AddMovementCostAll(100);
        base.OnBlockAdd(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected override void OnBlockRemove()
    {
        _bCtrl.RemovePotentialAction(e_Player.PLAYER, _potAct);
        _bCtrl.RemovePotentialAction(e_Player.TRAPPER, ActionManager.e_Action.PICKUP_TRAP);
        _bCtrl.AddMovementCostAll(-100);
        base.OnBlockRemove(); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }
}
