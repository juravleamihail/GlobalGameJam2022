using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    Vector2Int[] respawnPoints;
    [SerializeField]private int _turnsDelay;
    private Dictionary<int,int> _turnsElapsedPerNinja;
    private bool[] _areNinjasAlive; //TODO maybe avoid this extra caching; instead, use the Dictionary in NinjaManager and check Ninja.IsNinjaAlive

    //this is meant to be attached to the Player prefab

    private IEnumerator Start()
    {
        //wait a frame since the ninjas are not initialized;
        //need to find a different solution
        yield return null;
        
        Player player = GetComponent<Player>();
        int playerIndex = player.PlayerType.PlayerIndex;
        if (playerIndex == 0)
        {
            Init(NinjaManager.Instance.p1NinjaSpawnPoints);
        }
        if (playerIndex == 1)
        {
            Init(NinjaManager.Instance.p2NinjaSpawnPoints);
        }
    }

    public void Init(Vector2Int[] inRespawnPoints)
    {
        respawnPoints = new Vector2Int[inRespawnPoints.Length];
        inRespawnPoints.CopyTo(respawnPoints, 0);
        _turnsElapsedPerNinja = new Dictionary<int, int>(3);
        _areNinjasAlive = new bool[3];
        for (int i=0; i<3; ++i)
        {
            _areNinjasAlive[i] = true;
        }
        Player player = GetComponent<Player>();
        player.onEnterMoveState = OnTurnElapsed;
        NinjaManager.Instance.InitNinjasDeathAction(player.PlayerType.PlayerIndex, OnNinjaKilled);
    }

    private void OnTurnElapsed()
    {
        for (int i = 0; i<3; ++i)
        {
            if (!_areNinjasAlive[i])
            {
                ++_turnsElapsedPerNinja[i];
                if (_turnsElapsedPerNinja[i]>=_turnsDelay)
                {
                    int playerIndex = GetComponent<Player>().PlayerType.PlayerIndex;
                    Ninja ninja = NinjaManager.Instance.GetNinja(playerIndex, i);
                    RespawnNinja(ninja);
                }
            }
        }
    }

    private void OnNinjaKilled(Ninja ninja)
    {
        int ninjaIndex = ninja.ninjaIndex;     
        _areNinjasAlive[ninjaIndex] = false;
        if (_turnsDelay == 0)
        {
            RespawnNinja(ninja);
        }
    }

    private bool GetAvailableRespawnPoint(out Vector2Int respawnPoint)
    {
        bool foundValidRespawnPoint = false;
        int respawnPointIndex = -1;
        Vector2Int respawnTile = new Vector2Int(0,0);
        int i = 0;

        while (!foundValidRespawnPoint && i < 3)
        {
            respawnPointIndex = Random.Range(0, respawnPoints.Length);
            respawnTile = respawnPoints[respawnPointIndex];
            if (!IsTileOccupied(respawnTile))
            {
                foundValidRespawnPoint = true;
            }
            ++i;
        }
        
        if (foundValidRespawnPoint)
        {
            respawnPoint = respawnTile;
            return true;
        }

        respawnPoint = GameManager.Instance.vector2IntException;
        return false;
    }

    private bool IsTileOccupied(Vector2Int gridCoords)
    {
        List<Ninja> allNinjas = new List<Ninja>();
        allNinjas.AddRange(NinjaManager.Instance.GetAllNinjaForPlayer(0));
        allNinjas.AddRange(NinjaManager.Instance.GetAllNinjaForPlayer(1));

        foreach (Ninja ninja in allNinjas)
        {
            if  (ninja.GetPathOrigin() == gridCoords)
            {
                return true;
            }
        }

        return false;
    }

    private void RespawnNinja(Ninja ninja)
    {
        int ninjaIndex = ninja.ninjaIndex;
        
        Vector2Int respawnPoint = new Vector2Int(0,0);
        if (GetAvailableRespawnPoint(out respawnPoint))
        {
            ninja.RespawnAt(respawnPoint);

            _turnsElapsedPerNinja[ninjaIndex] = 0;
            _areNinjasAlive[ninjaIndex] = true;
        }
        else
        {
            --_turnsElapsedPerNinja[ninjaIndex]; //we need to give one more turn for this ninja to respawn
        }
    }
}