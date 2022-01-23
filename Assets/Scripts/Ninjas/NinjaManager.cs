using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaManager : Singleton<NinjaManager>
{
    [SerializeField]private Vector2Int[] p1NinjaSpawnPoints;
    [SerializeField]private Vector2Int[] p2NinjaSpawnPoints;
    [SerializeField]private NinjaTypeListSO _ninjaTypeList;

    private Dictionary<int,List<Ninja>> allNinjas;

    public int debugSelectedP1Ninja;
    public int debugSelectedP2Ninja;

    private void Awake()
    {
        base.Awake();
        allNinjas = new Dictionary<int, List<Ninja>>();
    }

    private void Start()
    {
        SpawnNinjasForPlayer(0, p1NinjaSpawnPoints);
        SpawnNinjasForPlayer(1, p2NinjaSpawnPoints);
    }

    internal void SpawnNinjasForPlayer(int playerIndex, Vector2Int[] spawnPoints)
    {
        List<Ninja> ninjaList = new List<Ninja>();
        Transform prefab = _ninjaTypeList.ninjaList[playerIndex].Prefab.transform;

        foreach (Vector2Int spawnPoint in spawnPoints)
        {
            Vector3 spawnPointInWorld = GameManager.Instance.ConvertGridCoordsToVector3((uint)spawnPoint.x, (uint)spawnPoint.y);
            Transform ninjaTransform = Instantiate(prefab, spawnPointInWorld, prefab.transform.rotation);
            Ninja ninja = ninjaTransform.gameObject.GetComponent<Ninja>();
            if (ninja == null)
            {
                Debug.Log("Error: " + prefab + " ninja prefab needs to have Ninja script attached.");
                return;
            }
            ninja.NinjaType = _ninjaTypeList.ninjaList[playerIndex];
            ninjaList.Add(ninja);
        }

        allNinjas.Add(playerIndex, ninjaList);
    }

    public void TryDrawPath(int playerIndex, int ninjaIndex, GridSystem.Directions direction)
    {
        if (!CanSelectedNinjaDraw(playerIndex, ninjaIndex))
        {
            //TODO give audio-visual feedback for denied drawing
            return;
        }

        Ninja ninja = allNinjas[playerIndex][ninjaIndex];
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

        Ninja ninja = allNinjas[playerIndex][ninjaIndex];
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
        List<Ninja> ninjaList = allNinjas[playerIndex];
        for (int i = 0; i < ninjaList.Count; ++i)
        {
            if (i == ninjaIndex)
            { 
                continue;
            }

            Ninja ninja = ninjaList[i];
            if (ninja.hasPath)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAnotherNinjaDrawing(int playerIndex, int ninjaIndex, out int foundNinjaIndex)
    {
        List<Ninja> ninjaList = allNinjas[playerIndex];
        for (int i = 0; i < ninjaList.Count; ++i)
        {
            if (i == ninjaIndex)
            {
                continue;
            }

            Ninja ninja = ninjaList[i];
            if (ninja.hasPath)
            {
                foundNinjaIndex = i;
                return true;
            }
        }
        foundNinjaIndex = -1;
        return false;
    }

    public void StartMovePhase()
    {
        List<Ninja> ninjaList = allNinjas[0];
        ninjaList.AddRange(allNinjas[1]);
        foreach (Ninja ninja in ninjaList)
        {
            ninja.StartMovePhase();
        }
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
        ninjaList = allNinjas[0];
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
}
