using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    [SerializeField]
    private PlayerTypeSO _playerType;

    public PlayerTypeSO PlayerType
    {
        get
        {
            return _playerType;
        }
    }
}
