using UnityEngine;
using System.Collections;

public class WallObject : MonoBehaviour {

    private BlockController _bCtrl;
    [SerializeField]
    private bool isEndWall;

	void Awake ()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.WALL);
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _bCtrl.SetWalkable(val, false);
	}
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.WALL);
    }

}
