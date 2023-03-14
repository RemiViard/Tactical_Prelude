using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    private static TurnManager _instance;
    public static TurnManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                GameObject turnManager = new GameObject();
                turnManager.name = "TurnManager";
                _instance = (TurnManager)turnManager.AddComponent(typeof(TurnManager));
                return _instance;
            }
        }
    }
    int turn = 1;
    int armyTurn = 0;
    GameObject turnSpritePrefab;
    [SerializeField] Image turnSprite;
    public PlayerController playerController;
    public IAController IaController;
    [SerializeField] List<Sprite> turnSpritesList = new List<Sprite>();
    private void Start()
    {
        turnSpritePrefab = (GameObject)Resources.Load("Prefab/TurnSprite");
        object[] loadSprites = Resources.LoadAll("Sprite/Turn/", typeof(Sprite));
        foreach (var sprite in loadSprites)
        {
            turnSpritesList.Add((Sprite)sprite);
        }
        turnSprite = Instantiate(turnSpritePrefab, GameManager.Instance.mainCanvas.transform).GetComponent<Image>();
        StartNewTurn(true);
    }
    public void StartNewTurn(bool isFirstTurn = false)
    {
        if (!isFirstTurn)
        {
            armyTurn++;
            if (armyTurn >= GameManager.Instance.armies.Count)
            {
                armyTurn = 0;
                turn++;
            }
        }
        ChangeTurnSprite();
        foreach (var unit in GameManager.Instance.armies[armyTurn].units)
        {
            unit.ResetTurn();
        }
        SetController();
    }
    private void ChangeTurnSprite()
    {
        turnSprite.sprite = turnSpritesList[(int)GameManager.Instance.armies[armyTurn].color];
    }
    void SetController()
    {
        playerController.ChangeArmyTurn(GameManager.Instance.armies[armyTurn]);
        if (!GameManager.Instance.armies[armyTurn].isHuman)
        {
            IaController.PlayTurn(GameManager.Instance.armies[armyTurn]);
        }
    }
    private void Awake()
    {
        _instance = this;
        gameObject.transform.parent = GameManager.Instance.managersLair;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (GameManager.Instance.armies[armyTurn].isHuman)
            {
                StartNewTurn();
            }
            else
            {
                IaController.EndTurn();
                StartNewTurn();
            }
        }
    }
}