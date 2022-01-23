using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    //this script is meant to be placed on each ninja
    private void Awake()
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        ninja.onDrawPathInput = OnDrawInputReceived;
    }

    private void OnDrawInputReceived(GridSystem.Directions direction)
    {
        Path path = gameObject.GetComponent<Path>();
        Vector2 currentDestination = path.GetDestination();

        Vector2Int newDestination = GameManager.Instance.GetAdjacentTileOnGrid((uint)currentDestination.x, (uint)currentDestination.y, direction);

        if (!path.CanBeDestination(newDestination))
        {
            //TODO give audio-visual feedback for "destination not allowed"
            return;
        }

        path.SetNewDestination(newDestination);

        PathDrawFeedback(newDestination);
    }

    private void PathDrawFeedback(Vector2Int newDestination)
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);
        Material material = player.pathDrawMaterial;

        Transform destinationTile = GameManager.Instance.GetTileObjectAt((uint)newDestination.x, (uint)newDestination.y);
        Renderer tileRenderer = destinationTile.gameObject.GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            tileRenderer.material = material;
        }

        //TODO add invisibility logic versions (with an editor-exposed design switch between them)

        //TODO maybe add audio feedback
    }
}
