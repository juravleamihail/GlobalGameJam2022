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
        if (!CanSelectedNinjaDraw())
        {
            //TODO give audio-visual feedback for denied drawing
            return;
        }

        Ninja ninja = allNinjas[playerIndex][ninjaIndex];
        ninja.TryDrawPath(direction);
    }

    private bool CanSelectedNinjaDraw()
    {
        //TODO implement:
        //if we have a single path per player per turn, check for that here
        
        //should this method be here, or in Player, or Ninja, or elsewhere?
        
        return true;
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
