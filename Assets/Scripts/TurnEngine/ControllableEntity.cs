using UnityEngine;
using System.Collections;

public abstract class ControllableEntity : MonoBehaviour
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _mp;
    [SerializeField]
    protected int _visionRange;
    [SerializeField]
    protected int _hearingRange = 0;
    [SerializeField]
    protected e_Player _pID;
    //protected Item _activItem = null;
    //protected List<Item> _passivList = new List<Item>();

    protected TurnManager _tManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    //protected ActionManager _actManager;

    protected abstract void OnTurnStart();

    protected abstract void OnHit();
}
