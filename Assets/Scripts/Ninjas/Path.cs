using System;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    //this script is meant to be placed on each ninja
    public Action onClearPath;
    public Action onInitPath;

    private List<Vector2Int> _Path;

    private void Start()
    {
        _Path = new List<Vector2Int>();
        Init();
    }

    public void Init()
    {
        Vector3 ninjaPos = transform.position;
        Vector2Int currentTile = GameManager.Instance.ConvertVector3CoordsToGrid(ninjaPos.x, ninjaPos.z);
        if (GameManager.Instance.IsOnGrid(currentTile))
        {
            _Path.Add(currentTile);
        }
        onInitPath?.Invoke();
    }

    public void ClearPath()
    {
        _Path.Clear();

        for (int i = 0; i<_Path.Count; ++i)
        {
            ApplyDefaultMaterial(i);
        }

        onClearPath?.Invoke();
    }

    public Vector2Int GetDestination()
    {
        return _Path[_Path.Count - 1];
    }

    public Vector2Int GetNextTile()
    {
        return _Path[1]; //0 is always the tile where the ninja is
    }

    public Vector2Int GetCurrentTile()
    {
        return _Path[0];
    }

    public bool IsOnlyCurrentTile()
    {
        return (_Path.Count == 1);
    }    

    public void SetNewDestination(Vector2Int gridPos)
    {
        if (GameManager.Instance.invisiblePaths)
        {
            Vector2Int prevTile = GetDestination();
            StartDisappearingTimer(prevTile);
        }
        _Path.Add(gridPos);
    }

    public void StartDisappearingTimer(Vector2Int gridPos)
    {
        Transform tileObject = GameManager.Instance.GetTileObjectAt((uint)gridPos.x, (uint)gridPos.y);
        tileObject.GetComponent<Tile>().StartDisappearTimer();
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

        ApplyDefaultMaterial(_Path.Count-1);
        _Path.RemoveAt(_Path.Count - 1);

        if (GameManager.Instance.invisiblePaths)
        {
            //restore feedback color to the new last-tile (if it's not the first one)
            if (IsOnlyCurrentTile())
            {
                return;
            }
            PathDrawFeedback(GetDestination());
        }
    }
    public void ApplyDefaultMaterial(int index)
    {
        Vector2Int lastTileCoords = _Path[index];
        Transform lastTileTransform = GameManager.Instance.GetTileObjectAt((uint)lastTileCoords.x, (uint)lastTileCoords.y);
        Tile lastTile = lastTileTransform.gameObject.GetComponent<Tile>();
        Material defaultMaterial = lastTile.defaultMaterial;

        Renderer renderer = lastTile.gameObject.GetComponent<Renderer>();
        renderer.material = defaultMaterial;
    }

    public bool CanBeDestination(Vector2Int destination)
    {
        //TODO add all conditions here:

        // 1. friendly ninja already on that tile
        int foundNinjaPlayerIndex = -1;
        if(NinjaManager.Instance.IsNinjaAtLocation(destination, out foundNinjaPlayerIndex))
        {
            Ninja thisNinja = gameObject.GetComponent<Ninja>();
            int thisNinjaPlayerIndex = thisNinja.GetPlayerIndex();
            if (thisNinjaPlayerIndex == foundNinjaPlayerIndex)
            {
                return false;
            }
        }

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
        if (IsDestinationOfFriendlyNinja(destination))
        {
            return false;
        }
        
        //7. the tile is already part of this path
        if (!GameManager.Instance.canPathIntersectItself && _Path.Contains(destination))
        {
            return false;
        }
           
        return true;
    }

    public bool IsDestinationOfFriendlyNinja(Vector2Int destination)
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);
        int ninjaIndex = player.selectedNinjaIndex; //TODO implement and use ninja.ninjaIndex instead of this
        return NinjaManager.Instance.IsDestinationOfFriendlyNinja(playerIndex, ninjaIndex, destination);
    }    

    public void OnMovedOneTile()
    {
        //TODO make a call to this from the movement logic
        _Path.RemoveAt(0);
    }

    public void PathDrawFeedback(Vector2Int newDestination)
    {
        Ninja ninja = gameObject.GetComponent<Ninja>();
        int playerIndex = ninja.GetPlayerIndex();
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);
        Material material = player.pathDrawMaterial;

        Transform destinationTile = GameManager.Instance.GetTileObjectAt((uint)newDestination.x, (uint)newDestination.y);
        Renderer tileRenderer = destinationTile.gameObject.GetComponent<Renderer>();
        tileRenderer.material = material;

        //TODO add invisibility logic versions (with an editor-exposed design switch between them)

        //TODO maybe add audio feedback
    }

    public Vector2Int GetOrigin()
    {
        return _Path[0];
    }
}
