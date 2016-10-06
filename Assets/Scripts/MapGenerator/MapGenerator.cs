using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;

public class PatternTypeInfos
{
    public int baseRarity;
    public int currentRarity;
    public int addedRarity;
    private int _numberAdded;

    public PatternTypeInfos(int baseR, int added)
    {
        this.baseRarity = baseR;
        this.currentRarity = baseRarity;
        this.addedRarity = added;
        this._numberAdded = 0;
    }

    public void resetRarity()
    {
        currentRarity = baseRarity;
        _numberAdded = 0;
    }

    public void addRarity()
    {
        currentRarity += (addedRarity * _numberAdded);
        ++_numberAdded;
    }
}

public class MapPattern
{
    public int posX;
    public int posY;
    public List<string> forbiddenPattern;
    public PatternDescriptorData currentPattern;

    public bool IsPatternAllowed(string name)
    {
        return forbiddenPattern.FirstOrDefault(x => x == name) == null;
    }
    public void AddForbiddenPattern(List<string> list)
    {
        foreach (string name in list)
            if (forbiddenPattern.FirstOrDefault(x => x == name) == null)
                forbiddenPattern.Add(name);
    }

    public MapPattern(int x, int y, PatternDescriptorData data)
    {
        posX = x;
        posY = y;
        forbiddenPattern = new List<string>();
        currentPattern = data;
    }

}

public class MapGenerator : MonoBehaviour
{
    public static string BLOCK_PATH = "Blocks/";

    private Dictionary<ePatternType, PatternTypeInfos> _typeDict = new Dictionary<ePatternType, PatternTypeInfos>()
    {
        {ePatternType.GENERIC, new PatternTypeInfos(5000, 10)},
        {ePatternType.CHEST, new PatternTypeInfos(10, 2)},
        {ePatternType.TORCH, new PatternTypeInfos(300, 3)},
        {ePatternType.TRAP, new PatternTypeInfos(200, 10)},
    };

    private Dictionary<ePatternType, List<PatternDescriptorData>> _patterns = new Dictionary<ePatternType, List<PatternDescriptorData>>();
    private Dictionary<ePatternType, int> _totalPatternsRarity = new Dictionary<ePatternType, int>();
    private Dictionary<eTile, GameObject> _tilePrefabsDict = new Dictionary<eTile, GameObject>();
    private List<GameObject> _tileList = new List<GameObject>();
    private List<MapPattern> _allTiles = new List<MapPattern>();
    private List<MapPattern> _emptyTiles = new List<MapPattern>();

    void Awake()
    {
        loadPatterns();
        loadTilePrefabs();
        CreateMap(50);
        InstantiateMap();
    }

    #region Utility
    void loadPatterns()
    {
        foreach (ePatternType enumType in System.Enum.GetValues(typeof(ePatternType)))
        {
            _patterns[enumType] = new List<PatternDescriptorData>();
            _totalPatternsRarity[enumType] = 0;
        }
        string[] fileEntries = Directory.GetFiles(PatternInfos.DESCRIPTOR_PATH);
        foreach (string file in fileEntries)
        {
            if (!file.Contains(".meta"))
            {
                string name = file.Substring(PatternInfos.DESCRIPTOR_PATH.Length, file.LastIndexOf(".") - PatternInfos.DESCRIPTOR_PATH.Length);
                PatternDescriptorData data = new PatternDescriptorData();
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PatternDescriptorData));
                FileStream fs = new FileStream(PatternInfos.DESCRIPTOR_PATH + name + ".xml", FileMode.Open);
                try
                {
                    data = reader.Deserialize(fs) as PatternDescriptorData;
                }
                catch
                {
                    Debug.LogError("File " + PatternInfos.DESCRIPTOR_PATH + name + ".xml" + " could not open !");
                    fs.Close();
                    return;
                }
                fs.Close();
                _patterns[data.type].Add(data);
                _totalPatternsRarity[data.type] += data.rarity;
            }
        }
    }

    void loadTilePrefabs()
    {
        foreach (eTile tileEnum in System.Enum.GetValues(typeof(eTile)))
            _tilePrefabsDict[tileEnum] = Resources.Load(BLOCK_PATH + PatternInfos.tileToString[tileEnum] + "Block", typeof(GameObject)) as GameObject;
    }

    void resetPatterns()
    {
        foreach (var patternType in _typeDict)
            patternType.Value.resetRarity();
    }

    void changeTypeRarity(ePatternType chosen)
    {
        foreach (KeyValuePair<ePatternType, PatternTypeInfos> infos in _typeDict)
        {
            if (infos.Key == chosen)
                infos.Value.resetRarity();
            else
                infos.Value.addRarity();
        }
    }

    void eraseMap()
    {
        foreach (GameObject obj in _tileList)
            GameObject.Destroy(obj);
        _tileList.Clear();
        _allTiles.Clear();
        _emptyTiles.Clear();
    }

    #endregion

    #region getter
    ePatternType getRandomPatternType()
    {
        int total = 0;
        Debug.Log("/////// \n Pattern types rarity");
        foreach (KeyValuePair<ePatternType, PatternTypeInfos> infos in _typeDict)
        {
            Debug.Log("TYPE = " + infos.Key + " ==> " + infos.Value.currentRarity);
            total += infos.Value.currentRarity;
        }
        int random = UnityEngine.Random.Range(1, total);
        Debug.Log("total = " + total + " RANDOM = " + random + " \\\\\\\\\\ \n");
        foreach (KeyValuePair<ePatternType, PatternTypeInfos> infos in _typeDict)
        {
            random -= infos.Value.currentRarity;
            if (random <= 0)
                return infos.Key;
        }
        return ePatternType.GENERIC;
    }

    PatternDescriptorData getRandomPatternOfType(ePatternType type, List<string> forbiddenList)
    {
        PatternDescriptorData chosen = null;
        while (chosen == null)
        {
            int total = _totalPatternsRarity[type];
            int random = UnityEngine.Random.Range(1, total);
            foreach (PatternDescriptorData data in _patterns[type])
            {
                random -= data.rarity;
                if (random <= 0)
                {
                    if (forbiddenList.FirstOrDefault(x => x == data.name) == null)
                        chosen = data;
                    break;
                }
            }
        }
        return chosen;
    }

    MapPattern getTileFromPos(int posX, int posY, List<MapPattern> list)
    {
        return list.FirstOrDefault(tile => tile.posX == posX && tile.posY == posY);
    }

    MapPattern getRandomEmptyTile()
    {
        List<MapPattern> closest = new List<MapPattern>();
        int rangeMin = Mathf.Abs(_emptyTiles[0].posX) + Mathf.Abs(_emptyTiles[0].posY);
        foreach (MapPattern patt in _emptyTiles)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) <= rangeMin)
                closest.Add(patt);
        }
        MapPattern chosen = closest[UnityEngine.Random.Range(0, closest.Count - 1)];
        _emptyTiles.Remove(chosen);
        return chosen;
    }

    #endregion

    #region creation

    void addNearbyTiles(MapPattern added)
    {
        MapPattern existing = null;

        // RIGHT TILE
        if ((existing = getTileFromPos(added.posX + 1, added.posY, _allTiles)) == null)
        {
            existing = new MapPattern(added.posX + 1, added.posY, null);
            _allTiles.Add(existing);
            _emptyTiles.Add(existing);
        }
        existing.AddForbiddenPattern(added.currentPattern.rightFor);
        // LEFT TILE
        if ((existing = getTileFromPos(added.posX - 1, added.posY, _allTiles)) == null)
        {
            existing = new MapPattern(added.posX - 1, added.posY, null);
            _allTiles.Add(existing);
            _emptyTiles.Add(existing);
        }
        existing.AddForbiddenPattern(added.currentPattern.leftFor);
        // UP TILE
        if ((existing = getTileFromPos(added.posX, added.posY + 1, _allTiles)) == null)
        {
            existing = new MapPattern(added.posX, added.posY + 1, null);
            _allTiles.Add(existing);
            _emptyTiles.Add(existing);
        }
        existing.AddForbiddenPattern(added.currentPattern.upFor);
        // DOWN TILE
        if ((existing = getTileFromPos(added.posX, added.posY - 1, _allTiles)) == null)
        {
            existing = new MapPattern(added.posX, added.posY - 1, null);
            _allTiles.Add(existing);
            _emptyTiles.Add(existing);
        }
        existing.AddForbiddenPattern(added.currentPattern.downFor);
    }

    void addPatternOfType(ePatternType type)
    {
        MapPattern chosen = getRandomEmptyTile();
        PatternDescriptorData data = getRandomPatternOfType(type, chosen.forbiddenPattern);
        Debug.Log("chosen pattern rarity = " + data.rarity + " of type = " + type);
        chosen.currentPattern = data;
        addNearbyTiles(chosen);
    }

    public void CreateMap(int patternNumber)
    {
        MapPattern addedPattern = new MapPattern(0, 0, null);
        ePatternType tileType;

        eraseMap();
        _allTiles.Add(addedPattern);
        _emptyTiles.Add(addedPattern);
        addPatternOfType(ePatternType.START);
        for (int i = 1; i < patternNumber; ++i)
        {
            tileType = getRandomPatternType();
            changeTypeRarity(tileType);
            addPatternOfType(tileType);
        }
        addPatternOfType(ePatternType.END);
    }

    void InstantiateMap()
    {
        foreach (MapPattern pattern in _allTiles)
        {
            if (pattern.currentPattern != null)
            {
                List<string> tileList = pattern.currentPattern.patternDesign;
                for (int i = 0; i < tileList.Count; ++i)
                {
                    GameObject block = Instantiate(_tilePrefabsDict[PatternInfos.stringToTile[tileList[i]]], new Vector3((i % 5) + (5 * pattern.posX), (i / 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                    _tileList.Add(block);
                }
            }
        }
    }

    #endregion

}
