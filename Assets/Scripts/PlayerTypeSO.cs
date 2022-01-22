using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Player/PlayerTypeSO")]
public class PlayerTypeSO : ScriptableObject
{
    public string NameText;
    public int PlayerIndex;
    public GameObject Prefab;
}
