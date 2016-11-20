using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlockController : MonoBehaviour
{
    private List<e_Object> _objectlist;
    private int _posX;
    private int _posY;
    private Dictionary<e_Player, bool> _walkable;
    private Dictionary<e_Player, bool> _visible;
    private Dictionary<e_Player, float> _heuristics;
    private SpriteRenderer _rend;

	void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _objectlist = new List<e_Object>();
        _walkable = new Dictionary<e_Player, bool>();
        _visible = new Dictionary<e_Player, bool>();
        _heuristics = new Dictionary<e_Player, float>();
        foreach (e_Player id in System.Enum.GetValues(typeof(e_Player)))
        {
            _walkable[id] = true;
            _visible[id] = false;
            _heuristics[id] = 0;
        }
        _posX = (int)transform.position.x;
        _posY = (int)transform.position.y;
	}

    #region Object

    public void AddObject(e_Object obj)
    {
        _objectlist.Add(obj);
    }

    public void RemoveObject(e_Object obj)
    {
        _objectlist.Remove(obj);
    }

    public bool HasObject(e_Object obj)
    {
        return _objectlist.Exists(x => x == obj);
    }

    #endregion

    #region Getter

    public int X
    {
        get { return _posX; }
    }
    public int Y
    {
        get { return _posY; }
    }

    public bool IsWalkable(e_Player playerId)
    {
        return _walkable[playerId];
    }

    public bool IsVisible(e_Player playerId)
    {
        return _visible[playerId];
    }

    public float GetHeuristic(e_Player playerId)
    {
        return _heuristics[playerId];
    }

    #endregion

    #region Setter

    public void SetWalkable(e_Player playerId, bool walk)
    {
        _walkable[playerId] = walk;
    }

    public void SetVisible(e_Player playerId, bool vis)
    {
        _visible[playerId] = vis;
    }

    public void SetHeuristic(e_Player playerId, float heur)
    {
        _heuristics[playerId] = heur;
    }

    #endregion

    #region vision

    public void SetVisionAlpha(float alpha)
    {
        if (_rend != null)
        {
            Color col = _rend.color;
            col.a = alpha;
            _rend.color = col;
        }
        IObject[] objs = GetComponentsInChildren<IObject>();
        foreach (IObject obj in objs)
        {
            obj.Alpha = alpha;
        }
    }

    public void AddAlpha(float alpha)
    {
        if (_rend != null)
        {
            Color col = _rend.color;
            col.a += alpha;
            _rend.color = col;
        }
        IObject[] objs = GetComponentsInChildren<IObject>();
        foreach (IObject obj in objs)
        {
            obj.Alpha += alpha;
        }
    }

    #endregion
}