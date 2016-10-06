using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatternTileInfos : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler
{

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
        _currentImage.sprite = Resources.Load(PatternInfos.tileToString[_currentTile], typeof(Sprite)) as Sprite;
    }

    public string getTile()
    {
        return PatternInfos.tileToString[_currentTile];
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
            _currentImage.sprite = Resources.Load(PatternInfos.tileToString[_currentTile], typeof(Sprite)) as Sprite;
        }
    }
}
