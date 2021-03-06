﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManager : MonoBehaviour
{
    public delegate bool Bool_D_CEnt_V3(ControllableEntity ent, Vector3 tPos);

    private VisionManager _lightingManager;
    private SoundManager _soundManager;
    private MapHolder _map;
    private Dictionary<e_Action, Bool_D_CEnt_V3> _actionMap = new Dictionary<e_Action,Bool_D_CEnt_V3>();
    private int _stepSpeed = 0;

    public enum e_Action : byte
    {
        NONE = 0,
        MOVE,
        ARM_TRAP,
        DISARM_TRAP,
        PICKUP_TRAP,
        LIT_TORCH,
        UNLIT_TORCH,
        OPEN_CHEST,
        USE_EXIT,
        ARM_PALLET,
        DESTROY_PALLET,
        DAMAGE_PLAYER
    };

    void Start()
    {
        _map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().GetMap();
        _lightingManager = GetComponent<VisionManager>();
        _soundManager = GetComponent<SoundManager>();
        _actionMap[e_Action.MOVE] = move;
        _actionMap[e_Action.UNLIT_TORCH] = unlitTorch;
        _actionMap[e_Action.LIT_TORCH] = litTorch;
        _actionMap[e_Action.DISARM_TRAP] = disarmTrap;
        _actionMap[e_Action.ARM_TRAP] = armTrap;
        _actionMap[e_Action.ARM_PALLET] = armPallet;
        _actionMap[e_Action.DESTROY_PALLET] = destroyPallet;
    }

    public bool RequestAction(ActionManager.e_Action act, ControllableEntity ent, Vector3 targetPos)
    {
        if (_actionMap.ContainsKey(act))
            return _actionMap[act](ent, targetPos);
        else
            Debug.LogError("ACTION " + act + " NOT FOUND");
        return false;
    }

    private bool armPallet(ControllableEntity ent, Vector3 palletPos)
    {
        BlockController block = _map.GetBlock((int)palletPos.x, (int)palletPos.y);
        PalletObject pallet = block.GetComponentInChildren<PalletObject>();
        if (pallet != null)
        {
            pallet.BlockPallet();
            //_lightingManager.UpdateLighting();
            return true;
        }
        return false;
    }
    private bool destroyPallet(ControllableEntity ent, Vector3 palletPos)
    {
        BlockController block = _map.GetBlock((int)palletPos.x, (int)palletPos.y);
        PalletObject pallet = block.GetComponentInChildren<PalletObject>();
        if (pallet != null)
        {
            pallet.DestroyPallet();
            //_lightingManager.UpdateLighting();
            return true;
        }
        return false;
    }

    private bool disarmTrap(ControllableEntity ent, Vector3 trapPos)
    {
        BlockController block = _map.GetBlock((int)trapPos.x, (int)trapPos.y);
        TrapObject trap = block.GetComponentInChildren<TrapObject>();
        if (trap != null)
        {
            trap.ArmTrap();
            _lightingManager.UpdateLightingAndVision();
            return true;
        }
        return false;
    }
    private bool armTrap(ControllableEntity ent, Vector3 trapPos)
    {
        BlockController block = _map.GetBlock((int)trapPos.x, (int)trapPos.y);
        TrapObject trap = block.GetComponentInChildren<TrapObject>();
        if (trap != null)
        {
            trap.ArmTrap();
            _lightingManager.UpdateLightingAndVision();
            return true;
        }
        return false;
    }

    private bool unlitTorch(ControllableEntity ent, Vector3 torchPos)
    {
        BlockController block = _map.GetBlock((int)torchPos.x, (int)torchPos.y);
        TorchObject torch = block.GetComponentInChildren<TorchObject>();
        if (torch != null)
        {
            torch.LitTorch();
            _soundManager.ProduceSound(ent, block, 5f);
            _lightingManager.UpdateLightingAndVision();
            return true;
        }
        return false;
    }
    private bool litTorch(ControllableEntity ent, Vector3 torchPos)
    {
        BlockController block = _map.GetBlock((int)torchPos.x, (int)torchPos.y);
        TorchObject torch = block.GetComponentInChildren<TorchObject>();
        if (torch != null)
        {
            torch.LitTorch();
            _soundManager.ProduceSound(ent, block, 5f);
            _lightingManager.UpdateLightingAndVision();
            return true;
        }
        return false;
    }

    public void SetStepSpeed(int speed)
    {
        _stepSpeed = speed;
    }

    private bool move(ControllableEntity ent, Vector3 moveTo)
    {
        BlockController dest = _map.GetBlock((int)moveTo.x, (int)moveTo.y);
        if (dest.IsWalkable(ent.PID) == true && ent.MP_Current > 0)
        {
            ent.MP_Current--;
            ent.GetComponent<IObject>().ChangeBlock(dest);
            _lightingManager.UpdateLightingAndVision();
            _soundManager.ProduceSound(ent, dest, _stepSpeed);
            return true;
        }
        return false;
    }
}
