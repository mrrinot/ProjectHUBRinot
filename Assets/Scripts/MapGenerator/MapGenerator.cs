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

public class MapHolder
{
    BlockController[][] _map;
    int _minX, _maxX, _minY, _maxY;

    public BlockController GetBlock(int x, int y)
    {
        return _map[x - _minX][y - _minY];
    }

    public int GetMapWidth()
    {
        return _maxX - _minX;
    }

    public int GetMapHeight()
    {
        return _maxY - _minY;
    }

    public BlockController[][] GetRawMap()
    {
        return _map;
    }

    public void EraseMap()
    {
        for (int i = 0; i < _maxX - _minX; ++i)
            for (int j = 0; j < _maxY - _minY; ++j)
                GameObject.Destroy(_map[i][j].gameObject);
    }

    public List<BlockController> GetAllBlockWithinDistanceOf(BlockController center, int distance)
    {
        List<BlockController> results = new List<BlockController>();
        for (int i = 0; i < _maxX - _minX; ++i)
            for (int j = 0; j < _maxY - _minY; ++j)
            {
                BlockController block = _map[i][j];
                if (block != null && (int)Vector2.Distance(block.transform.position, center.transform.position) == distance)
                    results.Add(block);
            }
        return results;
    }

    public List<BlockController> GetAllBlocksWith(List<e_Object> include)
    {
        List<BlockController> results = new List<BlockController>();
        for (int i = 0; i < _maxX - _minX; ++i)
            for (int j = 0; j < _maxY - _minY; ++j)
            {
                BlockController block = _map[i][j];
                if (block != null)
                {
                    foreach (e_Object obj in include)
                    {
                        if (block.HasObject(obj))
                        {
                            results.Add(block);
                            break;
                        }
                    }
                }
            }
        return results;
    }

    public MapHolder(int minX, int maxX, int minY, int maxY, List<BlockController> blockList)
    {
        _minX = minX;
        _maxX = maxX;
        _minY = minY;
        _maxY = maxY;
        _map = new BlockController[_maxX - _minX][];
        //Debug.Log("X SIZE = " + (_maxX - _minX));
        //Debug.Log("Y SIZE = " + (_maxY - _minY));
        for (int i = 0; i < _maxX - _minX; ++i)
        {
            _map[i] = new BlockController[_maxY - _minY];
            for (int j = 0; j < _maxY - _minY; ++j)
                _map[i][j] = null;
        }
        foreach (BlockController block in blockList)
        {
            //Debug.Log("x/y = "+block.X+"/"+block.Y+" ==>"+(block.X - _minX) + "/"+(block.Y - _minY));
            if (_map[block.X - _minX][block.Y - _minY] != null)
                GameObject.Destroy(_map[block.X - _minX][block.Y - _minY].gameObject);
            _map[block.X - _minX][block.Y - _minY] = block;
        }
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
    private Dictionary<e_Player, GameObject> _ennemyEnumToName = new Dictionary<e_Player, GameObject>();
    private Dictionary<ePatternType, int> _totalPatternsRarity = new Dictionary<ePatternType, int>();
    private Dictionary<eTile, GameObject> _tilePrefabsDict = new Dictionary<eTile, GameObject>();
    private MapHolder _map = null;
    private List<MapPattern> _allPatterns = new List<MapPattern>();
    private List<MapPattern> _emptyPatterns = new List<MapPattern>();

    void Awake()
    {
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue - 1);
        Debug.Log("SEED = " + seed);
        UnityEngine.Random.InitState(seed);
        loadPatterns();
        loadPrefabs();
        CreateMap(50, new List<e_Player>() {e_Player.CHASER});
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

    void loadPrefabs()
    {
        foreach (eTile tileEnum in System.Enum.GetValues(typeof(eTile)))
            _tilePrefabsDict[tileEnum] = Resources.Load(BLOCK_PATH + PatternInfos.tileToString[tileEnum] + "Block", typeof(GameObject)) as GameObject;
        _ennemyEnumToName[e_Player.CHASER] = Resources.Load("Objects/ChaserObject", typeof(GameObject)) as GameObject;
        _ennemyEnumToName[e_Player.SENTINEL] = Resources.Load("Objects/SentinelObject", typeof(GameObject)) as GameObject;
        _ennemyEnumToName[e_Player.TRAPPER] = Resources.Load("Objects/TrapperObject", typeof(GameObject)) as GameObject;
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
        if (_map != null)
            _map.EraseMap();
        _allPatterns.Clear();
        _emptyPatterns.Clear();
    }

    public MapHolder GetMap()
    {
        return _map;
    }

    #endregion

    #region getter
    ePatternType getRandomPatternType()
    {
        int total = 0;
        //Debug.Log("/////// \n Pattern types rarity");
        foreach (KeyValuePair<ePatternType, PatternTypeInfos> infos in _typeDict)
        {
            //Debug.Log("TYPE = " + infos.Key + " ==> " + infos.Value.currentRarity);
            total += infos.Value.currentRarity;
        }
        int random = UnityEngine.Random.Range(1, total);
        //Debug.Log("total = " + total + " RANDOM = " + random + " \\\\\\\\\\ \n");
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
        foreach (MapPattern patt in _emptyPatterns)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) > maxRange)
                maxRange = Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY);
        }
        foreach (MapPattern patt in _emptyPatterns)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) == maxRange)
                furthest.Add(patt);
        }
        MapPattern chosen = furthest[UnityEngine.Random.Range(0, furthest.Count - 1)];
        _emptyPatterns.Remove(chosen);
        return chosen;
    }

    MapPattern getRandomEmptyClosestTile()
    {
        List<MapPattern> closest = new List<MapPattern>();
        int rangeMin = Mathf.Abs(_emptyPatterns[0].posX) + Mathf.Abs(_emptyPatterns[0].posY);
        foreach (MapPattern patt in _emptyPatterns)
        {
            if (Mathf.Abs(patt.posX) + Mathf.Abs(patt.posY) <= rangeMin)
                closest.Add(patt);
        }
        MapPattern chosen = closest[UnityEngine.Random.Range(0, closest.Count - 1)];
        _emptyPatterns.Remove(chosen);
        return chosen;
    }

    #endregion

    #region creation

    bool addTile(List<string> forbList, int posX, int posY)
    {
        MapPattern existing = null;
        bool ret = UnityEngine.Random.Range(1, 100) > RAND_NEW_BLOCK;
        bool newOne = false;

        if ((existing = getTileFromPos(posX, posY, _allPatterns)) == null)
        {
            if (ret)
            {
                //Debug.Log("ADDED tile in " + posX + "/" + posY);
                newOne = true;
                existing = new MapPattern(posX, posY, null);
                _allPatterns.Add(existing);
                _emptyPatterns.Add(existing);
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
        //Debug.Log("i = " + i);
    }

    void setPatternOfType(ePatternType type)
    {
        //Debug.Log("AVAILABLE PATTERN = " + _emptyPatterns.Count);
        MapPattern chosen = null;
        if (type == ePatternType.END)
            chosen = getRandomEmptyFurthestTile();
        else
            chosen = getRandomEmptyClosestTile();
        //Debug.Log("Chosen is :" + chosen.posX + "/" + chosen.posY);
        PatternDescriptorData data = getRandomPatternOfType(type, chosen.forbiddenPattern);
        //Debug.Log("chosen pattern rarity = " + data.rarity + " of type = " + type);
        chosen.currentPattern = data;
        addNearbyTiles(chosen);
    }

    public void CreateMap(int patternNumber, List<e_Player> ennemyList)
    {
        MapPattern addedPattern = new MapPattern(0, 0, null);
        ePatternType tileType;

        eraseMap();
        _allPatterns.Add(addedPattern);
        _emptyPatterns.Add(addedPattern);
        setPatternOfType(ePatternType.START);
        for (int i = 1; i < patternNumber; ++i)
        {
            tileType = getRandomPatternType();
            changeTypeRarity(tileType);
            setPatternOfType(tileType);
        }
        setPatternOfType(ePatternType.END);
        InstantiateMap(patternNumber, ennemyList);
    }

    void InstantiateMap(int patternNumber, List<e_Player> ennemyList)
    {
        int maxX = 0, maxY = 0, minX = 0, minY = 0;
        List<BlockController> blockList = new List<BlockController>();
        foreach (MapPattern pattern in _allPatterns)
        {
            if (pattern != null && pattern.currentPattern != null)
            {
                if (pattern.posX < minX)
                    minX = pattern.posX;
                if (pattern.posX > maxX)
                    maxX = pattern.posX;
                if (pattern.posY < minY)
                    minY = pattern.posY;
                if (pattern.posY > maxY)
                    maxY = pattern.posY;
                List<string> tileList = pattern.currentPattern.patternDesign;
                for (int i = 0; i < tileList.Count; ++i)
                {
                    GameObject block = Instantiate(_tilePrefabsDict[PatternInfos.stringToTile[tileList[i]]], new Vector3((i % 5) + (5 * pattern.posX), (i / 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                    blockList.Add(block.GetComponent<BlockController>());
                }

                MapPattern nextTo = null;
                // RIGHT
                if ((nextTo = getTileFromPos(pattern.posX + 1, pattern.posY, _allPatterns)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3(4 + (5 * pattern.posX + 1), (i % 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                        blockList.Add(block.GetComponent<BlockController>());
                    }
                }
                //LEFT
                if ((nextTo = getTileFromPos(pattern.posX - 1, pattern.posY, _allPatterns)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3(0 + (5 * pattern.posX - 1), (i % 5) + (5 * pattern.posY)), Quaternion.identity) as GameObject;
                        blockList.Add(block.GetComponent<BlockController>());
                    }
                }
                //UP
                if ((nextTo = getTileFromPos(pattern.posX, pattern.posY + 1, _allPatterns)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3((i % 5) + (5 * pattern.posX), 4 + (5 * pattern.posY + 1)), Quaternion.identity) as GameObject;
                        blockList.Add(block.GetComponent<BlockController>());
                    }
                }
                //LEFT
                if ((nextTo = getTileFromPos(pattern.posX, pattern.posY - 1, _allPatterns)) == null || nextTo.currentPattern == null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        GameObject block = Instantiate(_tilePrefabsDict[eTile.WALLMAP], new Vector3((i % 5) + (5 * pattern.posX), 0 + (5 * pattern.posY - 1)), Quaternion.identity) as GameObject;
                        blockList.Add(block.GetComponent<BlockController>());
                    }
                }
            }
        }
        minX--;
        maxX += 2;
        minY--;
        maxY += 2;
        _map = new MapHolder(minX * 5, maxX * 5, minY * 5, maxY * 5, blockList);
        adjustMap(patternNumber, ennemyList);

    }

    void adjustMap(int patternNumber, List<e_Player> ennemyList)
    {
        List<BlockController> pallets = _map.GetAllBlocksWith(new List<e_Object>() { e_Object.PALLET });

        foreach (BlockController palletblock in pallets)
        {
            int unwalkableCount = 0;
            if (_map.GetBlock(palletblock.X + 1, palletblock.Y).IsWalkable(e_Player.PLAYER) == false)
                unwalkableCount++;
            if (_map.GetBlock(palletblock.X - 1, palletblock.Y).IsWalkable(e_Player.PLAYER) == false)
                unwalkableCount++;
            if (_map.GetBlock(palletblock.X, palletblock.Y + 1).IsWalkable(e_Player.PLAYER) == false)
                unwalkableCount++;
            if (_map.GetBlock(palletblock.X, palletblock.Y - 1).IsWalkable(e_Player.PLAYER) == false)
                unwalkableCount++;
            if (unwalkableCount >= 3)
                GameObject.Destroy(palletblock.GetComponentInChildren<PalletObject>().gameObject);
        }
        BlockController startBlock = _map.GetAllBlocksWith(new List<e_Object>() { e_Object.START })[0];
        if (ennemyList != null)
        {
            foreach (e_Player ennemy in ennemyList)
            {
                int cpt = 9;
                bool found = false;
                while (cpt > 0 && found == false)
                {
                    List<BlockController> lineBlocks = _map.GetAllBlockWithinDistanceOf(startBlock, cpt);
                    int rdn = 0;
                    while (found == false && lineBlocks.Count > 0)
                    {
                        BlockController block = lineBlocks[UnityEngine.Random.Range(0, lineBlocks.Count)];
                        if (block.IsWalkable(ennemy) && block.HasObject(e_Object.TRAP) == false && block.HasObject(e_Object.KILLER) == false)
                        {
                            GameObject ennemyGO = GameObject.Instantiate(_ennemyEnumToName[ennemy], block.transform);
                            ennemyGO.transform.localPosition = Vector3.zero;
                            found = true;
                            break;
                        }
                        else
                            lineBlocks.Remove(block);
                    }
                    cpt--;
                }
            }
        }

    }

    #endregion
}
