using UnityEngine;
using System.Collections;

public class ExitObject : IObject
{
    protected override void Awake()
    {
        base.Awake();
        _bCtrl.SetWalkableAll(false);
        _bCtrl.AddPotentialAction(ActionManager.e_Action.USE_EXIT);
    }

}
