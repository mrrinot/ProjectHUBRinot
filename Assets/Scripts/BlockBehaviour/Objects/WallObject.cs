using UnityEngine;
using System.Collections;

public class WallObject : IObject
{

    [SerializeField]
    private bool isEndWall;

	protected override void Awake ()
    {
        base.Awake();
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _bCtrl.SetWalkable(val, false);
        _bCtrl.SetLightBlock(1f);
	}
}
