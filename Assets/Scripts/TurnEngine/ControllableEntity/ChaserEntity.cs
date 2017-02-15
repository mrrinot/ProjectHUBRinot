using UnityEngine;
using System.Collections;

public class ChaserEntity : ControllableEntity
{
    protected override void Awake()
    {
        base.Awake();
        OnSoundHeard += OnSoundHeardHeuristicsChanged;
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        UpdateHeuristicMap();
        _tManager.EndTurn(this);
    }

    private void UpdateHeuristicMap()
    {
        int height = _map.GetMapHeight();
        int width = _map.GetMapWidth();
        BlockController[][] map = _map.GetRawMap();
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                BlockController ctrl = map[i][j];
                if (ctrl != null)
                {
                    if (ctrl.IsVisible(_pID) == false)
                        ctrl.AddHeuristics(_pID, 5f);
                }
            }
    }

    private void OnSoundHeardHeuristicsChanged(ControllableEntity src, BlockController from, float power)
    {
        if (src != this)
        {
            Debug.Log(Vector3.Angle(this.transform.parent.position, from.transform.position));
            Debug.Log("POWER = " + power);
            Vector3 dir = Vector3.Normalize(from.transform.position - this.transform.parent.position);
            Debug.Log("direction = " + dir);
            Debug.DrawRay(this.transform.parent.position, new Vector3(dir.x - 0.1f, dir.y + 0.1f, dir.z), Color.gray, 5.0f);
            Debug.DrawRay(this.transform.parent.position, dir, Color.cyan, 5.0f);
            Debug.DrawRay(this.transform.parent.position, new Vector3(dir.x + 0.1f, dir.y - 0.1f, dir.z), Color.gray, 5.0f);
        }
    }
}
