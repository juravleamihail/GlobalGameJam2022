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

    private List<Player> players;

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

        players = new List<Player>( FindObjectsOfType<Player>());
    }

    public Player GetPlayerByIndex(int index)
    {
        if(!(index>=0 && index<2))
        {
            Debug.LogError("wrong index");
            return null;
        }

        return players.Find(player => player.PlayerIndex == index);
    }
}
