using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightingManager : MonoBehaviour
{
    private MapHolder _map;
    private List<BlockController> _seenBlocks = new List<BlockController>();
    private List<LightingSource> _lightSourcesList = new List<LightingSource>();

    void Start()
    {
        _map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().GetMap();
        List<BlockController> lightBlocks = _map.GetAllBlocksWith(new List<e_Object>() { e_Object.PLAYER, e_Object.TORCH });
        foreach (BlockController block in lightBlocks)
            _lightSourcesList.Add(block.GetComponentInChildren<LightingSource>());
        UpdateLighting();
    }

    public void UpdateLighting()
    {
        foreach (BlockController block in _seenBlocks)
        {
            if (block == null)
            {
                Debug.LogError("NULL");
                continue;
            }
            block.SetVisible(e_Player.PLAYER, false);
            block.Alpha = 0f;
            BlockController adj;
            if ((adj = _map.GetBlock(block.X + 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false)
                adj.Alpha = 0f;
            if ((adj = _map.GetBlock(block.X - 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false)
                adj.Alpha = 0f;
            if ((adj = _map.GetBlock(block.X, block.Y + 1)) != null && adj.IsVisible(e_Player.PLAYER) == false)
                adj.Alpha = 0f;
            if ((adj = _map.GetBlock(block.X, block.Y - 1)) != null && adj.IsVisible(e_Player.PLAYER) == false)
                adj.Alpha = 0f;
        }
        _seenBlocks.Clear();
        foreach (LightingSource source in _lightSourcesList)
        {
            if (!source.IsLit)
                continue;
            BlockController bCtrl = source.GetComponentInParent<BlockController>();
            _seenBlocks.Add(bCtrl);
            bCtrl.SetVisible(e_Player.PLAYER, true);
            bCtrl.Alpha = 1f;
            for (float i = -1.0f; i <= 1.0f; i += 0.2f)
                for (float j = -1.0f; j <= 1.0f; j += 0.2f)
                {
                    //Debug.Log("I/J = " + i + "/" + j);
                    RaycastHit[] hits = Physics.RaycastAll(source.transform.parent.position, new Vector3(i, j, source.transform.parent.position.z), source.LightRange);
                    //Debug.DrawRay(transform.parent.position, new Vector3(i, j, transform.parent.position.z), Color.white, 60.0f);
                    float alpha = 1f;
                    foreach (RaycastHit hit in hits)
                    {
                        BlockController block = hit.collider.GetComponent<BlockController>();
                        if (_seenBlocks.Contains(block) == false)
                            _seenBlocks.Add(block);
                        block.SetVisible(e_Player.PLAYER, true);
                        //Debug.Log("ALPHA = " + alpha);
                        //Debug.Log("distance / range " + hit.distance + "/" + range + " MINUS " + Mathf.Sqrt(hit.distance) / Mathf.Pow(range, 2));
                        if (block.Alpha < alpha)
                        {
                            block.Alpha = alpha;
                        }
                        alpha -= (block.GetLightBlock() + Mathf.Sqrt(hit.distance) / Mathf.Pow(source.LightRange, 2));
                        if (alpha <= 0f)
                            break;
                    }
                }
        }
        foreach (BlockController block in _seenBlocks)
        {
            BlockController adj;
            if ((adj = _map.GetBlock(block.X + 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false)
            {
                adj.Alpha += Mathf.Clamp((block.Alpha / 3f) - block.GetLightBlock(), 0f, 1.0f);
            }
            if ((adj = _map.GetBlock(block.X - 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false)
            {
                adj.Alpha += Mathf.Clamp((block.Alpha / 3f) - block.GetLightBlock(), 0f, 1.0f);
            }
            if ((adj = _map.GetBlock(block.X, block.Y + 1)) != null && adj.IsVisible(e_Player.PLAYER) == false)
            {
                adj.Alpha += Mathf.Clamp((block.Alpha / 3f) - block.GetLightBlock(), 0f, 1.0f);
            }
            if ((adj = _map.GetBlock(block.X, block.Y - 1)) != null && adj.IsVisible(e_Player.PLAYER) == false)
            {
                adj.Alpha += Mathf.Clamp((block.Alpha / 3f) - block.GetLightBlock(), 0f, 1.0f);
            }
        }
    }
}
