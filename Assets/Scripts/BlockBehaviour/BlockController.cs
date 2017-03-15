using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public delegate void Void_D_Void();

public class BlockController : MonoBehaviour
{

    private List<e_Object> _objectlist;
    private int _posX;
    private int _posY;
    private Dictionary<e_Player, bool> _walkable;
    private Dictionary<e_Player, bool> _visible;
    private Dictionary<e_Player, float> _heuristics;
    private Dictionary<e_Player, float> _movementCost;
    private Dictionary<e_Player, float> _alphas;
    private Dictionary<e_Player, List<ActionManager.e_Action>> _potentialActions;
    private SpriteRenderer _rend;
    private float _lightBlock;

    public event Void_D_Void OnObjectAdded;
    public event Void_D_Void OnObjectRemoved;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _objectlist = new List<e_Object>();
        _walkable = new Dictionary<e_Player, bool>();
        _visible = new Dictionary<e_Player, bool>();
        _heuristics = new Dictionary<e_Player, float>();
        _movementCost = new Dictionary<e_Player, float>();
        _alphas = new Dictionary<e_Player, float>();
        _potentialActions = new Dictionary<e_Player, List<ActionManager.e_Action>>();
        foreach (e_Player id in System.Enum.GetValues(typeof(e_Player)))
        {
            _walkable[id] = true;
            _visible[id] = false;
            _potentialActions[id] = new List<ActionManager.e_Action>();
            _alphas[id] = 0f;
            _heuristics[id] = 0f;
            _movementCost[id] = 100f;
        }
        _posX = (int)transform.position.x;
        _posY = (int)transform.position.y;
        _lightBlock = 0f;
    }

    #region Object

    public void AddObject(e_Object obj)
    {
        _objectlist.Add(obj);
        if (OnObjectAdded != null)
            OnObjectAdded();
    }

    public void RemoveObject(e_Object obj)
    {
        _objectlist.Remove(obj);
        if (OnObjectRemoved != null)
            OnObjectRemoved();
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

    public float GetMovementCost(e_Player playerId)
    {
        return _movementCost[playerId];
    }

    public float GetAlpha(e_Player playerId)
    {
        return _alphas[playerId];
    }

    #endregion

    #region Setter

    public void SetHeuristic(e_Player playerId, float val)
    {
        _heuristics[playerId] = val;
    }

    public void SetHeuristicsAll(float val)
    {
        foreach (e_Player playerId in System.Enum.GetValues(typeof(e_Player)))
            _heuristics[playerId] = val;
    }
    public void AddHeuristics(e_Player playerId, float val)
    {
        _heuristics[playerId] += val;
    }
    public void AddHeuristicsAll(float val)
    {
        foreach (e_Player playerId in System.Enum.GetValues(typeof(e_Player)))
            _heuristics[playerId] += val;
    }
    public void SetMovementCost(e_Player playerId, float val)
    {
        _movementCost[playerId] = val;
    }
    public void SetMovementCostAll(float val)
    {
        foreach (e_Player playerId in System.Enum.GetValues(typeof(e_Player)))
            _movementCost[playerId] = val;
    }
    public void AddMovementCost(e_Player playerId, float val)
    {
        _movementCost[playerId] += val;
    }
    public void AddMovementCostAll(float val)
    {
        foreach (e_Player playerId in System.Enum.GetValues(typeof(e_Player)))
            _movementCost[playerId] += val;
    }

    public void SetWalkable(e_Player playerId, bool walk)
    {
        _walkable[playerId] = walk;
    }

    public void SetVisible(e_Player playerId, bool vis)
    {
        _visible[playerId] = vis;
    }

    private void changeAlpha(e_Player playerId)
    {
        if (playerId == e_Player.PLAYER)
        {
            if (_rend != null)
            {
                Color col = _rend.color;
                col.a = _alphas[playerId];
                _rend.color = col;
            }
            IObject[] objs = GetComponentsInChildren<IObject>();
            foreach (IObject obj in objs)
            {
                obj.Alpha = _alphas[playerId];
            }
        }
    }

    public void SetAlpha(e_Player playerId, float val)
    {
        _alphas[playerId] = val;
        changeAlpha(playerId);
    }

    public void AddAlpha(e_Player playerId, float val)
    {
        _alphas[playerId] += val;
        changeAlpha(playerId);
    }

    public void SetWalkableAll(bool walk)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _walkable[val] = walk;
    }
    public void SetVisibleAll(bool vis)
    {
        foreach (e_Player val in System.Enum.GetValues(typeof(e_Player)))
            _visible[val] = vis;
    }

    #endregion

    #region vision

    public void AddLightBlock(float block)
    {
        _lightBlock = +block;
    }

    public float GetLightBlock()
    {
        return _lightBlock;
    }

    #endregion
}