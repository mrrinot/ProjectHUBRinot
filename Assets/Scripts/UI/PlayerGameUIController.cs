using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerGameUIController : MonoBehaviour
{
    [SerializeField]
    private Text EndTurnButtonText;

    [SerializeField]
    private GameObject MPHolder;

    [SerializeField]
    private GameObject HPHolder;

    private float _deltaX;
    private PlayerEntity _pEnt;
    private ActionManager _actManager;
    private bool _isTurn;
    private Camera _cam;
    private Canvas _worldCanvas;
    private GameObject _outlinePrefab;

    private List<BlockController> _selectedTiles = new List<BlockController>();
    private List<GameObject> _outlineList = new List<GameObject>();

    void Awake()
    {
        _pEnt = GetComponent<PlayerEntity>();
        _cam = GetComponentInChildren<Camera>();
        _outlinePrefab = Resources.Load("UI/Outline", typeof(GameObject)) as GameObject;
        _worldCanvas = GameObject.Find("WorldCanvas").GetComponent<Canvas>();
        _actManager = GameObject.Find("TurnManager").GetComponent<ActionManager>();
        _pEnt.OnHPChanged += OnHPChanged;
        _pEnt.OnMPChanged += OnMPChanged;
        _isTurn = false;
        EndTurnButtonText.text = "Your turn";
        _deltaX = MPHolder.GetComponent<RectTransform>().sizeDelta.x;
        OnHPChanged(_pEnt.HP_Current);
        OnMPChanged(_pEnt.MP_Current);
    }

    void Update()
    {
        if (_isTurn)
            ManageMovement();
    }

    private void ManageMovement()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Block")))
        {
            BlockController block = hit.collider.GetComponent<BlockController>();
            //Debug.Log("HOVER " + block.name + " " + block.X + "/" + block.Y);
            if (Input.GetMouseButtonDown(0)) // ADD TO QUEUE
                if (_selectedTiles.FirstOrDefault(x => x == block) == null && _pEnt.MP_Current > 0)
                {
                    BlockController nearest = _selectedTiles.Count == 0 ? this.GetComponentInParent<BlockController>() : _selectedTiles[_selectedTiles.Count - 1];
                    if (((block.IsVisible(e_Player.PLAYER) && block.IsWalkable(e_Player.PLAYER)) || (block.IsVisible(e_Player.PLAYER) == false)) && Vector3.Distance(block.transform.position, nearest.transform.position) == 1.0f)
                    {
                        GameObject outline = GameObject.Instantiate(_outlinePrefab, _worldCanvas.transform) as GameObject;
                        outline.transform.position = block.transform.position;
                        _outlineList.Add(outline);
                        _pEnt.MP_Current--;
                        _selectedTiles.Add(block);
                    }
                }
            if (Input.GetMouseButtonDown(1)) // REMOVE FROM QUEUE
            {
                int index;
                if ((index = _selectedTiles.FindIndex(x => x == block)) != -1)
                {
                    for (int i = _selectedTiles.Count - 1; i >= index; --i)
                    {
                        _pEnt.MP_Current++;
                        GameObject.Destroy(_outlineList[_selectedTiles.Count - 1]);
                        _outlineList.RemoveAt(_selectedTiles.Count - 1);
                        _selectedTiles.RemoveAt(_selectedTiles.Count - 1);
                    }
                }
            }
        }
    }

    public void OnHPChanged(float val)
    {
        RectTransform bar = HPHolder.GetComponent<RectTransform>();
        Text txt = HPHolder.transform.parent.GetComponentInChildren<Text>();

        bar.sizeDelta = new Vector2(_deltaX * (val / _pEnt.HP_Max), bar.sizeDelta.y);
        txt.text = val + " HP";
    }
    public void OnMPChanged(float val)
    {
        RectTransform bar = MPHolder.GetComponent<RectTransform>();
        Text txt = MPHolder.transform.parent.GetComponentInChildren<Text>();

        bar.sizeDelta = new Vector2(_deltaX * (val / _pEnt.MP_Max), bar.sizeDelta.y);
        txt.text = val + " MP";
        EndTurnButtonText.text = _pEnt.MP_Max == val ? "End turn" : "Move";
    }

    private IEnumerator Moveto()
    {
        int count = _selectedTiles.Count;
        bool moving = true;
        for (int i = 0; i < count; ++i)
        {
            _pEnt.MP_Current++;
            if (moving)
                moving = _actManager.Move(_pEnt, _selectedTiles[0].transform.position);
            _selectedTiles.RemoveAt(0);
            GameObject.Destroy(_outlineList[0]);
            _outlineList.RemoveAt(0);
            if (i + 1 < count)
                yield return new WaitForSeconds(0.5f);
        }
        _pEnt.OnEndTurn();
    }

    public void OnEndTurn()
    {
        if (_isTurn)
        {
            EndTurnButtonText.text = "Waiting ...";
            _isTurn = false;
            if (_selectedTiles.Count > 0)
                StartCoroutine(Moveto());
            else
                _pEnt.OnEndTurn();
        }
    }

    public void OnStartTurn()
    {
        EndTurnButtonText.text = "End turn";
        _isTurn = true;
    }
}
