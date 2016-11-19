using UnityEngine;
using System.Collections;

public class PlayerObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.PLAYER);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.PLAYER);
    }

}
