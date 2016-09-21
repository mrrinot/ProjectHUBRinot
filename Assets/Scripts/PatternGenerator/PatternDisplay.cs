using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PatternDisplay : MonoBehaviour {

    private PatternDescriptorData _currentPattern = null;
    private List<GameObject> _patternTileList;
    public GameObject tilePrefab;

    void Start()
    {
        _patternTileList = new List<GameObject>();
    }

    public void LoadPattern(PatternDescriptorData data)
    {
        _currentPattern = data;
        foreach (GameObject obj in _patternTileList)
            GameObject.Destroy(obj);
        _patternTileList.Clear();
        foreach (string patternName in data.patternDesign)
        {
            GameObject tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            _patternTileList.Add(tile);
            tile.transform.parent = this.transform;
            tile.transform.localScale = new Vector3(1, 1, 1);
            PatternTileInfos infos = tile.GetComponent<PatternTileInfos>();
            infos.setTile(PatternTileInfos.stringToTile[patternName]);
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
