using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    public NinjaTypeSO NinjaType { get; set; }

    public void Init(NinjaTypeSO ninjaTypeSO)
    {
        NinjaType = ninjaTypeSO;
    }

    public int GetPlayerIndex()
    {
        return NinjaType.PlayerIndex;
    }
}
