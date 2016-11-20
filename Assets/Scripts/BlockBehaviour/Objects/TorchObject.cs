using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchObject : IObject
{

    private float _visionRange = 3.0f;
    private List<BlockController> _seenBlocks;
    private MapGenerator _gen;


    protected override void Awake()
    {
        base.Awake();
        _gen = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        _seenBlocks = new List<BlockController>();
    }

    void Start()
    {
        checkVision();
    }

    void checkVision()
    {
        foreach (BlockController block in _seenBlocks)
            block.SetVisible(e_Player.PLAYER, false);
        _seenBlocks.Add(_bCtrl);
        _bCtrl.SetVisible(e_Player.PLAYER, true);
        _bCtrl.SetVisionAlpha(1f);
        for (float i = -1.0f; i <= 1.0f; i += 0.2f)
            for (float j = -1.0f; j <= 1.0f; j += 0.2f)
            {
                //Debug.Log("I/J = "+i +"/"+j);
                RaycastHit[] hits = Physics.RaycastAll(transform.parent.position, new Vector3(i, j, transform.parent.position.z), _visionRange);
                //Debug.DrawRay(transform.parent.position, new Vector3(i, j, transform.parent.position.z), Color.white, 60.0f);
                foreach (RaycastHit hit in hits)
                {
                    BlockController block = hit.collider.GetComponent<BlockController>();
                    if (_seenBlocks.Contains(block) == false)
                        _seenBlocks.Add(block);
                    block.SetVisible(e_Player.PLAYER, true);
                    block.SetVisionAlpha(1.0f / hit.distance * _visionRange / 2.0f);
                    if (block.HasObject(e_Object.WALL))
                        break;
                }
            }
        MapHolder map = _gen.GetMap();
        foreach (BlockController block in _seenBlocks)
        {
            BlockController adj;
            if ((adj = map.GetBlock(block.X + 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.AddAlpha(0.2f);
            }
            if ((adj = map.GetBlock(block.X - 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.AddAlpha(0.2f);
            }
            if ((adj = map.GetBlock(block.X, block.Y + 1)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.AddAlpha(0.2f);
            }
            if ((adj = map.GetBlock(block.X, block.Y - 1)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.AddAlpha(0.2f);
            }
        }
    }
}
