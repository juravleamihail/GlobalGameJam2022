using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NinjaManager : Singleton<NinjaManager>
{
    [SerializeField]private Vector2Int[] _p1NinjaSpawnPoints;
    [SerializeField]private Vector2Int[] _p2NinjaSpawnPoints;
    [SerializeField]private NinjaTypeListSO _ninjaTypeList;

    private Dictionary<int, List<Ninja>> _allNinjas;

    public Vector2Int[] p1NinjaSpawnPoints { get { return _p1NinjaSpawnPoints; } }
    public Vector2Int[] p2NinjaSpawnPoints { get { return _p2NinjaSpawnPoints; } }

    public override void Awake()
    {
        base.Awake();
        _allNinjas = new Dictionary<int, List<Ninja>>();
    }

    private void Start()
    {
        SpawnNinjasForPlayer(0, _p1NinjaSpawnPoints);
        SpawnNinjasForPlayer(1, _p2NinjaSpawnPoints);
    }

    internal void SpawnNinjasForPlayer(int playerIndex, Vector2Int[] spawnPoints)
    {
        List<Ninja> ninjaList = new List<Ninja>();
        Transform prefab = _ninjaTypeList.ninjaList[playerIndex].Prefab.transform;

        var gm = GameManager.Instance;
        
        for (int i = 0; i<spawnPoints.Length; ++i)
        {
            Vector2Int spawnPoint = spawnPoints[i];
            Vector3 spawnPointInWorld = gm.ConvertGridCoordsToVector3((uint)spawnPoint.x, (uint)spawnPoint.y);
            Transform ninjaTransform = Instantiate(prefab, spawnPointInWorld, prefab.transform.rotation);
            ninjaTransform.LookAt(new Vector3(gm.GetGridSize/2,0,gm.GetGridSize/2));
            Ninja ninja = ninjaTransform.gameObject.GetComponent<Ninja>();
            if (ninja == null)
            {
                Debug.Log("Error: " + prefab + " ninja prefab needs to have Ninja script attached.");
                return;
            }
            ninja.Init(_ninjaTypeList.ninjaList[playerIndex], i);
            ninjaList.Add(ninja);
        }

        _allNinjas.Add(playerIndex, ninjaList);
    }

    public void TryDrawPath(int playerIndex, int ninjaIndex, GridSystem.Directions direction)
    {
        if (!CanSelectedNinjaDraw(playerIndex, ninjaIndex))
        {
            //TODO give audio-visual feedback for denied drawing
            return;
        }

        Ninja ninja = _allNinjas[playerIndex][ninjaIndex];
        ninja.TryDrawPath(direction);
    }

    public void UndoDrawPath(int playerIndex, int ninjaIndex, bool longUndo)
    {
        //if only one ninja moves per turn, undo works regardless of selected ninja
        if (GameManager.Instance.canMoveOnlyOneNinjaPerTurn)
        {
            int foundNinjaIndex = -1;
            if (IsAnotherNinjaDrawing(playerIndex, ninjaIndex, out foundNinjaIndex))
            {
                if (foundNinjaIndex != -1)
                {
                    ninjaIndex = foundNinjaIndex;
                }
            }
        }

        Ninja ninja = _allNinjas[playerIndex][ninjaIndex];
        ninja.UndoDrawPath(longUndo);
    }

    private bool CanSelectedNinjaDraw(int playerIndex, int ninjaIndex)
    {
        if (GameManager.Instance.canMoveOnlyOneNinjaPerTurn)
        {
            return !IsAnotherNinjaDrawing(playerIndex, ninjaIndex);
        }
        
        return true;
    }

    private bool IsAnotherNinjaDrawing(int playerIndex, int ninjaIndex)
    {
        List<Ninja> ninjaList = _allNinjas[playerIndex];
        for (int i = 0; i < ninjaList.Count; ++i)
        {
            if (i == ninjaIndex)
            { 
                continue;
            }

            Ninja ninja = ninjaList[i];
           if (ninja.IsDrawing())
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAnotherNinjaDrawing(int playerIndex, int ninjaIndex, out int foundNinjaIndex)
    {
        List<Ninja> ninjaList = _allNinjas[playerIndex];
        for (int i = 0; i < ninjaList.Count; ++i)
        {
            if (i == ninjaIndex)
            {
                continue;
            }

            Ninja ninja = ninjaList[i];
            if (ninja.IsDrawing())
            {
                foundNinjaIndex = i;
                return true;
            }
        }
        foundNinjaIndex = -1;
        return false;
    }

    public int StartMovePhase(Action onCompleteCb)
    {
        List<Ninja> ninjaList = new List<Ninja>(_allNinjas[0]);
        ninjaList.AddRange(_allNinjas[1]);
        foreach (Ninja ninja in ninjaList)
        {
            ninja.StartMovePhase(onCompleteCb);
        }

        return ninjaList.Count;
    }

    public bool IsNinjaAtLocation(Vector2Int tileCoords, out int playerIndex)
    {
        if (IsNinjaOfPlayer(0, tileCoords))
        {
            playerIndex = 0;
            return true;
        }
        if (IsNinjaOfPlayer(1, tileCoords))
        {
            playerIndex = 1;
            return true;
        }
        playerIndex = -1;
        return false;
    }

    private bool IsNinjaOfPlayer(int playerIndex, Vector2Int tileCoords)
    {
        List<Ninja> ninjaList;
        ninjaList = _allNinjas[playerIndex];
        foreach (Ninja ninja in ninjaList)
        {
            Vector3 pos = ninja.transform.position;
            Vector2Int gridPos = GameManager.Instance.ConvertVector3CoordsToGrid(pos.x, pos.z);
            if (gridPos == tileCoords)
            {
                return true;
            }
        }
        return false;
    }

    internal List<Ninja> GetAllNinjaForPlayer(int playerID)
    {
        if(!_allNinjas.TryGetValue(playerID, out var listOfNinjas))
        {
            return null;
        }

        return listOfNinjas;
    }

    public Ninja GetNinjaOnTile(Vector2Int tileCoords)
    {
        for (int i=0; i<2; ++i)
        {
            foreach (Ninja ninja in _allNinjas[i])
            {
                if (ninja.GetGridPositionViaPath() == tileCoords)
                {
                    return ninja;
                }
            }
        }

        return null;
    }

    public Ninja GetNinja(int playerID, int ninjaIndex)
    {
        return _allNinjas[playerID][ninjaIndex];
    }

    public bool IsDestinationOfFriendlyNinja(int playerIndex, int ninjaIndex, Vector2Int destination)
    {
        List<Ninja> ninjaList = _allNinjas[playerIndex];
        for (int i = 0; i < ninjaList.Count; ++i)
        {
            if (i == ninjaIndex)
            {
                continue;
            }

            Ninja ninja = ninjaList[i];
            {
                //TODO obvious source of spaghetti code in this scope, should change
                Path path = ninja.gameObject.GetComponent<Path>();
                if (path.GetDestination() == destination)
                {
                    Debug.Log("Tile at " + destination.x + ", " + destination.y + " is the destination of friendly ninja.");
                    return true;
                }
            }
        }
        return false;
    }

    /*internal void AddNinja(int playerIndex)
    {
        DictionaryEntry<int, List<Ninja
        
        if(ninja == null)
        {
            return;
        }

        allNinjas.Add(ninja);
    }


    public List<Ninja> GetNinjaByPlayer(int playerIndex)
    {
        return allNinjas.FindAll(ninja => ninja.NinjaType.PlayerIndex == playerIndex);
    }*/

    public void InitNinjasDeathAction(int playerId, UnityAction<Ninja> onNinjaDeath)
    {
        List <Ninja> ninjaList = GetAllNinjaForPlayer(playerId);
        foreach (Ninja ninja in ninjaList)
        {
            ninja.AddToOnNinjaDeath(onNinjaDeath);
        }
    }
}
