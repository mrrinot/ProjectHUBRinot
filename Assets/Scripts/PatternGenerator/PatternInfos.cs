using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ePatternType : byte
{
    GENERIC = 0,
    START,
    END,
    TORCH,
    CHEST,
    TRAP
}

[System.Serializable]
public class PatternDescriptorData
{
    public string name;
    public int rarity;
    public ePatternType type;
    public List<string> patternDesign;
    public List<string> upFor;
    public List<string> downFor;
    public List<string> leftFor;
    public List<string> rightFor;

    public PatternDescriptorData()
    {
        patternDesign = new List<string>();
        upFor = new List<string>();
        downFor = new List<string>();
        leftFor = new List<string>();
        rightFor = new List<string>();
    }
}

public enum eTile : byte
{
    EMPTY = 0,
    GRASS,
    PALLET,
    CHEST,
    TRAP,
    START,
    EXIT,
    TORCH,
    WALL,
    WALLMAP
}


public class PatternInfos
{
    public static string DESCRIPTOR_PATH = "Assets/Resources/PatternDescriptors/";

    public static Dictionary<eTile, string> tileToString = new Dictionary<eTile, string>() { 
        { eTile.EMPTY, "Empty" },
        { eTile.GRASS, "Grass" },
        { eTile.PALLET, "Pallet" },
        { eTile.CHEST, "Chest" },
        { eTile.TRAP, "Trap" },
        { eTile.START, "Start" },
        { eTile.EXIT, "Exit" },
        { eTile.TORCH, "Torch" },
        { eTile.WALL, "Wall" },
        { eTile.WALLMAP, "WallMap" },
    };

    public static Dictionary<string, eTile> stringToTile = new Dictionary<string, eTile>() { 
        { "Empty", eTile.EMPTY },
        { "Grass", eTile.GRASS },
        { "Pallet", eTile.PALLET },
        { "Chest", eTile.CHEST },
        { "Trap", eTile.TRAP },
        { "Start", eTile.START },
        { "Exit", eTile.EXIT },
        { "Torch", eTile.TORCH },
        { "Wall", eTile.WALL },
        { "WallMap", eTile.WALLMAP },
    };
}
