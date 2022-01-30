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

    public float TileSize { get { return _TileSize; } }

    public enum Directions
    {
        Up,
        Right,
        Down,
        Left,
        None
    }

    public void InitGameObjectConnection(GameObject gridContainer)
    {
        Transform[] tiles = gridContainer.GetComponentsInChildren<Transform>();
        foreach (Transform tile in tiles)
        {
            if (tile == gridContainer)
            {
                continue;
            }

            if (tile.GetComponent<Tile>() == null)
            {
                Debug.Log("Tile object " + tile.name + " is not actually a tile.");
                continue;
            }
            //we assume that the tiles are placed correctly
            //and that each tile has the pivot at + (0f, 0f, 0f) relative to the neighboring edge of the previous tile
            // this may need flipping the assets currently being used, so we should never refer to local space

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
    
    public Transform GetTileObjectAt(uint gridX, uint gridZ)
    {
        return _Grid[gridX, gridZ];
    }

    public Vector2Int GetGridCoordsOfTileObject(Transform tile)
    {
        for (int x = 1; x <= _GridSize; ++x)
        {
            for (int y = 1; y <= _GridSize; ++y)
            {
                Transform candidate = _Grid[x, y];
                if (candidate == tile)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        Debug.Log("Tile object " + tile.gameObject.name + " was not found in grid.");
        return GameManager.Instance.vector2IntException;
    }

    public Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
    {
        if (!IsOnGrid(new Vector2Int((int)gridX, (int)gridY)))
        {
            return GameManager.Instance.vector3Exception;
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
            return GameManager.Instance.vector2IntException;
        }

        return new Vector2Int(gridX, gridY);
    }
    
    public Vector2Int GetAdjacentTileOnGrid(uint currentGridX, uint currentGridY, Directions direction)
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
            return GameManager.Instance.vector2IntException;
        }

        return result;
    }
    
    public Vector3 GetAdjacentTileInWorld(Vector3 currentPosition, Directions direction)
    {
        //this assumes the current position has y == 0f;
        //we will not do a check for this because of potential approximation errors with floats
        
        Vector2Int posOnGrid = ConvertVector3ToGridCoords(currentPosition.x, currentPosition.z);
        if (!IsValid(posOnGrid))
        {
            return GameManager.Instance.vector3Exception;
        }
        
        posOnGrid = GetAdjacentTileOnGrid((uint)posOnGrid.x, (uint)posOnGrid.y, direction);
        if (!IsValid(posOnGrid))
        {
            return GameManager.Instance.vector3Exception;
        }

        Vector3 result = ConvertGridCoordsToVector3((uint)posOnGrid.x, (uint)posOnGrid.y);
        if (!IsValid(result))
        {
            return GameManager.Instance.vector3Exception;
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

    //gets a grid direction relative to a given vector (meant to be used with a ninja's transform.forward)
    public Directions GetRelativeDirection(GridSystem.Directions inDirection, Vector3 inVector)
    {
        //we will need the angle between the Up position in the grid and the vector
        //we will assume that the map is placed correctly and that GridSystem.Directions.Up corresponds with Vector3.forward
        Vector3 gridUp = Vector3.forward;
        float dotProduct = Vector3.Dot(inVector, gridUp);
        float cos = dotProduct / (inVector.magnitude * gridUp.magnitude);
        float angleInRadians = Mathf.Acos(cos);

        if (Mathf.Approximately(angleInRadians, 0f))
        {
            return inDirection;
        }
        if (Mathf.Approximately(angleInRadians, Mathf.PI / 2))
        {
            return AddAngleToDirection(inDirection, 1);
        }
        if (Mathf.Approximately(angleInRadians, Mathf.PI))
        {
            return AddAngleToDirection(inDirection, 2);
        }
        if (Mathf.Approximately(angleInRadians, 3 * Mathf.PI / 2))
        {
            return AddAngleToDirection(inDirection, 3);
        }
        Debug.Log("Angle should be a multiple of 90 degrees.");
        return Directions.None; //this should signal an exception somewhere
    }

    //this adds an angle to the direction; the angle must be a 90 degrees * the multiplier that is the second parameter
    private Directions AddAngleToDirection(Directions inDirection, int ninetyDegreeAngleMultipler)
    {
        int result = ((int)inDirection + ninetyDegreeAngleMultipler) % 4; //there are four directions starting at 0, so a result of 4 should cycle back to 0
        return (Directions)result;
    }

    public void ClearInvasions()
    {
        foreach (Transform tileTransform in _Grid)
        {
            if (tileTransform == null)
            {
                continue;
            }

            Tile tile = tileTransform.GetComponent<Tile>();
            if (tile != null)
            {
                tile.ClearInvasion();
            }
        }
    }
}