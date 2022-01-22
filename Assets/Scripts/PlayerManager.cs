using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }

            return _instance;
        }
    }

    [SerializeField]
    private PlayerTypeListSO _playerTypeList;

    private List<Player> _players;

    [SerializeField]
    private int _maxPlayers=2;

    private void Awake()
    {
        if(_instance==null)
        {
            _instance = this;
        }
        else if(_instance!=null)
        {
            Destroy(this);
        }

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
           player.transform.parent = transform;
           player.transform.position = Vector3.zero;
            player.PlayerType = _playerTypeList.playerTypeList[i];

           _players.Add(player);
           
        }
    }


}
