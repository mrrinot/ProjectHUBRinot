using UnityEngine;
using System.Collections;

public class WallObject : IObject
{
    [SerializeField]
    private bool isEndWall;

	protected override void Awake ()
    {
        base.Awake();
        _bCtrl.SetWalkableAll(false);
	}
}
