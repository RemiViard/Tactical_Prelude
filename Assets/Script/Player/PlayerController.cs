using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public Army currentArmyTurn;
    OverlayTile overlayTile;
    Tile selectedTile;
    Tile enlightTile;
    Tile currentTile;
    Unit selectedUnit;
    Camera cam;
    [SerializeField] Transform camPivot;
    Vector3 inputMovement;
    Vector3 movement;
    Vector2 LastMousePos;
    float zoomLevel = 0;
    [SerializeField][Range(5, 10)] int camSensitivity;

    void Start()
    {
        cam = Camera.main;
        overlayTile = GetComponentInChildren<OverlayTile>();
        TurnManager.Instance.playerController = this;
    }
    #region Inputs
    private void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (selectedUnit != null)
            {
                selectedUnit.Die();
            }
        }
    }
    void GridInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Tile"))
            {
                currentTile = hit.collider.gameObject.GetComponent<Tile>();
                if (Input.GetMouseButtonDown(0))
                {
                    if (selectedTile != currentTile)
                    {
                        if (selectedUnit != null && currentArmyTurn.isHuman)
                        {
                            if (currentTile.isMoveReceiving)
                            {
                                selectedUnit.Move(currentTile.pos, currentTile.pathTile.G);
                                selectedUnit.Unselect();
                            }
                            else if (currentTile.isAttackReceiving)
                            {
                                selectedUnit.Attack(currentTile.GetUnitOnTile(), currentTile);
                            }
                        }
                        SelectTile();
                    }
                }
                CheckEnlightTile();
            }
        }
        else
        {
            overlayTile.Hide();
            if (Input.GetMouseButtonDown(0))
            {
                UnselectTile();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            UnselectTile();
        }
    }

    void CheckEnlightTile()
    {
        if (currentTile != enlightTile)
        {
            enlightTile = currentTile;
            if (enlightTile != selectedTile)
            {
                overlayTile.transform.position = enlightTile.transform.position;
                overlayTile.Show();
            }
            else
            {
                overlayTile.Hide();
            }
            switch (enlightTile.rangeState)
            {
                case Tile.RangeState.OnMove:
                    overlayTile.ChangeSprite(OverlayTile.ESpriteName.OnMoving);
                    break;
                case Tile.RangeState.OnRange:
                    overlayTile.ChangeSprite(OverlayTile.ESpriteName.OnRange);
                    break;
                case Tile.RangeState.OnEnnemy:
                    overlayTile.ChangeSprite(OverlayTile.ESpriteName.OnRange);
                    break;
                case Tile.RangeState.OnAllied:
                    overlayTile.ChangeSprite(OverlayTile.ESpriteName.OnAllied);
                    break;
                default:
                    overlayTile.ChangeSprite(OverlayTile.ESpriteName.Regular);
                    break;
            }
        }
    }
    void SelectTile()
    {
        if (selectedTile != null)
        {
            UnselectTile();
            if (selectedUnit != null)
            {
                selectedUnit.Unselect();
            }
        }
        overlayTile.Hide();
        selectedTile = currentTile;
        selectedTile.Select();
        if (selectedTile.IsUnitOnTile())
        {
            selectedUnit = selectedTile.GetUnitOnTile();
            selectedUnit.Select(currentArmyTurn);
        }
    }
    public void UnselectTile()
    {
        if (selectedTile != null)
        {
            selectedTile.Darken();
            selectedTile = null;
            if (selectedUnit != null)
            {
                selectedUnit.Unselect();
                selectedUnit = null;
            }
        }
    }
    void UnselectAll()
    {
        selectedTile = null;
        selectedUnit = null;
        enlightTile = null;
    }
    #endregion
    #region CameraInputs
    void CameraInputs()
    {
        if (Input.anyKey)
        {
            inputMovement.x = Input.GetAxis("Horizontal");
            inputMovement.z = Input.GetAxis("Vertical");


            movement = GetCamFlatForward() * inputMovement.z;
            movement += GetCamFlatRight() * inputMovement.x;
            camPivot.position += Vector3.Normalize(movement) * Time.deltaTime * (float)camSensitivity;
        }
        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
            {
                LastMousePos = Input.mousePosition;
            }

            camPivot.Rotate(Vector3.up, (LastMousePos.x - Input.mousePosition.x) * Time.deltaTime * (float)camSensitivity * 10f, Space.World);

            //Clamp Swivel Rotation
            GameObject nextRotation = new GameObject();
            nextRotation.transform.rotation = camPivot.GetChild(0).transform.rotation;
            nextRotation.transform.Rotate(nextRotation.transform.right, (LastMousePos.y - Input.mousePosition.y) * Time.deltaTime * (float)camSensitivity * 10f, Space.World);
            if (nextRotation.transform.rotation.eulerAngles.x >= 10 && nextRotation.transform.rotation.eulerAngles.x <= 80)
            {
                camPivot.GetChild(0).rotation = nextRotation.transform.rotation;
            }
            Destroy(nextRotation);
            LastMousePos = Input.mousePosition;
        }
        if (Input.mouseScrollDelta.magnitude > 0)
        {
            zoomLevel += Input.mouseScrollDelta.y * Time.deltaTime * camSensitivity * 30f;
            zoomLevel = Mathf.Clamp(zoomLevel, -10, 4);
            cam.transform.localPosition = new Vector3(0, 0, zoomLevel);

        }
        //if (Input.GetMouseButton(0))
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        LastMousePos = Input.mousePosition;
        //    }
        //    movement += GetCamFlatForward() * (LastMousePos.y - Input.mousePosition.y);
        //    movement += GetCamFlatRight() * (LastMousePos.x - Input.mousePosition.x);
        //    camPos.position += movement * Time.deltaTime * (float)camSensitivity * 0.01f;
        //}
    }
    // Update is called once per frame
    Vector3 GetCamFlatForward()
    {
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        return Vector3.Normalize(camForward);
    }
    Vector3 GetCamFlatRight()
    {
        Vector3 camForward = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);
        return Vector3.Normalize(camForward);
    }
    #endregion


    void Update()
    {
        GridInput();
        CameraInputs();
        DebugInput();
    }


    private void OnDrawGizmos()
    {
        if (enlightTile != null && selectedUnit != null)
        {
            if (selectedUnit.army.isHuman)
            {
                if (enlightTile.isMoveReceiving)
                {
                    Tile tile = enlightTile;
                    while (tile.pathTile.parent != null)
                    {
                        Gizmos.DrawLine(tile.transform.position, tile.pathTile.parent.transform.position);
                        tile = tile.pathTile.parent;
                    }
                }
                else if (enlightTile.isAttackReceiving)
                {
                    Tile tile = enlightTile.pathTile.parent;
                    while (tile.pathTile.parent != null)
                    {
                        Gizmos.DrawLine(tile.transform.position, tile.pathTile.parent.transform.position);
                        tile = tile.pathTile.parent;
                    }
                }
                else
                {
                    foreach (var item in Grid.Instance.currentDrawTiles)
                    {
                        if (item.isMoveReceiving)
                        {
                            Gizmos.DrawLine(item.transform.position, item.pathTile.parent.transform.position);
                        }

                    }
                }
            }

        }
    }
    public void ChangeArmyTurn(Army newArmy)
    {
        currentArmyTurn = newArmy;
    }
    public Vector2Int GetSelectedTilePos()
    {
        if (selectedTile != null)
        {
            return selectedTile.pos;
        }
        else
        {
            return new Vector2Int(0, 0);
        }
    }
}
