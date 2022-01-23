using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    //subscribe the stuff in this class to delegates in the input handler
    private void Awake()
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);

        player.onDrawPathInput = OnDrawInputReceived;
    }

    private void OnDrawInputReceived(GridSystem.Directions direction)
    {

    }
}
