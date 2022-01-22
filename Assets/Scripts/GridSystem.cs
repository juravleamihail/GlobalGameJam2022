using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    public GridSystem(uint gridSize, float tileSize)
    {
        _GridSize = gridSize;
        _TileSize = tileSize;
    }

    protected uint _GridSize;
    protected float _TileSize;

    public Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
    {
        if (gridX == 0 || gridY == 0 || gridX > _GridSize || gridY > _GridSize)
        {
            return new Vector3(-100f, -100f, -100f);
            //we can use these negative values to signal an exception
        }
        
        //TODO if y = 0 leads to bugs, get the baseline y of the scene and use that
        Vector3 centerOfFirstTile = new Vector3(_TileSize / 2, 0f, _TileSize / 2);

        //we subtract 1 from the x and y because we already have the coords for the first tile resolved
        uint offsetX = gridX - 1;
        uint offsetZ = gridY - 1;

        float posX = centerOfFirstTile.x + offsetX * _TileSize;
        float posZ = centerOfFirstTile.z + offsetZ * _TileSize;
        return new Vector3(posX, 0f, posZ);
    }
}