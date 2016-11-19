using UnityEngine;
using System.Collections;

public class ChestObject : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.CHEST);
    }
    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.CHEST);
    }

}

