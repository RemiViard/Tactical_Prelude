using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour
{

    Army currentArmy;
    [SerializeField]  float dangerValue;
    [SerializeField]  float attackValue;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.Instance.IaController = this;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void PlayTurn(Army army)
    {
        currentArmy = army;
        CalculateMoves();
        CalculateFuture();
    }
    void CalculateMoves()
    {
        foreach (var unit in currentArmy.units)
        {
            if (!unit.TryGetComponent(out UnitMemory unitBrain))
            {
                unit.gameObject.AddComponent<UnitMemory>();
            }
            Grid.Instance.IAMovableTile(unit);
        }
    }
    public void EndTurn()
    {
        foreach (var unit in currentArmy.units)
        {
            unit.GetComponent<UnitMemory>().ClearData();
        }
        currentArmy = null;
    }
    public void GetAllPossibilities(List<Possibility> possibilities, Possibility currentPossibility = null, int deepness = 0)
    {
        if (deepness < currentArmy.units.Count - 1)
        {
            Unit unit = currentArmy.units[deepness];
            foreach (var move in unit.GetComponent<UnitMemory>().potentialMoves)
            {
                if (deepness == 0)
                {
                    currentPossibility = new Possibility();
                    currentPossibility.actions.Add(new IAUnitAction(unit, move.pos));
                    currentPossibility.posList.Add(move.pos);
                    GetAllPossibilities(possibilities, currentPossibility, deepness + 1);
                }
                else
                {
                    if (!currentPossibility.posList.Contains(move.pos))
                    {
                        Possibility copyPoss = new Possibility(currentPossibility);
                        copyPoss.actions.Add(new IAUnitAction(unit, move.pos));
                        copyPoss.posList.Add(move.pos);
                        GetAllPossibilities(possibilities, copyPoss, deepness + 1);
                    }
                }
            }
        }
        else
        {
            Unit unit = currentArmy.units[deepness];
            foreach (var move in unit.GetComponent<UnitMemory>().potentialMoves)
            {
                if (!currentPossibility.posList.Contains(move.pos))
                {
                    Possibility copyPoss = new Possibility(currentPossibility);
                    copyPoss.actions.Add(new IAUnitAction(unit, move.pos));
                    possibilities.Add(copyPoss);
                    Debug.Log("ActionsCount : " + copyPoss.actions.Count);
                }
            }
        }
    }
    public void GetBestPossibilities(List<Possibility> _possibilities)
    {
        foreach (var unit in currentArmy.units)
        {
            unit.memory.GetBestActions();
        }
    }
    public void CalculateFuture()
    {
        List<Possibility> possibilities = new List<Possibility>();
        //GetAllPossibilities(possibilities);
        GetBestPossibilities(possibilities);
        Debug.Log("Possibility : " + possibilities.Count);
        foreach (var possibility in possibilities)
        {

        }
    }
}
