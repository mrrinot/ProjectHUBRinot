using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

public class PatternTileInfos : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler {

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
        WALL
    }

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
    };

    private Image _currentImage;
    private eTile _currentTile;
    private PatternInfosManager _infos;

	void Awake()
    {
        _currentImage = GetComponent<Image>();
        _currentTile = eTile.EMPTY;
        _infos = GameObject.FindObjectOfType<PatternInfosManager>();
	}

    public void setTile(eTile tile)
    {
        _currentTile = tile;
        _currentImage.sprite = Resources.Load(tileToString[_currentTile], typeof(Sprite)) as Sprite;
    }

    public string getTile()
    {
        return tileToString[_currentTile];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnPointerClick(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_infos.isLocked())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if ((byte)_currentTile == System.Enum.GetValues(typeof(eTile)).Length - 1)
                    _currentTile = eTile.EMPTY;
                else
                    ++_currentTile;

            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if ((byte)_currentTile == 0)
                    _currentTile = eTile.WALL;
                else
                    --_currentTile;
            }
            _currentImage.sprite = Resources.Load(tileToString[_currentTile], typeof(Sprite)) as Sprite;
        }
    }
}
