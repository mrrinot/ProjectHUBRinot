using UnityEngine;
using System.Collections;

public class ExitObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.EXIT);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.EXIT);
    }

}
