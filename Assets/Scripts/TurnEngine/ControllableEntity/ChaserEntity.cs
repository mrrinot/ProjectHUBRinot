using UnityEngine;
using System.Collections;

public class ChaserEntity : ControllableEntity
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        _tManager.EndTurn(this);
    }

    public override void OnHit()
    {
    }
}
