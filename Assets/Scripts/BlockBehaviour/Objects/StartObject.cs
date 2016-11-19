using UnityEngine;
using System.Collections;

public class StartObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.START);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.START);
    }

}
