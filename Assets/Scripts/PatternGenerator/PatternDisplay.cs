using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PatternDisplay : MonoBehaviour {

    public GameObject tilePrefab;

    private List<GameObject> _patternTileList;

    void Awake()
    {
        _patternTileList = new List<GameObject>();
    }

    public void LoadPattern(PatternDescriptorData data)
    {
        foreach (GameObject obj in _patternTileList)
            GameObject.Destroy(obj);
        _patternTileList.Clear();
        foreach (string patternName in data.patternDesign)
        {
            GameObject tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            _patternTileList.Add(tile);
            tile.transform.SetParent(this.transform);
            tile.transform.localScale = new Vector3(1, 1, 1);
            PatternTileInfos infos = tile.GetComponent<PatternTileInfos>();
            infos.setTile(PatternInfos.stringToTile[patternName]);
        }
    }

    public List<string> GetPatternList()
    {
        List<string> pattern = new List<string>();

        foreach (GameObject obj in _patternTileList)
            pattern.Add(obj.GetComponent<PatternTileInfos>().getTile());
        return pattern;
    }
}
