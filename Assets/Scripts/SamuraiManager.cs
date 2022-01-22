using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiManager : MonoBehaviour
{
    private static SamuraiManager _instance;
    public static SamuraiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SamuraiManager>();
            }

            return _instance;
        }
    }

    private List<Samurai> samuraiList;

   
    private void Awake()
    {
        samuraiList = new List<Samurai>();

        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != null)
        {
            Destroy(this);
        }
    }

    internal void AddSamurai(Samurai samurai)
    {
       if(samurai==null)
        {
            return;
        }

        samuraiList.Add(samurai);
    }


    public List<Samurai> GetSamuraiByPlayer(int playerIndex)
    {
        return samuraiList.FindAll(samurai => samurai.PlayerIndex == playerIndex);
    }
}
