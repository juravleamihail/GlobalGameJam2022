using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    public GridSystem(uint gridSize, float tileSize)
    {
        _GridSize = gridSize;
        _TileSize = tileSize;
        _Grid = new Transform[gridSize + 1, gridSize + 1]; 
        //we use +1 because arrays start at (0, 0) but our grid starts at (1, 1)
        // TODO maybe change this and make the grid itself start at (0, 0) also?
    }

    protected uint _GridSize;
    protected float _TileSize;

    protected Transform[,] _Grid;

    public enum Directions
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    //use these to signal exceptions; negative values for positions should not be used anywhere in the game
    //these are meant to be const, but Unity won't allow const Vector3
    private Vector3 _vector3Exception = new Vector3(-100f, -100f, -100f);
    private Vector2Int _vector2IntException = new Vector2Int(-1, -1);

    public void InitGameObjectConnection(GameObject gridContainer)
    {
        Transform[] tiles = gridContainer.GetComponentsInChildren<Transform>();
        foreach (Transform tile in tiles)
        {
            //we assume that the tiles are placed correctly
            //and that each tile has the pivot at + (0f, 0f, 0f) relative to the neighboring edge of the previous tile
            // this needs flipping the assets currently being used, so we should never refer to local space

            Vector3 tileCenter = tile.position + new Vector3(_TileSize / 2, 0f, _TileSize / 2);
            Vector2Int gridCoords = ConvertVector3ToGridCoords(tileCenter.x, tileCenter.z);

            if (!IsOnGrid(gridCoords))
            {
                Debug.Log("Tile " + tile.gameObject.name + " at " + tile.position + " is placed outside the logical grid.");
                continue;
            }

            _Grid[gridCoords.x, gridCoords.y] = tile;
        }
    }
    
    public Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
    {
        if (!IsOnGrid(new Vector2Int((int)gridX, (int)gridY)))
        {
            return _vector3Exception;
        }

        //we assume the grid has the origin (0f, 0f, 0f)
        //TODO if this leads to bugs, get the origin of the map and use that (avoid caching stuff)
        //TODO if y = 0 alone leads to bugs, get the baseline y of the map and use that
        Vector3 centerOfFirstTile = new Vector3(_TileSize / 2, 0f, _TileSize / 2);

        //we subtract 1 from the x and y because we already have the coords for the first tile resolved
        uint offsetX = gridX - 1;
        uint offsetZ = gridY - 1;

        float posX = centerOfFirstTile.x + offsetX * _TileSize;
        float posZ = centerOfFirstTile.z + offsetZ * _TileSize;
        return new Vector3(posX, 0f, posZ);
    }

    public Vector2Int ConvertVector3ToGridCoords(float worldX, float worldZ)
    {
        //this assumes that the grid has the origin at (0f, 0f, 0f)
        //TODO implement a parameter for the grid origin instead (avoid caching stuff)

        int gridX = Mathf.FloorToInt(worldX / _TileSize) + 1;
        int gridY = Mathf.FloorToInt(worldZ / _TileSize) + 1;

        //check that the given X and Z map coords are valid in the grid
        if (!IsOnGrid(new Vector2Int(gridX, gridY)))
        {
            return _vector2IntException;
        }

        return new Vector2Int(gridX, gridY);
    }
    
    public Vector2Int MoveOneTileOnGrid(uint currentGridX, uint currentGridY, Directions direction)
    {
        Vector2Int increment = new Vector2Int(0, 0);
        switch (direction)
        {
            case Directions.None:
                increment = new Vector2Int(0, 0);
                break;
            case Directions.Up:
                increment = new Vector2Int(0, 1);
                break;
            case Directions.Down:
                increment = new Vector2Int(0, -1);
                break;
            case Directions.Left:
                increment = new Vector2Int(-1, 0);
                break;
            case Directions.Right:
                increment = new Vector2Int(1, 0);
                break;
        }

        Vector2Int result = new Vector2Int((int)currentGridX, (int)currentGridY) + increment;

        if (!IsOnGrid(result))
        {
            return _vector2IntException;
        }

        return result;
    }
    
    public Vector3 MoveOneTileInWorld(Vector3 currentPosition, Directions direction)
    {
        //this assumes the current position has y == 0f;
        //we will not do a check for this because of potential approximation errors with floats
        
        Vector2Int posOnGrid = ConvertVector3ToGridCoords(currentPosition.x, currentPosition.z);
        if (!IsValid(posOnGrid))
        {
            return _vector3Exception;
        }
        
        posOnGrid = MoveOneTileOnGrid((uint)posOnGrid.x, (uint)posOnGrid.y, direction);
        if (!IsValid(posOnGrid))
        {
            return _vector3Exception;
        }

        Vector3 result = ConvertGridCoordsToVector3((uint)posOnGrid.x, (uint)posOnGrid.y);
        if (!IsValid(result))
        {
            return _vector3Exception;
        }

        return result;
    }

    public bool IsOnGrid(Vector2Int gridPos)
    {
        if (gridPos.x < 1 || gridPos.y < 1 || gridPos.x > _GridSize || gridPos.y > _GridSize)
        {
            return false;
        }
        return true;
    }

    public bool IsOnGrid(Vector3 worldPos)
    {
        Vector2Int gridPos = ConvertVector3ToGridCoords(worldPos.x, worldPos.z);
        return IsValid(gridPos);
    }

    protected bool IsValid(Vector3 pos)
    {
        //negative values are only used to signal exceptions
        if (pos.x < 0)
        {
            return false;
        }
        return true;
    }

    protected bool IsValid(Vector2Int pos)
    {
        //negative values are only used to signal exceptions
        if (pos.x < 0)
        {
            return false;
        }
        return true;
    }
}