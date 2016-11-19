using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

    private BlockController _bCtrl;

    void Awake()
    {
        _bCtrl = GetComponentInParent<BlockController>();
        _bCtrl.AddObject(e_Object.KILLER);
    }


    void OnDestroy()
    {
        _bCtrl.RemoveObject(e_Object.KILLER);
    }
}
