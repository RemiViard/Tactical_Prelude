using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    public List<Tile> FindPathAStar(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closeList = new List<Tile>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(salut => salut.pathTile.F).First();
            openList.Remove(currentTile);

            List<Tile> successors = Grid.Instance.GetNeighbors(currentTile.pos);
            foreach (var successor in successors)
            {
                if (!closeList.Contains(successor) || !(openList.Contains(successor) && successor.pathTile.G <= currentTile.pathTile.G + 1))
                {
                    if (successor == end)
                    {
                        return ReconstitutePath(end);
                    }
                    successor.pathTile.parent = currentTile;
                    successor.pathTile.G = currentTile.pathTile.G + 1;
                    successor.pathTile.H = (end.pos.x - successor.pos.x) + (end.pos.y - successor.pos.y);
                    successor.pathTile.parent = currentTile;
                    openList.Add(successor);
                }
            }
            closeList.Add(currentTile);
        }
        return null;
    }

    private List<Tile> ReconstitutePath(Tile end)
    {
        return new List<Tile>();
    }

    private void ReplaceOrAddInList(Tile tile, List<Tile> _moveCloseList)
    {
        if (!_moveCloseList.Contains(tile))
        {
            _moveCloseList.Add(tile);
        }
    }
    public void MoveTile(Tile _tile, List<Tile> _moveCloseList, List<Tile> _rangeCloseList, List<Tile> _pressureTile, Unit _unit, int _compteur = 0)
    {
        if (_compteur == 0)
        {
            _moveCloseList.Add(_tile);
        }
        _compteur++;
        if (_tile.IsWalkable())
        {
            if (_compteur <= _unit.moveP)
            {
                List<Tile> neighbors = Grid.Instance.GetNeighbors(_tile.pos);
                foreach (var neighbor in neighbors)
                {
                    if (!_moveCloseList.Contains(neighbor) || (_moveCloseList.Contains(neighbor) && neighbor.pathTile.G > _compteur))
                    {
                        if (neighbor.IsUnitOnTile())
                        {
                            if (neighbor.GetUnitOnTile().army.allegiance == _unit.army.allegiance)
                            {
                                neighbor.pathTile.parent = _tile;
                                neighbor.pathTile.G = _compteur;
                                Grid.Instance.RangeReceive(neighbor, Tile.RangeState.OnAllied);
                                ReplaceOrAddInList(neighbor, _moveCloseList);
                                MoveTile(neighbor, _moveCloseList, _rangeCloseList, _pressureTile, _unit, _compteur);
                            }
                            else
                            {
                                _rangeCloseList.Add(neighbor);
                                ReplaceOrAddInList(neighbor, _moveCloseList);
                            }
                        }
                        else
                        {
                            neighbor.pathTile.parent = _tile;
                            neighbor.pathTile.G = _compteur;
                            Grid.Instance.RangeReceive(neighbor, Tile.RangeState.OnMove);
                            ReplaceOrAddInList(neighbor, _moveCloseList);
                            if (_pressureTile.Contains(neighbor))
                            {
                                neighbor.pathTile.G = _unit.moveP;
                                MoveTile(neighbor, _moveCloseList, _rangeCloseList, _pressureTile, _unit, _unit.moveP);
                            }
                            else
                            {
                                MoveTile(neighbor, _moveCloseList, _rangeCloseList, _pressureTile, _unit, _compteur);
                            }
                        }
                    }
                }
            }
        }
    }
    public void RangeTile(Tile 
        _tile, 
        List<Tile> _moveList, 
        List<Tile> _rangeList, 
        List<Tile> _rangeCloseList, 
        Unit _unit, 
        int count = 0)
    {
        count++;
        if (count <= _unit.stats.range.max)
        {
            foreach (var neighbor in Grid.Instance.GetNeighbors(_tile.pos))
            {
                if (!_moveList.Contains(neighbor) 
                    && (!_rangeCloseList.Contains(neighbor) 
                    || (_rangeCloseList.Contains(neighbor) 
                    && neighbor.pathTile.G > count)))
                {

                    if (_unit.stats.range.LongRange 
                        && Grid.Instance.Distance(neighbor.pos, _unit.pos) == 1 
                        && _unit.moveP == 0)
                    {
                        neighbor.pathTile.G = count;
                        ReplaceOrAddInList(neighbor, _rangeCloseList);
                        RangeTile(neighbor, _moveList, _rangeList, _rangeCloseList, _unit, count);
                    }
                    else
                    {
                        Grid.Instance.RangeReceive(neighbor, Tile.RangeState.OnRange);
                        neighbor.pathTile.G = count;
                        ReplaceOrAddInList(neighbor, _rangeList);
                        ReplaceOrAddInList(neighbor, _rangeCloseList);
                        RangeTile(neighbor, _moveList, _rangeList, _rangeCloseList, _unit, count);
                    }
                }
            }
        }
    }
    public void EnnemyTile(List<Tile> _rangeList, List<Tile> moveList, Unit _unit)
    {
        foreach (var rangeTile in _rangeList)
        {
            if (rangeTile.IsUnitOnTile())
            {
                if (rangeTile.GetUnitOnTile().army.allegiance == _unit.army.allegiance)
                {
                    Grid.Instance.RangeReceive(rangeTile, Tile.RangeState.OnAllied);
                }
                else
                {
                    Grid.Instance.RangeReceive(rangeTile, Tile.RangeState.OnEnnemy);
                    Tile bestTile = null;
                    int distanceBT = _unit.stats.range.max;
                    foreach (var moveTile in moveList)
                    {
                        int distance = Grid.Instance.Distance(moveTile.pos, rangeTile.pos);
                        if (_unit.stats.range.IsInRange(distance))
                        {
                            if (bestTile == null)
                            {
                                bestTile = moveTile;
                                distanceBT = distance;
                            }
                            else
                            {
                                if (moveTile.pathTile.G < bestTile.pathTile.G)
                                {
                                    bestTile = moveTile;
                                    distanceBT = distance;
                                }
                                else if (moveTile.pathTile.G == bestTile.pathTile.G)
                                {
                                    if (distance > distanceBT)
                                    {
                                        bestTile = moveTile;
                                        distanceBT = distance;
                                    }
                                }
                            }
                        }
                    }
                    rangeTile.pathTile.parent = bestTile;
                }
            }
        }
    }

    public List<Tile> ReconstituateMovePath(Tile _destination)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = _destination;
        while (currentTile.pathTile.parent != null)
        {
            path.Add(currentTile);
            currentTile = currentTile.pathTile.parent;
        }
        path.Reverse();
        return path;
    }

}
