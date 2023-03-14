using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Army : MonoBehaviour
{
    public enum UnitClass
    {
        Swordman,
        Door,
        Spearman,
        Wizard,
    }
    public enum ArmyColor
    {
        Blue,
        Green,
        Red,
        Yellow,
    }
    [SerializeField] List<GameObject> unitPrefabs;
    public List<Unit> units = new List<Unit>();
    public List<Unit> rangeDrawedUnits = new List<Unit>();
    public ArmyColor color;
    public bool isHuman = true;
    public int allegiance;
    [SerializeField] Vector2Int squadPos;
    [SerializeField] CharacterManager characterManager;
    [Range(0, 5)] [SerializeField] int squadNb;
    [ExecuteInEditMode]
    private void Awake()
    {
        GameManager.Instance.armies.Add(this);
        GameManager.Instance.armies = GameManager.Instance.armies.OrderBy(o => o.color).ToList();
    }
    private void Start()
    {
        //for (int i = 0; i < squadNb; i++)
        //{
        //    SpawnUnit(UnitClass.Swordman, squadPos);
        //}
        //foreach (var unit in army)
        //{
        //    unit.CheckProtectionState();
        //}
    }
    [ExecuteInEditMode]
    private void Update()
    {
        squadPos = Grid.Instance.RangePosInGrid(squadPos);
    }

    public void SpawnUnit(UnitClass _class, Vector2Int _pos)
    {
        Vector2Int finalPos = Grid.Instance.GetAvailableTile(_pos);
        if (finalPos.x >= 0)
        {
            Unit minion = characterManager.CreateCharacter(unitPrefabs[(int)_class], _class).GetComponent<Unit>();
            minion.Spawn(finalPos, this);
            units.Add(minion);
            minion.GetComponent<UnitMemory>().StoreRangeMemory();
            Tile tile = Grid.Instance.GetTile(finalPos);
            //Debug.Log(color);
            GameManager.Instance.CheckMemoryRange(this, tile);
        }
        
    }
    public bool ContainUnit(Unit _unit)
    {
        return units.Contains(_unit) ? true : false;
    }
    public void CheckEndTurn()
    {
        if (units.TrueForAll(HasActed))
        {
            TurnManager.Instance.StartNewTurn();
        }
    }
    private bool HasActed(Unit unit)
    {
        return unit.hasActed;
    }
    public int GetArmyIndex()
    {
        for (int i = 0; i < GameManager.Instance.armies.Count; i++)
        {
            if (this == GameManager.Instance.armies[i])
            {
                return i;
            }
        }
        return -1;
    }
    public void StoreMemory()
    {
        foreach (var unit in units)
        {
            unit.gameObject.GetComponent<UnitMemory>().StoreRangeMemory();
        }
    }
    public void CheckMemory(Tile _tile)
    {
        foreach (var unit in units)
        {
            unit.gameObject.GetComponent<UnitMemory>().CheckRangeMemory(_tile);
        }
    }
    //Pas pensé pour le + de 1V1
    public void ReDrawMemory()
    {
        foreach(var unit in rangeDrawedUnits)
        {
            unit.memory.DrawRangeMemory();
        }
    }
    public void Obliterate()
    {
        foreach (var unit in units)
        {
            Destroy(unit.gameObject);
        }
        units.Clear();
    }
}
