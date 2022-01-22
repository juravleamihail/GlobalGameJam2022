using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerTypeListSO")]
public class PlayerTypeListSO : ScriptableObject
{
    public List<PlayerTypeSO> playerTypeList;
}

