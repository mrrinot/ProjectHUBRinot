using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum e_Player : byte
{
    PLAYER = 0,
    CHASER,
    SENTINEL,
    TRAPER
}

public class TurnManager : MonoBehaviour
{
    private List<ControllableEntity> _players = new List<ControllableEntity>();
    private int _playingTurnIndex = 0;

    public void Start()
    {
        _players[_playingTurnIndex].OnTurnStart();
    }

    public void AddPlayer(ControllableEntity ent)
    {
        _players.Add(ent);
    }

    public void EndTurn(ControllableEntity ent)
    {
        if (_players[_playingTurnIndex] == ent)
        {
            _playingTurnIndex = _playingTurnIndex + 1 == _players.Count ? _playingTurnIndex = 0 : ++_playingTurnIndex;
            _players[_playingTurnIndex].OnTurnStart();
        }
    }
}
