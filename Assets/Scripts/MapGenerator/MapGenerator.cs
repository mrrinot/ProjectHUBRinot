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

public class MapTile
{
    public int posX;
    public int posY;
    public GameObject obj;
    public eTile tileType;

    public MapTile(int x, int y, GameObject o)
    {
        posX = x;
        posY = y;
        obj = o;
    }
}

public class MapGenerator : MonoBehaviour
{
    public static string BLOCK_PATH = "Blocks/";
    public static int RAND_NEW_BLOCK = 65;

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
    private List<MapTile> _tileList = new List<MapTile>();
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
        foreach (MapTile tile in _tileList)
            GameObject.Destroy(tile.obj);
        _tileList.Clear();
        _allTiles.Clear();
        _emptyTiles.Clear();
    }

    List<MapTile> GetMap()
    {
        return _tileList;
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

    MapPattern getRandomEmptyFurthestTile()
    {
        int maxRange = 0;
        List<MapPattern> furthest = new List<MapPattern>();
        foreach (MapPattern patt in _emptyTiles)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) > maxRange)
                maxRange = Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY);
        }
        foreach (MapPattern patt in _emptyTiles)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) == maxRange)
                furthest.Add(patt);
        }
        MapPattern chosen = furthest[UnityEngine.Random.Range(0, furthest.Count - 1)];
        _emptyTiles.Remove(chosen);
        return chosen;
    }

    MapPattern getRandomEmptyClosestTile()
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

    bool addTile(List<string> forbList, int posX, int posY)
    {
        MapPattern existing = null;
        bool ret = UnityEngine.Random.Range(1, 100) > RAND_NEW_BLOCK;
        bool newOne = false;

        if ((existing = getTileFromPos(posX, posY, _allTiles)) == null)
        {
            if (ret)
            {
                Debug.Log("ADDED tile in " + posX + "/" + posY);
                newOne = true;
                existing = new MapPattern(posX, posY, null);
                _allTiles.Add(existing);
                _emptyTiles.Add(existing);
            }
        }
        if (ret)
            existing.AddForbiddenPattern(forbList);
        return newOne;
    }

    void addNearbyTiles(MapPattern added)
    {
        int cpt = 0;
        int i = 0;
        while (cpt == 0 && i < 100)
        {
            if (addTile(added.currentPattern.rightFor, added.posX + 1, added.posY))
                ++cpt;
            if (addTile(added.currentPattern.leftFor, added.posX - 1, added.posY))
                ++cpt;
            if (addTile(added.currentPattern.upFor, added.posX, added.posY + 1))
                ++cpt;
            if (addTile(added.currentPattern.downFor, added.posX, added.posY - 1))
                ++cpt;
            ++i;
        }
        Debug.Log("i = " + i);
    }

    void setPatternOfType(ePatternType type)
    {
        Debug.Log("AVAILABLE PATTERN = " + _emptyTiles.Count);
        MapPattern chosen = null;
        if (type == ePatternType.END)
            chosen = getRandomEmptyFurthestTile();
        else
            chosen = getRandomEmptyClosestTile();
        Debug.Log("Chosen is :" + chosen.posX + "/" + chosen.posY);
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
        setPatternOfType(ePatternType.START);
        for (int i = 1; i < patternNumber; ++i)
        {
            tileType = getRandomPatternType();
            changeTypeRarity(tileType);
            setPatternOfType(tileType);
        }
        setPatternOfType(ePatternType.END);
    }

    void InstantiateMap()
    {
        foreach (MapPattern pattern in _allTiles)
        {
            if (pattern != null && pattern.currentPattern != null)
            {
                List<string> tileList = pattern.currentPattern.patternDesign;
                for (int i = 0; i < tileList.Count; ++i)
                {
                    GameObject block = Instantiate(_tilePrefabsDict[PatternInfos.stringToTile[tileList[i]]], new Vector3((i % 5) + (5 * pattern.posX), (i / 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                    MapTile tile = new MapTile((int)block.transform.position.x, (int)block.transform.position.y, block);
                    _tileList.Add(tile);
                }

                MapPattern nextTo = null;
                // RIGHT
                if ((nextTo = getTileFromPos(pattern.posX + 1, pattern.posY, _allTiles)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3(4 + (5 * pattern.posX + 1), (i % 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                        MapTile tile = new MapTile((int)block.transform.position.x, (int)block.transform.position.y, block);
                        _tileList.Add(tile);
                    }
                }
                //LEFT
                if ((nextTo = getTileFromPos(pattern.posX - 1, pattern.posY, _allTiles)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3(0 + (5 * pattern.posX - 1), (i % 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                        MapTile tile = new MapTile((int)block.transform.position.x, (int)block.transform.position.y, block);
                        _tileList.Add(tile);
                    }
                }
                //UP
                if ((nextTo = getTileFromPos(pattern.posX, pattern.posY + 1, _allTiles)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3((i % 5) + (5 * pattern.posX), 4 + (5 * pattern.posY + 1)), Quaternion.identity) as GameObject;
                        MapTile tile = new MapTile((int)block.transform.position.x, (int)block.transform.position.y, block);
                        _tileList.Add(tile);
                    }
                }
                //LEFT
                if ((nextTo = getTileFromPos(pattern.posX, pattern.posY - 1, _allTiles)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3((i % 5) + (5 * pattern.posX), 0 + (5 * pattern.posY - 1)), Quaternion.identity) as GameObject;
                        MapTile tile = new MapTile((int)block.transform.position.x, (int)block.transform.position.y, block);
                        _tileList.Add(tile);
                    }
                }
            }
            adjustMap();
        }

    }

    void adjustMap()
    {
    }

    #endregion

}
