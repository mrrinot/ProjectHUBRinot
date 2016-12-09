using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightingSource : MonoBehaviour
{
    private List<BlockController> _seenBlocks;
    private MapGenerator _gen;
    private BlockController _bCtrl;
    private float _baseLighting;
    private Light _lighting;

    void Awake()
    {
        _lighting = GetComponentInChildren<Light>();
        _baseLighting = _lighting.intensity;
        _bCtrl = GetComponentInParent<BlockController>();
        _gen = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        _seenBlocks = new List<BlockController>();
        StartCoroutine("flickerLightning");
    }


    IEnumerator flickerLightning()
    {
        while (true)
        {
            _lighting.intensity = UnityEngine.Random.Range((int)((_baseLighting - 0.1f) * 100), (int)((_baseLighting + 0.1f) * 100)) / 100f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateVision(float range)
    {
        foreach (BlockController block in _seenBlocks)
        {
            block.SetVisible(e_Player.PLAYER, false);
            block.Alpha = 0f;
        }
        _seenBlocks.Add(_bCtrl);
        _bCtrl.SetVisible(e_Player.PLAYER, true);
        _bCtrl.Alpha = 1f;
        for (float i = -1.0f; i <= 1.0f; i += 0.2f)
            for (float j = -1.0f; j <= 1.0f; j += 0.2f)
            {
                //Debug.Log("I/J = " + i + "/" + j);
                RaycastHit[] hits = Physics.RaycastAll(transform.parent.position, new Vector3(i, j, transform.parent.position.z), range);
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
                        block.Alpha = alpha;
                    alpha -= (block.GetLightBlock() + Mathf.Sqrt(hit.distance) / Mathf.Pow(range, 2));
                    if (alpha <= 0f)
                        break;
                }
            }
        MapHolder map = _gen.GetMap();
        foreach (BlockController block in _seenBlocks)
        {
            BlockController adj;
            if ((adj = map.GetBlock(block.X + 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.Alpha += block.Alpha / 3f;
            }
            if ((adj = map.GetBlock(block.X - 1, block.Y)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.Alpha += block.Alpha / 3f;
            }
            if ((adj = map.GetBlock(block.X, block.Y + 1)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.Alpha += block.Alpha / 3f;
            }
            if ((adj = map.GetBlock(block.X, block.Y - 1)) != null && adj.IsVisible(e_Player.PLAYER) == false && block.HasObject(e_Object.WALL) == false)
            {
                adj.Alpha += block.Alpha / 3f;
            }
        }
    }
}
