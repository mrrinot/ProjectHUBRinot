using UnityEngine;
using System.Collections;

public class PalletObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.PALLET);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.PALLET);
    }

}
