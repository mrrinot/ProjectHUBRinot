using UnityEngine;
using System.Collections;

public abstract class ControllableEntity : MonoBehaviour
{
    [SerializeField]
    protected float _hpMax;
    protected float _hpCurrent;

    [SerializeField]
    protected float _mpMax;
    protected float _mpCurrent;

    public event Void_D_Float OnHPChanged;
    public event Void_D_Float OnMPChanged;

    [SerializeField]
    protected int _visionRange;

    [SerializeField]
    protected int _hearingRange = 0;

    [SerializeField]
    protected e_Player _pID;

    //protected Item _activItem = null;
    //protected List<Item> _passivList = new List<Item>();

    protected TurnManager _tManager;
    protected ActionManager _actManager;
    
    protected virtual void Awake()
    {
        _tManager= GameObject.Find("TurnManager").GetComponent<TurnManager>();
        _actManager = GameObject.Find("TurnManager").GetComponent<ActionManager>();
        _mpCurrent = _mpMax;
        _hpCurrent = _hpMax;
    }

    public virtual void OnTurnStart()
    {
        MP_Current = MP_Max;
    }

    public abstract void OnHit();

    public virtual e_Player PID
    {
        get { return _pID; }
    }

    public virtual float HP_Current
    {
        get { return _hpCurrent; }
        set
        {
            _hpCurrent = value > _hpMax ? _hpMax : value;
            _hpCurrent = _hpCurrent < 0 ? 0 : _hpCurrent;
            if (OnHPChanged != null)
                OnHPChanged(_hpCurrent);
        }
    }

    public virtual float HP_Max
    {
        get { return _hpMax; }
        set
        {
            _hpMax = value < 0 ? 0 : value;
            if (_hpMax < HP_Current)
                HP_Current = _hpMax;
        }
    }

    public virtual float MP_Current
    {
        get { return _mpCurrent; }
        set
        {
            _mpCurrent = value > _mpMax ? _mpMax : value;
            _mpCurrent = _mpCurrent < 0 ? 0 : _mpCurrent;
            if (OnMPChanged != null)
                OnMPChanged(_mpCurrent);
        }
    }

    public virtual float MP_Max
    {
        get { return _mpMax; }
        set
        {
            _mpMax = value < 0 ? 0 : value;
            if (_mpMax < MP_Current)
                MP_Current = _mpMax;
        }
    }

}
