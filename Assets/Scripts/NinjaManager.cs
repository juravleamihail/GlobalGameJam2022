using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaManager : MonoBehaviour
{
    private static NinjaManager _instance;
    public static NinjaManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Dictionary<int,List<Ninja>> ninjaList;

   
    private void Awake()
    {
        ninjaList = new Dictionary<int, List<Ninja>>();

        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != null)
        {
            Destroy(this);
        }
    }

    /*internal void AddNinja(int playerIndex)
    {
       if(ninja == null)
        {
            return;
        }

        ninjaList.Add(ninja);
    }


    public List<Ninja> GetNinjaByPlayer(int playerIndex)
    {
        return ninjaList.FindAll(ninja => ninja.NinjaType.PlayerIndex == playerIndex);
    }*/

    
}
