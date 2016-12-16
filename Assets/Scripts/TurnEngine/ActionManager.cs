using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManager : MonoBehaviour
{
    private LightingManager _lightingManager;
    private MapHolder _map;

    void Start()
    {
        _map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().GetMap();
        _lightingManager = GetComponent<LightingManager>();

    }
    public bool Move(ControllableEntity ent, Vector3 moveTo)
    {
        BlockController dest = _map.GetBlock((int)moveTo.x, (int)moveTo.y);
        BlockController src = ent.GetComponentInParent<BlockController>();
        if (dest.IsWalkable(ent.PID) == true && ent.MP_Current > 0)
        {
            e_Object type = ent.GetComponent<IObject>().Type;
            src.RemoveObject(type);
            dest.AddObject(type);
            ent.MP_Current--;
            ent.transform.SetParent(dest.transform);
            ent.transform.localPosition = Vector3.zero;
            _lightingManager.UpdateLighting();
            return true;
        }
        return false;
    }
}
