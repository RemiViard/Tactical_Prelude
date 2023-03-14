using System;
using System.Collections.Generic;
using UnityEngine;
public struct PathTile
{
    [HideInInspector] public int G;
    [HideInInspector] public int H;
    [HideInInspector] public Tile parent;
    public int F { get { return G + H; } }
}
public class Tile : MonoBehaviour
{
    public enum SpritesName
    {
        selectedSpr,
        moveSpr,
        rangeSpr,
        alliedSpr,
    }
    public enum RangeState
    {
        None,
        OnMove,
        OnRange,
        OnAllied,
        OnEnnemy,
    }
    [SerializeField] List<Sprite> sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer EnRngSprRenderer;
    Unit currentUnit = null;
     public Vector2Int pos;
    [HideInInspector] public bool isMoveReceiving = false;
    [HideInInspector] public bool isAttackReceiving = false;
    public RangeState rangeState = RangeState.None;
    public PathTile pathTile;
    // Start is called before the first frame update
    void Start()
    {
        pathTile.G = 0;
    }
    // Update is called once per frame
    void Update()
    {
    }
    #region UnitManagementTool
    public void AddUnitOnTile(Unit _unit)
    {
        currentUnit = _unit;
    }
    public void RemoveUnitOnTile()
    {
        currentUnit = null;
    }
    public bool IsUnitOnTile()
    {
        if (currentUnit != null)
        {
            return true;
        }
        else
        {
            
            return false;
        }
    }
    public Unit GetUnitOnTile()
    {
        return currentUnit;
    }
    public bool IsWalkable()
    {     
        return true;
    }
    #endregion
    #region Visual
    public void Select()
    {
        spriteRenderer.sprite = sprites[(int)SpritesName.selectedSpr];
        spriteRenderer.enabled = true;
    }
    public void Darken()
    {
        if (rangeState == RangeState.None)
        {
            spriteRenderer.enabled = false;
        }
    }
    public void StartRangeReceiving(RangeState state)
    {
        switch (state)
        {
            case RangeState.OnMove:
                spriteRenderer.sprite = sprites[(int)SpritesName.moveSpr];
                isMoveReceiving = true;
                break;
            case RangeState.OnRange:
                spriteRenderer.sprite = sprites[(int)SpritesName.rangeSpr];
                break;
            case RangeState.OnAllied:
                spriteRenderer.sprite = sprites[(int)SpritesName.alliedSpr];
                break;
            case RangeState.OnEnnemy:
                spriteRenderer.sprite = sprites[(int)SpritesName.rangeSpr];
                isAttackReceiving = true;
                break;
        }
        spriteRenderer.enabled = true;
        rangeState = state;
    }
    public void StopRangeReceiving()
    {
        switch (rangeState)
        {
            case RangeState.OnMove:
                isMoveReceiving = false;
                break;
            case RangeState.OnEnnemy:
                isAttackReceiving = false;
                break;
        }
        spriteRenderer.enabled = false;
        rangeState = RangeState.None;
    }
    public void EnRangeShow()
    {
        EnRngSprRenderer.enabled = true;
    }
    public void EnRangeHide()
    {
        EnRngSprRenderer.enabled = false;
    }
    #endregion
    public void ClearPathTile()
    {
        pathTile.G = 0;
        pathTile.H = 0;
        pathTile.parent = null;
    }
    public SpritesName GetCurrentlyUsedSprite()
    {
        return (SpritesName)sprites.IndexOf(spriteRenderer.sprite);
    }
    
}
