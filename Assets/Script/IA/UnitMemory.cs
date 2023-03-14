using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMemory : MonoBehaviour
{
    public struct Target
    {
        public Target(Unit _unit, List<TileData> _attackPossibilities)
        {
            unit = _unit;
            attackPossibilities = _attackPossibilities;
        }
        public Unit unit;
        public List<TileData> attackPossibilities;
    }
    //Army data
    public List<Target> potentialTargets = new List<Target>();
    public List<TileData> potentialMoves;
    public List<TileData> potentialAttacks;
    //Other army data
    bool isShowed = false;
    List<Tile> rangeMemory = new List<Tile>();
    //Global
    List<Tile> currentDrawTiles = new List<Tile>();


    public void StoreData(Unit _unit, List<Tile> moveTile, List<Tile> rangeTile)
    {
        potentialMoves = new List<TileData>();
        foreach (var tile in moveTile)
        {
            TileData newTileData = new TileData(tile);
            potentialMoves.Add(newTileData);
        }
        StoreTarget(_unit, moveTile, rangeTile);
    }
    void StoreTarget(Unit _unit, List<Tile> moveTiles, List<Tile> rangeTiles)
    {
        foreach (var rangeTile in rangeTiles)
        {
            if (rangeTile.IsUnitOnTile())
            {
                Unit testedUnit = rangeTile.GetUnitOnTile();
                if (testedUnit.army.allegiance != _unit.army.allegiance)
                {
                    List<TileData> attackPossibilities = new List<TileData>();
                    foreach (var moveTile in moveTiles)
                    {
                        if (_unit.stats.range.IsInRange(Grid.Instance.Distance(moveTile.pos, rangeTile.pos)))
                        {
                            TileData tileData = new TileData(moveTile);
                            attackPossibilities.Add(tileData);
                        }
                    }
                    potentialTargets.Add(new Target(testedUnit, attackPossibilities));
                }
            }
        }
    }
    public void StoreRangeMemory()
    {
        rangeMemory = Grid.Instance.CalculateRangeMemory(gameObject.GetComponent<Unit>());
    }
    public void CheckRangeMemory(Tile _tile)
    {
        if(rangeMemory.Contains(_tile))
        {
            StoreRangeMemory();
            if(isShowed)
            {
                HideRangeMemory();
                DrawRangeMemory();
            }
            //Debug.Log("modif");
        }
    }
    public void GetBestActions()
    {
        new List<IAUnitAction>();
        foreach (var move in potentialMoves)
        {
            IAUnitAction newAction = new IAUnitAction(GetComponent<Unit>(), move.pos);
            Grid.Instance.RateAction(newAction);
        }
    }
    public void ShowData()
    {
        foreach (var target in potentialTargets)
        {
            Grid.Instance.RangeReceive(Grid.Instance.GetTile(target.unit.pos), Tile.RangeState.OnEnnemy);
            AddDrawTile(Grid.Instance.GetTile(target.unit.pos));
            foreach (var moveTarget in target.attackPossibilities)
            {
                Tile tile = Grid.Instance.GetTile(moveTarget.pos);
                Grid.Instance.RangeReceive(tile, Tile.RangeState.OnAllied);
                AddDrawTile(target.unit.currentTile);
            }
        }
        foreach(var move in potentialMoves)
        {
            Tile tile = Grid.Instance.GetTile(move.pos);
            Grid.Instance.RangeReceive(tile, Tile.RangeState.OnMove);
            AddDrawTile(tile);
        }
        
    }
    public void SelectMemory(Unit unit)
    {
        if(isShowed)
        {
            HideRangeMemory();
            unit.army.rangeDrawedUnits.Remove(unit);
            unit.army.ReDrawMemory();
        }
        else
        {
            DrawRangeMemory();
            unit.army.rangeDrawedUnits.Add(unit);
        }
        isShowed = !isShowed;
        
    }
    public void DrawRangeMemory()
    {
        foreach (var tile in rangeMemory)
        {
            tile.EnRangeShow();
            currentDrawTiles.Add(tile);
        }
    }
    public void HideRangeMemory()
    {
        if (currentDrawTiles.Count > 0)
        {
            foreach (var tile in currentDrawTiles)
            {
                tile.EnRangeHide();
            }
            currentDrawTiles.Clear();
        }
    }
    public void HideData()
    {
        if (currentDrawTiles.Count > 0)
        {
            foreach (Tile drawTile in currentDrawTiles)
            {
                drawTile.StopRangeReceiving();
            }
            currentDrawTiles.Clear();
        }
    }
    void AddDrawTile(Tile _tile)
    {
        if (!currentDrawTiles.Contains(_tile))
        {
            currentDrawTiles.Add(_tile);
        }
    }
    public void ClearData()
    {
        potentialTargets.Clear();
        potentialMoves.Clear();
    }
}