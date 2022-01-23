using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    private List<Vector2Int> _Path;

    private void Awake()
    {
        ResetPath();
    }

    //TODO implement:
    //  while path length > 1
    //      ninja moves to first tile of path
    //      first tile of path is removed and array is offset to the left by 1

    private void ResetPath()
    {
        //this should only get called when using the Undo key (and in the Awake of this class, for consistency)
        //we shouldn't need to rely on resetting the path when the path is complete
        //the recursive implementation of movement should handle that automatically

        _Path.Clear();
        Vector3 ninjaPos = transform.position;
        Vector2Int currentTile = GameManager.Instance.ConvertVector3CoordsToGrid(ninjaPos.x, ninjaPos.y);
        if (GameManager.Instance.IsOnGrid(currentTile))
        {
            _Path.Add(currentTile);
        }
    }

    public Vector2Int GetDestination()
    {
        return _Path[_Path.Count - 1];
    }

    public void AddTileToPath(uint gridX, uint gridY)
    {
        if (!CanThisPlayerDraw())
        {
            //TODO give audio-visual feedback for denied drawing
            return;
        }

        Vector2Int gridPos = new Vector2Int((int)gridX, (int)gridY);
        if (!CanBeDestination(gridPos))
        {
            //TODO give audio-visual feedback for "destination not allowed"
            return;
        }

        _Path.Add(gridPos);
    }

    public void RemoveLastTileFromPath()
    {
        if (_Path.Count == 0)
        {
            Debug.Log("Error: Path length was zero.");
            return;
        }

        if (_Path.Count == 1)
        {
            //TODO give audiovisual feedback for "no more tiles to undo"
            return;
        }

        _Path.RemoveAt(_Path.Count - 1);
    }

    public bool CanBeDestination(Vector2Int destination)
    {
        //TODO add all conditions here:

        // 1. ninja already on that tile
        //  TODO keep an array of locations of ninjas for both players in the GameMode, compare to that array

        // 2. tile would be outside grid   
        if (!GameManager.Instance.IsOnGrid(destination))
        {
            Debug.Log("Tile with coordinates (" + destination.x + ", " + destination.y + ") doesn't exist on grid.");
            return false;
        }

        // 3. there is an obstacle in the way

        // 4. path destination is beyond max movement distance
        if (GameManager.Instance.isUsingMaxDistance)
        {
            //TODO implement max movement distance if needed
            Debug.Log("Max movement distance not yet implemented.");
        }

        // 5. path exceeds number of movement points
        if (GameManager.Instance.isUsingMovePoints)
        {
            if (_Path.Count - 1 == GameManager.Instance.movePoints)
            {
                return false;
            }
        }

        // 6. there is already another path of the same player with that destination
        // consider an edge case, if we will have multiple paths per player per turn:
        //  not all path tiles are destination tiles
        //  path tiles should be able to intersect each other, but not in the destination point
        //  for now, we are treating each tile as a destination in the code, so this might cause problems

        return true;
    }

    public bool CanThisPlayerDraw()
    {
        //TODO if we have a single path per player per turn, check for that here
        return true;
    }

    public void OnMovedOneTile()
    {
        //TODO make a call to this from the movement logic
        _Path.RemoveAt(0);
    }
}