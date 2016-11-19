using UnityEngine;
using System.Collections;

public class TrapObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.TRAP);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.TRAP);
    }

}
