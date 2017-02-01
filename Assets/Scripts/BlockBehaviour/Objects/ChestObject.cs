using UnityEngine;
using System.Collections;

public class ChestObject : IObject
{
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetWalkableAll(false);
        _bCtrl.AddPotentialAction(ActionManager.e_Action.OPEN_CHEST);
    }

}
