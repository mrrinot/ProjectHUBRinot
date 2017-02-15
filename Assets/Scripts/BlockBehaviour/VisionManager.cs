using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisionManager : MonoBehaviour
{
    private MapHolder _map;
    private TurnManager _tManager;
    private List<BlockController> _lightedBlocks = new List<BlockController>();
    private List<BlockController> _seenBlocks = new List<BlockController>();
    private List<LightingSource> _lightSourcesList = new List<LightingSource>();

    void Start()
    {
        _map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().GetMap();
        _tManager = GetComponent<TurnManager>();
        List<BlockController> lightBlocks = _map.GetAllBlocksWith(new List<e_Object>() { e_Object.PLAYER, e_Object.TORCH });
        foreach (BlockController block in lightBlocks)
            _lightSourcesList.Add(block.GetComponentInChildren<LightingSource>());
        UpdateLightingAndVision();
    }

    public void UpdateLightingAndVision()
    {
        // CLEAR LIGHTED BLOCKS
        foreach (BlockController block in _lightedBlocks)
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
        _lightedBlocks.Clear();
        // CLEAR SEEN BLOCKS
        foreach (BlockController block in _seenBlocks)
        {
            if (block == null)
            {
                Debug.LogError("NULL SEEN");
                continue;
            }
            block.SetVisibleAll(false);
        }
        _seenBlocks.Clear();
        // PROCESS VIISON FOR ENTITIES (PLAYER AND ENNEMIES, NOT TORCHES)
        List<ControllableEntity> players = _tManager.GetAllEnnemies();
        foreach (ControllableEntity ent in players)
        {
            BlockController bCtrl = ent.GetComponentInParent<BlockController>();
            _seenBlocks.Add(bCtrl);
            bCtrl.SetVisible(ent.PID, true);
            for (float i = -1.0f; i <= 1.0f; i += 0.2f)
                for (float j = -1.0f; j <= 1.0f; j += 0.2f)
                {
                    RaycastHit[] hits = Physics.RaycastAll(ent.transform.position, new Vector3(i, j, ent.transform.position.z), ent.VisionRange);
                    float alpha = 1f;
                    foreach (RaycastHit hit in hits)
                    {
                        BlockController block = hit.collider.GetComponent<BlockController>();
                        if (_seenBlocks.Contains(block))
                            _seenBlocks.Add(block);
                        block.SetVisible(ent.PID, true);
                        alpha -= (block.GetLightBlock() + Mathf.Sqrt(hit.distance) / Mathf.Pow(ent.VisionRange, 2));
                        if (alpha <= 0f)
                            break;
                    }
                }
        }
        // PROCESS LIGHT SOURCES (PLAYER AND TORCHES)
        foreach (LightingSource source in _lightSourcesList)
        {
            if (!source.IsLit)
                continue;
            BlockController bCtrl = source.GetComponentInParent<BlockController>();
            _lightedBlocks.Add(bCtrl);
            bCtrl.SetVisible(e_Player.PLAYER, true);
            bCtrl.Alpha = 1f;
            for (float i = -1.0f; i <= 1.0f; i += 0.2f)
                for (float j = -1.0f; j <= 1.0f; j += 0.2f)
                {
                    //Debug.Log("I/J = " + i + "/" + j);
                    RaycastHit[] hits = Physics.RaycastAll(source.transform.parent.position, new Vector3(i, j, source.transform.parent.position.z), source.LightRange);
                    //Debug.DrawRay(source.transform.parent.position, new Vector3(i, j, source.transform.parent.position.z), Color.white, 60.0f);
                    float alpha = 1f;
                    foreach (RaycastHit hit in hits)
                    {
                        BlockController block = hit.collider.GetComponent<BlockController>();
                        if (_lightedBlocks.Contains(block) == false)
                            _lightedBlocks.Add(block);
                        block.SetVisible(e_Player.PLAYER, true);
                        //Debug.Log("ALPHA = " + alpha);
                        //Debug.Log("distance / range " + hit.distance + "/" + source.LightRange + " MINUS " + Mathf.Sqrt(hit.distance) / Mathf.Pow(source.LightRange, 2));
                        if (block.Alpha < alpha)
                            block.Alpha = alpha;
                        alpha -= (block.GetLightBlock() + Mathf.Sqrt(hit.distance) / Mathf.Pow(source.LightRange, 2));
                        if ((block.GetLightBlock() + Mathf.Sqrt(hit.distance) / Mathf.Pow(source.LightRange, 2)) < 0f)
                        {
                            Debug.Log("distance / range " + hit.distance + "/" + source.LightRange + " MINUS " + Mathf.Sqrt(hit.distance) / Mathf.Pow(source.LightRange, 2));
                            Debug.Log(block.name + block.transform.position);
                        }
                        if (alpha <= 0f)
                            break;
                    }
                }
        }
        // PROCESS FALLOUT LIGHTS
        foreach (BlockController block in _lightedBlocks)
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
