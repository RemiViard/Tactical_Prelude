using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private static Grid _instance;
    public static Grid Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                GameObject gridManager = new GameObject();
                gridManager.name = "Grid";
                _instance = (Grid)gridManager.AddComponent(typeof(Grid));
                gridManager.tag = "Tile";
                return _instance;
            }
        }
    }
    public Vector2Int size = new Vector2Int(10, 10);
    Tile[,] tilesArray;
    public List<Tile> currentDrawTiles = new List<Tile>();
    GameObject tileprefab;
    public Pathfinder pathfinder = new Pathfinder();
    MapDataReader gridreader = new MapDataReader();
    [SerializeField] TextAsset mapData;
    // Start is called before the first frame update
    private void Awake()
    {
        _instance = this;
        tileprefab = (GameObject)Resources.Load("Prefab/Tile", typeof(GameObject));
        gameObject.tag = "Tile";
        InitGrid();
    }
    #region Save
    public void LoadMap()
    {
        if (mapData != null)
        {
            gridreader.ReadGrid(this, mapData);
        }
    }
    public void SaveMap(string name)
    {
        gridreader.SaveGrid(this, name);
    }
    #endregion
    #region Tool
    public void LoadGrid(Vector2Int _size)
    {
        TurnManager.Instance.playerController.UnselectTile();
        foreach (var army in GameManager.Instance.armies)
        {
            army.Obliterate();
        }
        foreach (var tile in tilesArray)
        {
            Destroy(tile.gameObject);
        }
        size = _size;
        InitGrid();
    }
    void InitGrid()
    {
        tilesArray = new Tile[(int)size.x, (int)size.y];
        transform.position = new Vector3(-size.x / 2, 0f, -size.y / 2);
        for (int i = 0; i < size.x; i++)
        {
            for (int y = 0; y < size.y; y++)
            {

                GameObject currentTile = Instantiate(tileprefab, transform);
                currentTile.transform.localPosition = new Vector3(i + 0.5f, 0, y + 0.5f);
                currentTile.GetComponent<Tile>().pos = new Vector2Int(i, y);
                tilesArray[i, y] = currentTile.GetComponent<Tile>();
            }
        }
    }
    bool IsOnGrid(Vector2Int _tilePosInGrid)
    {
        if (_tilePosInGrid.x < size.x && _tilePosInGrid.y < size.y && _tilePosInGrid.x >= 0 && _tilePosInGrid.y >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Tile GetTile(Vector2Int pos)
    {
        if (IsOnGrid(pos))
        {
            return tilesArray[pos.x, pos.y];
        }
        return null;
    }
    public Tile[,] GetTiles()
    {
        return tilesArray;
    }
    public Vector3 GetTilePos(Vector2Int _tilePosInGrid)
    {
        if (IsOnGrid(_tilePosInGrid))
        {
            return GetTile(_tilePosInGrid).transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
    public Vector2Int RangePosInGrid(Vector2Int _vector)
    {
        if (_vector.x >= size.x)
            _vector.x = size.x - 1;
        else if (_vector.x < 0)
        {
            _vector.x = 0;
        }
        if (_vector.y >= size.y)
            _vector.y = size.y - 1;
        else if (_vector.y < 0)
        {
            _vector.y = 0;
        }
        return _vector;
    }
    public List<Tile> GetNeighbors(Vector2Int _pos)
    {
        List<Tile> neighbors = new List<Tile>();
        for (int i = -1; i <= 1; i += 2)
        {
            Vector2Int neighborPos1 = new Vector2Int(_pos.x + i, _pos.y);
            Vector2Int neighborPos2 = new Vector2Int(_pos.x, _pos.y + i);
            if (CheckTileViability(neighborPos1))
            {
                neighbors.Add(GetTile(neighborPos1));
            }
            if (CheckTileViability(neighborPos2))
            {
                neighbors.Add(GetTile(neighborPos2));
            }
        }
        return neighbors;
    }
    public List<Tile> AddNeighbors(Vector2Int _pos, List<Tile> tiles)
    {
        foreach (var neighbor in GetNeighbors(_pos))
        {
            if (!tiles.Contains(neighbor))
            {
                tiles.Add(neighbor);
            }
        }
        return tiles;
    }
    public Vector2Int GetAvailableTile(Vector2Int _pos)
    {

        if (GetTile(_pos).IsUnitOnTile())
        {
            foreach (var neigbhor in GetNeighbors(_pos))
            {
                if (!neigbhor.IsUnitOnTile())
                {
                    return neigbhor.pos;
                }
            }
            Debug.Log("NoAvailableTile");
            return new Vector2Int(-1, -1);
            //TODO:recursive de malade bouilla
        }
        else
        {
            return _pos;
        }

    }
    public bool CheckTileViability(Vector2Int _pos)
    {
        if (GetTile(_pos) != null)
        {
            if (GetTile(_pos).IsWalkable())
            {
                return true;
            }
        }
        return false;
    }
    public void MoveOnGrid(Unit _unit, Vector2Int _destination)
    {
        GetTile(_unit.pos).RemoveUnitOnTile();
        GetTile(_destination).AddUnitOnTile(_unit);
        _unit.currentTile = GetTile(_destination);
    }

    public void SpawnOnGrid(Unit _unit, Vector2Int _pos)
    {
        GetTile(_pos).AddUnitOnTile(_unit);
        _unit.currentTile = GetTile(_pos);
    }

    public void RemoveFromGrid(Unit _unit)
    {
        GetTile(_unit.pos).RemoveUnitOnTile();
        _unit.currentTile = null;
    }
    public int Distance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    #endregion
    #region Pathfinder
    #region IA
    public void IAMovableTile(Unit _unit)
    {
        List<Tile> moveCloseList = new List<Tile>();
        List<Tile> rangeCloseList = new List<Tile>();
        List<Tile> pressureList = GetPressuredTiles(_unit);
        pathfinder.MoveTile(GetTile(_unit.pos), moveCloseList, rangeCloseList, pressureList, _unit);
        IARangeTile(_unit, moveCloseList, rangeCloseList, pressureList);

        HideMoveableTile();

        moveCloseList.Clear();
        rangeCloseList.Clear();
        pressureList.Clear();
    }
    public void IARangeTile(Unit _unit, List<Tile> _moveList, List<Tile> _rangeList, List<Tile> _pressureList)
    {
        IARangeTileSorting(_unit, _moveList, _rangeList, _pressureList);
        _unit.GetComponent<UnitMemory>().StoreData(_unit, _moveList, _rangeList);
    }
    private void IARangeTileSorting(Unit _unit, List<Tile> _moveList, List<Tile> _rangeList, List<Tile> _pressureList)
    {
        //Don't get rid of "OnAllied" Tiles
        List<Tile> maxRangeTile = new List<Tile>();
        foreach (var moveTile in _moveList)
        {
            if ((moveTile.pathTile.G == _unit.moveP || _pressureList.Contains(moveTile)) && !maxRangeTile.Contains(moveTile))
            {
                maxRangeTile.Add(moveTile);
            }
        }
        List<Tile> _rangeCloseList = new List<Tile>();
        foreach (var tile in maxRangeTile)
        {
            pathfinder.RangeTile(tile, _moveList, _rangeList, _rangeCloseList, _unit);
        }
    }
    public void RateAction(IAUnitAction action)
    {
        //if(action.)
    }
    #endregion
    #region Player
    public void RangeReceive(Tile tile, Tile.RangeState _state)
    {
        if (tile.rangeState == Tile.RangeState.None || tile.rangeState == Tile.RangeState.OnRange)
        {
            tile.StartRangeReceiving(_state);
            currentDrawTiles.Add(tile);
        }
    }
    public void PlayerMovableTile(Unit _unit)
    {
        List<Tile> moveCloseList = new List<Tile>();
        List<Tile> rangeCloseList = new List<Tile>();
        List<Tile> pressureList = GetPressuredTiles(_unit);
        pathfinder.MoveTile(GetTile(_unit.pos), moveCloseList, rangeCloseList, pressureList, _unit);
        PlayerRangeTile(_unit, moveCloseList, rangeCloseList, pressureList);

        moveCloseList.Clear();
        rangeCloseList.Clear();
        pressureList.Clear();
    }

    public void PlayerRangeTile(Unit _unit, List<Tile> _moveList, List<Tile> _rangeList, List<Tile> _pressureList)
    {
        PlayerRangeTileSorting(_unit, _moveList, _rangeList, _pressureList);
        pathfinder.EnnemyTile(_rangeList, _moveList, _unit);
    }
    private void PlayerRangeTileSorting(Unit _unit, List<Tile> _moveList, List<Tile> _rangeList, List<Tile> _pressureList)
    {
        //Get rid of the "OnAllied" Tiles
        List<Tile> maxRangeTile = new List<Tile>();
        List<Tile> removeFromMoveList = new List<Tile>();

        foreach (var moveTile in _moveList)
        {
            if (moveTile.rangeState == Tile.RangeState.OnAllied)
            {
                removeFromMoveList.Add(moveTile);
            }
            else if ((moveTile.pathTile.G == _unit.moveP  || _pressureList.Contains(moveTile)  || moveTile == _unit.currentTile)  && !maxRangeTile.Contains(moveTile))
            {
                maxRangeTile.Add(moveTile);
            }
        }
        foreach (var removeTile in removeFromMoveList)
        {

            _moveList.Remove(removeTile);

        }
        List<Tile> _rangeCloseList = new List<Tile>();
        foreach (var tile in maxRangeTile)
        {
            pathfinder.RangeTile(tile, _moveList, _rangeList, _rangeCloseList, _unit);
        }
    }
    #endregion
    public void HideMoveableTile()
    {
        if (currentDrawTiles.Count > 0)
        {
            foreach (Tile drawTile in currentDrawTiles)
            {
                drawTile.StopRangeReceiving();
                drawTile.ClearPathTile();
            }
            currentDrawTiles.Clear();
        }
    }
    #region Memory
    private List<Tile> CreateRangeMemory(List<Tile> _moveList, List<Tile> _rangeList)
    {
        List<Tile> rangeMemory = new List<Tile>();
        rangeMemory.AddRange(_moveList);
        rangeMemory.AddRange(_rangeList);
        return rangeMemory;
    }
    public List <Tile> CalculateRangeMemory(Unit _unit)
    {
        List<Tile> moveCloseList = new List<Tile>();
        List<Tile> rangeCloseList = new List<Tile>();
        List<Tile> pressureList = GetPressuredTiles(_unit);
        pathfinder.MoveTile(GetTile(_unit.pos), moveCloseList, rangeCloseList, pressureList, _unit);
        IARangeTileSorting(_unit, moveCloseList, rangeCloseList, pressureList);
        HideMoveableTile();
        return CreateRangeMemory(moveCloseList, rangeCloseList);
    }
    #endregion
    #region Pressure
    public void CheckPressure(Vector2Int _pos)
    {
        foreach (var neighbor in GetNeighbors(_pos))
        {
            if (neighbor.IsUnitOnTile())
            {
                neighbor.GetUnitOnTile().CheckProtectionState();
            }
        }
    }

    public void CheckEngagementPressure(Vector2Int _unitPos1, Unit _engagedUnit)
    {
        foreach (var neighbor in GetNeighbors(_unitPos1))
        {
            if (neighbor.IsUnitOnTile())
            {
                if (neighbor.GetUnitOnTile() != _engagedUnit)
                {
                    neighbor.GetUnitOnTile().CheckProtectionState();
                }
            }
        }

    }

    public List<Tile> GetPressuredTiles(Unit unit)
    {
        List<Tile> pressure = new List<Tile>();
        foreach (var army in GameManager.Instance.armies)
        {
            if (army.allegiance != unit.army.allegiance)
            {
                foreach (var soldier in army.units)
                {
                    pressure = AddNeighbors(soldier.pos, pressure);
                }
            }
        }
        return pressure;
    }
    #endregion

    #endregion
}
