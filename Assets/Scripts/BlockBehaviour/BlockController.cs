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
    private Dictionary<e_Player, List<ActionManager.e_Action>> _potentialActions;
    private SpriteRenderer _rend;
    private float _lightBlock;
    private float _alpha;

	void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _objectlist = new List<e_Object>();
        _walkable = new Dictionary<e_Player, bool>();
        _visible = new Dictionary<e_Player, bool>();
        _heuristics = new Dictionary<e_Player, float>();
        _potentialActions = new Dictionary<e_Player, List<ActionManager.e_Action>>();
        foreach (e_Player id in System.Enum.GetValues(typeof(e_Player)))
        {
            _walkable[id] = true;
            _visible[id] = false;
            _potentialActions[id] = new List<ActionManager.e_Action>();
            _heuristics[id] = 0;
        }
        _posX = (int)transform.position.x;
        _posY = (int)transform.position.y;
        _lightBlock = 0f;
        _alpha = 0f;
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

    #region Actions

    public void AddPotentialAction(e_Player pid, ActionManager.e_Action act)
    {
        _potentialActions[pid].Add(act);
    }

    public void RemovePotentialAction(e_Player pid, ActionManager.e_Action act)
    {
        _potentialActions[pid].Remove(act);
    }
    public void AddPotentialActionAll(ActionManager.e_Action act)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _potentialActions[val].Add(act);
    }

    public void RemovePotentialActionAll(ActionManager.e_Action act)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _potentialActions[val].Remove(act);
    }

    public List<ActionManager.e_Action> GetPotentialActionsForPlayer(e_Player pid)
    {
        return _potentialActions[pid];
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

    public void SetWalkableAll(bool walk)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _walkable[val] = walk;
    }
    public void SetVisibleAll(bool vis)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _visible[val] =  vis;
    }

    public void SetHeuristic(e_Player playerId, float heur)
    {
        _heuristics[playerId] = heur;
    }

    #endregion

    #region vision

    public void AddLightBlock(float block)
    {
        _lightBlock =+ block;
    }

    public float GetLightBlock()
    {
        return _lightBlock;
    }

    public float Alpha
    {
        get { return _alpha; }
        set
        {
            _alpha = value;
            if (_rend != null)
            {
                Color col = _rend.color;
                col.a = _alpha;
                _rend.color = col;
            }
            IObject[] objs = GetComponentsInChildren<IObject>();
            foreach (IObject obj in objs)
            {
                obj.Alpha = _alpha;
            }
        }
    }

    #endregion
}