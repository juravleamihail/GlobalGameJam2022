using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ninja/NinjaTypeSO")]
public class NinjaTypeSO : ScriptableObject
{
    public string NameText;
    public int PlayerIndex;
    public GameObject Prefab;
}
