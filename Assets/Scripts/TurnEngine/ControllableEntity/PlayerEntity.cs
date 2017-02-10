using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEntity : ControllableEntity
{
    private PlayerGameUIController _uiController;

    protected override void Awake()
    {
        base.Awake();
        _uiController = GetComponent<PlayerGameUIController>();
    }
    
    public void OnEndTurn()
    {
        _tManager.EndTurn(this);
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        _uiController.OnStartTurn();
    }

    public override void OnHit()
    {
    }
}
