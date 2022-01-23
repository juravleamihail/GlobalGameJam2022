using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    //this script is meant to be placed on each ninja
    private void Awake()
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);

        player.onDrawPathInput = OnDrawInputReceived;
    }

    private void OnDrawInputReceived(GridSystem.Directions direction)
    {
        if (!CanThisPlayerDraw())
        {
            //TODO give audio-visual feedback for denied drawing
            return;
        }

        Path path = gameObject.GetComponent<Path>();
        Vector2 currentDestination = path.GetDestination();

        Vector2Int newDestination = GameManager.Instance.GetAdjacentTileOnGrid((uint)currentDestination.x, (uint)currentDestination.y, direction);

        if (!path.CanBeDestination(newDestination))
        {
            //TODO give audio-visual feedback for "destination not allowed"
            return;
        }

        path.SetNewDestination(newDestination);
    }

    private void PathDrawFeedback()
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);

        Material material = player.pathDrawMaterial;


    }

    private bool CanThisPlayerDraw()
    {
        //TODO if we have a single path per player per turn, check for that here
        //TODO should this method call a similarly-named method in the Player class? probably
        return true;
    }
}
