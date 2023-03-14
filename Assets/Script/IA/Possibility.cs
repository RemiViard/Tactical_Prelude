using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Wound
{
    public int nbOfAttackers;
    public int damage;
    public bool engagement;
}
public struct IAUnitAction
{
    public IAUnitAction(Unit _unit, Vector2Int _actionPos, Unit _target = null)
    {
        unit = _unit;
        target = _target;
        pos = _actionPos;
        value = 0;
        damage = 0;
        danger = 0;
        engageable = false;
        suicidal = false;
        kill = false;
    }
    Unit unit;
    Unit target;
    Vector2Int pos;
    int value;
    int damage;
    int danger;
    bool engageable;
    bool suicidal;
    bool kill;
    void Rate()
    {
        if (target != null)
        {
            damage = unit.CalculateDamage(target);
            value += damage;
            if (damage >= target.Hp)
            {
                kill = true;
                value += 10;
            }
            

        }
    }
}
public class Possibility
{
    public Possibility()
    {
        actions = new List<IAUnitAction>();
        posList = new List<Vector2Int>();
        value = 0;
    }
    public Possibility(Possibility _possibility)
    {
        actions = new List<IAUnitAction>(_possibility.actions);
        posList = new List<Vector2Int>(_possibility.posList);
    }
    public List<IAUnitAction> actions;
    public List<Vector2Int> posList;
    int value;
    public void Rate()
    {
        foreach (var action in actions)
        {

        }
    }
    
}
