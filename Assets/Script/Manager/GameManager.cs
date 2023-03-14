using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                GameObject gameManager = new GameObject();
                gameManager.name = "GameManager";
                _instance = (GameManager)gameManager.AddComponent(typeof(GameManager));
                _instance.Init();
                return _instance;
            }
        }
    }
    public List<Army> armies = new List<Army>();
    [HideInInspector] public Transform managersLair;
    public Canvas mainCanvas;
    private void Awake()
    {
        _instance = this;
        Init();
    }
    private void FindLair()
    {
        if (managersLair == null)
        { 
        GameObject lair = new GameObject();
        lair.name = "Manager's Lair";
        managersLair = lair.transform;
        gameObject.transform.parent = lair.transform;
        }
    }
    #region
    public void CheckMemoryRange(Army fromArmy, Tile _tile)
    {
        foreach (var army in armies)
        {
            if(army != fromArmy)
            {
                army.CheckMemory(_tile);
            }
        }
    }
    public int GetWoundsByPos(Army fromArmy, Tile _tile)
    {
        int WoundDamage = 0;
        foreach (var army in armies)
        {
            if (army.allegiance != fromArmy.allegiance)
            {
                //if ()
            }
        }
        return WoundDamage;
        
    }
        #endregion
    private void Init()
    {
        FindLair();
        Canvas[] allCanvas = FindObjectsOfType<Canvas>();
        foreach (var canvas in allCanvas)
        {
            if (canvas.gameObject.name == "MainCanvas")
            {
                mainCanvas = canvas;
            }
        }
    }
}
