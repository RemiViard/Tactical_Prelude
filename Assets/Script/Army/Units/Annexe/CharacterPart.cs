using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPart : MonoBehaviour
{
    [SerializeField] bool isMale;
    public SkinnedMeshRenderer torso;
    [SerializeField] GameObject swordPrefab;
    [SerializeField] GameObject axePrefab;
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] GameObject spearPrefab;
    [SerializeField] GameObject staffPrefab;
    [SerializeField] GameObject wandPrefab;
    GameObject staff;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    public void EquipCharacter(Army.UnitClass _class)
    {
        switch (_class)
        {
            case Army.UnitClass.Swordman:
                Instantiate(swordPrefab, rightHand);
                break;
            case Army.UnitClass.Door:
                Instantiate(axePrefab, rightHand);
                GameObject shield = Instantiate(shieldPrefab, leftHand);
                if(!isMale)
                {
                    shield.transform.Rotate(new Vector3(0, 0, -12f));
                }
                break;
            case Army.UnitClass.Spearman:
                Instantiate(spearPrefab, rightHand);
                break;
            case Army.UnitClass.Wizard:
                staff = Instantiate(staffPrefab, rightHand);
                
                break;
            default:
                break;
        }
    }
    public void ChangeStaffColor(Army.ArmyColor _color)
    {
        staff.GetComponent<MeshRenderer>().material = ColorManager.Instance.GetStaffColor(_color);
    }
    
}
