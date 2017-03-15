using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaserEntity : ControllableEntity
{
    protected override void Awake()
    {
        base.Awake();
        OnSoundHeard += OnSoundHeardHeuristicsChanged;
        OnBlockSeen += OnBlockSeenHeuristicChanged;
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        updateHeuristicMap();
        _tManager.EndTurn(this);
    }

    private void updateHeuristicMap()
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

    private BlockController getBestTarget()
    {
        List<BlockController> targets = new List<BlockController>();
        int height = _map.GetMapHeight();
        int width = _map.GetMapWidth();
        BlockController[][] map = _map.GetRawMap();
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                BlockController ctrl = map[i][j];
                if (ctrl != null)
                {
                    if (targets.Count == 0)
                        targets.Add(ctrl);
                    else if (targets[0].GetHeuristic(_pID) < ctrl.GetHeuristic(_pID)
                        || (targets[0].GetHeuristic(_pID) == ctrl.GetHeuristic(_pID) && Mathf.Ceil(Vector3.Distance(transform.parent.position, targets[0].transform.position)) < Mathf.Ceil(Vector3.Distance(transform.parent.position, ctrl.transform.position))))
                    {
                        targets.Clear();
                        targets.Add(ctrl);
                    }
                    else if ((targets[0].GetHeuristic(_pID) == ctrl.GetHeuristic(_pID) && Mathf.Ceil(Vector3.Distance(transform.parent.position, targets[0].transform.position)) == Mathf.Ceil(Vector3.Distance(transform.parent.position, ctrl.transform.position))))
                        targets.Add(ctrl);
                }
            }
        return targets[UnityEngine.Random.Range(0, targets.Count)];
    }

    private void OnSoundHeardHeuristicsChanged(ControllableEntity src, BlockController from, float power)
    {
        if (src != this)
        {
            Vector3 dir = Vector3.Normalize(from.transform.position - this.transform.parent.position);
            List<BlockController> soundAffectedBlockList = new List<BlockController>();
            //Debug.Log("ORIGIN = "+from.transform.position);
            int baseHeur = 15;
            for (float angle = -45f; angle <= 45f; angle += 15f)
            {
                //Debug.DrawRay(this.transform.parent.position, Quaternion.Euler(0, 0, angle) * dir * power, Color.red, 50f);
                RaycastHit[] hits = Physics.RaycastAll(this.transform.parent.position, Quaternion.Euler(0, 0, angle) * dir, power);
                foreach (RaycastHit hit in hits)
                {
                    BlockController block = hit.collider.GetComponent<BlockController>();
                    if (block != null && soundAffectedBlockList.Contains(block) == false)
                    {
                        soundAffectedBlockList.Add(block);
                        float dist = 1 + Vector3.Distance(from.transform.position, block.transform.position);
                        //Debug.Log("block = "+block.transform.position + " added "+ (baseHeur / dist));
                        block.AddHeuristics(_pID, baseHeur / dist);
                    }
                }
            }
        }
    }

    private void OnBlockSeenHeuristicChanged(BlockController block)
    {
        if (block.HasObject(e_Object.PLAYER))
        {
            float hiddenChances = 100 * block.GetAlpha(_pID);
            if (block.HasObject(e_Object.GRASS))
                hiddenChances *= 0.5f;
            hiddenChances = Mathf.Clamp(hiddenChances - 20f, 0, 100);
            Debug.Log("PLAYER LAST SEEN HERE " + block.transform.position + " CHANCES OF BEING SEEN "+hiddenChances);
            if (UnityEngine.Random.Range(0, 100) < hiddenChances)
            {
                Debug.Log("TROUVE !!!");
            }
        }
    }
}
