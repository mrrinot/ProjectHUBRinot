using UnityEngine;
using System.Collections;

public class GrassObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.GRASS);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.GRASS);
    }

}
