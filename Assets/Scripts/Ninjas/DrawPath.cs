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
        ninja.onUndoInput = OnUndoInputReceived;
    }

    private void OnDrawInputReceived(GridSystem.Directions direction)
    {
        Path path = gameObject.GetComponent<Path>();
        Vector2Int currentDestination = path.GetDestination();

        Vector2Int newDestination = GameManager.Instance.GetAdjacentTileOnGrid((uint)currentDestination.x, (uint)currentDestination.y, direction);

        if (!path.CanBeDestination(newDestination))
        {
            //TODO give audio-visual feedback for "destination not allowed"
            return;
        }

        path.SetNewDestination(newDestination);
        path.PathDrawFeedback(newDestination);
    }

    private void OnUndoInputReceived(bool longUndo)
    {
        if (longUndo)
        {
            UndoFullPath();
        }
        else
        {
            UndoOneTile();
        }
    }

    public void UndoOneTile()
    {
        Path path = gameObject.GetComponent<Path>();
        if (path.IsOnlyCurrentTile())
        {
            //TODO either this should give some kind of meaningful audiovisual feedback
            //or the successful undo should, or both
            //so the player knows when a path was fully cleared
            return;
        }

        path.RemoveLastTileFromPath();
        
        if (path.IsOnlyCurrentTile())
        {
            SetNinjaHasPath(false);
        }
    }

    public void UndoFullPath()
    {
        Path path = gameObject.GetComponent<Path>();
        while (!path.IsOnlyCurrentTile())
        {
            path.RemoveLastTileFromPath();
        }
        SetNinjaHasPath(false);
    }
}
