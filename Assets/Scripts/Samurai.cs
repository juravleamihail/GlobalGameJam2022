using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai : MonoBehaviour
{
    public int PlayerIndex { get; private set; }


    public void Init(SamuraiSO samuraiSO)
    {
        PlayerIndex = samuraiSO.PlayerIndex;
    }
}
