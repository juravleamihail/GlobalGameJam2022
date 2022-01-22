using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField]
    private PlayerTypeListSO _playerTypeList;

    private List<Player> _players;

    [SerializeField]
    private int _maxPlayers=2;

    [SerializeField] private Transform _mapParent;

    public bool canPlayersDrawPaths { get; private set; }

    public override void Awake()
    {
        base.Awake();
        GameManager.Instance.onTurnStateChanged = SetCanPlayersDrawPaths;
        
        _players = new List<Player>();
        InitPlayers();
    }

    public Player GetPlayerByIndex(int index)
    {
        if(!(index>=0 && index<_playerTypeList.playerTypeList.Count))
        {
            Debug.LogError("wrong index");
            return null;
        }

        return _players.Find(player => player.PlayerType.PlayerIndex == index);
    }

    void InitPlayers()
    {
        for(int i=0;i<_maxPlayers;i++)
        {
           var player =  Instantiate(_playerTypeList.playerTypeList[i].Prefab).GetComponent<Player>();
           player.transform.parent = _mapParent == null ? transform : _mapParent;
           player.transform.position = Vector3.zero;
            player.PlayerType = _playerTypeList.playerTypeList[i];

           _players.Add(player);
           
        }
    }

    void SetCanPlayersDrawPaths(bool inCanPlayersDrawPaths)
    {
        canPlayersDrawPaths = inCanPlayersDrawPaths;
    }
}
