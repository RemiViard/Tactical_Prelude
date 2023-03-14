using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UnitMemory))]
public class Unit : MonoBehaviour, IKillable
{
    public enum ProtectionState
    {
        None,
        Surrounded,
        Covered,
    }
    [Serializable]
    public struct Range
    {
        public bool LongRange;
        public int max;
        [HideInInspector]
        public int min { get { return LongRange ? 2 : 1; } }
        public bool IsInRange(int _value)
        {
            if (_value <= max && _value >= min)
            {
                return true;
            }
            return false;
        }
    }
    [Serializable]
    public struct Stats
    {
        public int MaxHp;
        [Range(1, 4)] public int Movement;
        public Range range;
        public int Strenght;
        public int Stamina;
        public int Armor;
        public int Technique;
    }
    public Stats stats;


    [Header("Debug")]
    public Vector2Int pos;
    public Tile currentTile;
    public int Hp;
    public int moveP;
    Animator animator;
    public UnitMemory memory;
    public bool hasActed { get; private set; } = false;
    public bool engaged = false;
    public ProtectionState protectionState = ProtectionState.None;
    Unit target;
    Unit engagedUnit = null;
    [Header("Technical")]
    [SerializeField] float movementSpeed;
    public SkinnedMeshRenderer torso = new SkinnedMeshRenderer();
    [SerializeField] bool isMale;
    [SerializeField] int torsoIndex;
    [SerializeField] GameObject CharacterUI;
    HealthBar healthBar;
    ProtectionStateUI PSUI;
    [HideInInspector] public Army army;


    //Events
    [HideInInspector] public UnityEvent endMove = new UnityEvent();
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
    }
    public void ResetTurn()
    {
        moveP = stats.Movement;
        hasActed = false;
    }

    public void Spawn(Vector2Int _pos, Army _army)
    {
        //Instance variable
        healthBar = CharacterUI.GetComponent<HealthBar>();
        PSUI = CharacterUI.GetComponent<ProtectionStateUI>();
        GetComponentInChildren<UnitEvent>().LoadEvent(this);
        memory = GetComponent<UnitMemory>();
        InstanceColor();
        //Instance army
        army = _army;
        ChangeColor(army.color);
        //Instance pos
        pos = _pos;
        Grid.Instance.SpawnOnGrid(this, _pos);
        transform.position = Grid.Instance.GetTilePos(pos);
        transform.parent = army.transform;
        //Instance Stat
        healthBar.InitMaxHp(stats.MaxHp);
        moveP = stats.Movement;
        Hp = stats.MaxHp;
        CheckProtectionState();
    }
    void Update()
    {

    }
    IEnumerator MoveToDestination(Vector2Int _destination, int movePCost)
    {
        animator.SetFloat("Speed", 1);
        List<Tile> path = Grid.Instance.pathfinder.ReconstituateMovePath(Grid.Instance.GetTile(_destination));
        foreach (var tile in path)
        {
            float timer = 0;
            Vector3 firstPos = transform.position;
            transform.LookAt(tile.transform.position, transform.up);
            
            while (timer <= 1)
            {
                transform.position = Vector3.Lerp(firstPos, tile.transform.position, timer);
                timer += Time.deltaTime * movementSpeed;
                yield return null;
            }
        }
        animator.SetFloat("Speed", 0);
        Grid.Instance.MoveOnGrid(this, _destination);
        Grid.Instance.CheckPressure(pos);
        Grid.Instance.CheckPressure(_destination);
        Debug.Log("Start : " + Grid.Instance.GetTile(pos).IsUnitOnTile());
        Debug.Log("End : " + Grid.Instance.GetTile(_destination).IsUnitOnTile());
        Vector2Int start = new Vector2Int(pos.x, pos.y);
        pos = _destination;
        GameManager.Instance.CheckMemoryRange(army, Grid.Instance.GetTile(start));
        GameManager.Instance.CheckMemoryRange(army, Grid.Instance.GetTile(pos));
        moveP -= movePCost;
        CheckProtectionState();
        endMove.Invoke();
    }
    public void Move(Vector2Int _destination, int _movePCost)
    {
        Grid.Instance.CheckPressure(pos);
        StartCoroutine(MoveToDestination(_destination, _movePCost));

    }
    public void Select(Army currentArmyTurn)
    {
        if(currentArmyTurn == army)
        {
            if (army.isHuman)
            {
                if (engaged)
                {
                    Tile engagedTile = engagedUnit.currentTile;
                    Grid.Instance.RangeReceive(engagedTile, Tile.RangeState.OnEnnemy);
                    engagedTile.pathTile.parent = currentTile;
                }
                else
                {
                    Grid.Instance.PlayerMovableTile(this);
                }
            }
            else
            {
                memory.ShowData();
            }
        }
        else
        {
            //memory.SelectMemory(this);
        }
    }
    public void Unselect()
    {
        if (army.isHuman)
        {
            Grid.Instance.HideMoveableTile();
        }
        else
        {
            GetComponent<UnitMemory>().HideData();
        }
    }
    public void InstanceColor()
    {
        isMale = torso.material.name.StartsWith("Male") ? true : false;
        string[] strings = torso.material.name.Split('_');
        torsoIndex = strings[2][0] - '0';
    }
    virtual public void ChangeColor(Army.ArmyColor _color)
    {
        torso.material = ColorManager.Instance.GetColor(_color, isMale, torsoIndex);
        healthBar.ChangeColor(_color);
    }

    public void Rot()
    {
        Grid.Instance.RemoveFromGrid(this);
        Grid.Instance.HideMoveableTile();
        army.units.Remove(this);
        Destroy(gameObject);
    }
    public void Die()
    {
        animator.SetTrigger("Die");
    }
    public void TakeDamage(int _damage)
    {
        Hp -= _damage;
        if (Hp <= 0)
        {
            if (engaged)
            {
                engagedUnit.Disengage();
                Disengage();
            }
            Die();
        }
        healthBar.ChangeValue(Hp);
    }

    public void HealHp(int _healAmount)
    {
        Hp += _healAmount;
        if (Hp > stats.MaxHp)
        {
            Hp = stats.MaxHp;
        }
        healthBar.ChangeValue(Hp);
    }

    public void Attack(Unit _target, Tile tile)
    {
        target = _target;
        if (tile.pathTile.parent.pos != pos)
        {
            endMove.AddListener(StartStrike);
            Move(tile.pathTile.parent.pos, tile.pathTile.G);
        }
        else
        {
            StartStrike();
        }
    }
    public void StartStrike()
    {
        if (engaged)
        {
            engagedUnit.PauseEngagement();
            PauseEngagement();
        }
        transform.LookAt(target.transform, transform.up);
        animator.SetInteger("RandomAttack", Random.Range(0, 3));
        animator.SetTrigger("Attack");
        endMove.RemoveAllListeners();
    }
    public void Strike()
    {
        target.TakeDamage(CalculateDamage(target));
        if (target.Hp > 0)
        {
            if (Vector2Int.Distance(target.pos, pos) == 1)
            {
                if (target.engaged == false)
                {
                    Engage(target);
                    target.Engage(this);
                }
                else if(target.engagedUnit == this)
                {
                    ResumeEngagement();
                    target.ResumeEngagement();
                }
            }
        }
        target = null;
        EndTurn();
    }

    public int CalculateDamage(Unit _target)
    {
        float damage = stats.Strenght - _target.stats.Armor;
        float multiplicator = 1f;
        if (_target.engaged)
        {
            if (_target.engagedUnit != this)
            {
                if (-(_target.pos - _target.engagedUnit.pos) == (Vector2)(_target.pos - pos) / Vector2Int.Distance(_target.pos, pos))
                {
                    //Debug.Log("BACKSTAB");
                    multiplicator += 0.5f;
                }
            }
            switch (target.protectionState)
            {
                case ProtectionState.Surrounded:
                    //Debug.Log("Surrounded");
                    multiplicator += 0.3f;
                    break;
                case ProtectionState.Covered:
                    //Debug.Log("Covered");
                    multiplicator -= 0.3f;
                    break;
            }
        }
        return (int)(damage * multiplicator);
    }

    public void CheckProtectionState()
    {
        if (engaged)
        {
            int count = 0;
            foreach (var neighbor in Grid.Instance.GetNeighbors(pos))
            {
                if (neighbor.IsUnitOnTile())
                {
                    Unit unit = neighbor.GetUnitOnTile();
                    if (!unit.engaged && unit != engagedUnit)
                    {
                        if (unit.army.allegiance == army.allegiance)
                        {
                            count++;
                        }
                        else
                        {
                            count--;
                        }
                    }
                }
            }
            if (count > 0)
            {
                protectionState = ProtectionState.Covered;
            }
            else if (count < 0)
            {
                protectionState = ProtectionState.Surrounded;
            }
            else
            {
                protectionState = ProtectionState.None;
            }
        }
        else
        {
            protectionState = ProtectionState.None;
        }
        PSUI.ChangeSprite(protectionState);
    }
    void Engage(Unit _engagedUnit)
    {
        engaged = true;
        animator.SetBool("Engage", true);
        engagedUnit = _engagedUnit;
        transform.LookAt(engagedUnit.transform);
        CheckProtectionState();
        Grid.Instance.CheckEngagementPressure(pos, _engagedUnit);
    }
    void PauseEngagement()
    {
        animator.SetBool("Engage", false);
    }
    void ResumeEngagement()
    {
        animator.SetBool("Engage", true);
    }
    void Disengage()
    {
        engaged = false;
        animator.SetBool("Engage", false);
        CheckProtectionState();
        Grid.Instance.CheckEngagementPressure(pos, engagedUnit);
        engagedUnit = null;
    }
    void EndTurn()
    {
        hasActed = true;
        army.CheckEndTurn();
    }
}
