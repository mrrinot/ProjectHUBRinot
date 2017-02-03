using UnityEngine;
using System.Collections;

public enum e_Object
{
    PLAYER = 0,
    KILLER,
    DUMMY,
    PALLET,
    START,
    EXIT,
    CHEST,
    TORCH,
    GRASS,
    TRAP,
    WALL
}

public delegate void Void_D_Float(float f);

public class IObject : MonoBehaviour
{
    [SerializeField]
    protected e_Object _type;
    [SerializeField]
    protected float _minAlpha;
    [SerializeField]
    protected float _reducedAlpha;

    protected SpriteRenderer _sprite;
    protected BlockController _bCtrl;
    protected float _alpha = 0f;

    public event Void_D_Float OnAlphaChanged;

    protected virtual void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        OnAlphaChanged += HideObject;
        _bCtrl = GetComponentInParent<BlockController>();
        OnBlockAdd();
    }

    protected virtual void OnBlockAdd()
    {
        _bCtrl.AddLightBlock(_reducedAlpha);
        _bCtrl.AddObject(_type); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    protected virtual void OnBlockRemove()
    {
        _bCtrl.AddLightBlock(_reducedAlpha * -1f);
        _bCtrl.RemoveObject(_type); // MUST BE LAST DUE TO CALLBACK IN CONTROLLER
    }

    public virtual void ChangeBlock(BlockController newBlock)
    {
        OnBlockRemove();
        _bCtrl = newBlock;
        this.transform.SetParent(newBlock.transform);
        this.transform.localPosition = Vector3.zero;
        OnBlockAdd();
    }

    void HideObject(float alpha)
    {
        if (alpha >=_minAlpha)
            _sprite.enabled = true;
        else
            _sprite.enabled = false;
    }

    public e_Object Type
    {
        get { return _type; }
    }

    public float Alpha
    {
        get { return _alpha; }
        set
        {
            _alpha = value;
            Color col = _sprite.color;
            col.a = _alpha;
            _sprite.color = col;
            if (OnAlphaChanged != null)
                OnAlphaChanged(_alpha);
        }
    }
}
