using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector2Int pos;
    Unit unitOnTile;
    public PathTile path;
    public TileData(Tile _tile)
    {
        pos = _tile.pos;
        unitOnTile = _tile.GetUnitOnTile();
        path = _tile.pathTile;
    }
    bool IsUnitOnTile()
    {
        if (unitOnTile != null)
        {
            return true;
        }
        else
        {

            return false;
        }
    }
}
