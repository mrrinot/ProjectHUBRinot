using UnityEngine;
using System.Collections;

public class TorchObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.TORCH);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.TORCH);
    }

}
