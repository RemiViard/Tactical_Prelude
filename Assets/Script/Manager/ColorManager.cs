using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ColorManager : MonoBehaviour
{

    private static ColorManager _instance;
    public static ColorManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                GameObject colorManager = new GameObject();
                colorManager.name = "ColorManager";
                _instance = (ColorManager)colorManager.AddComponent(typeof(ColorManager));
                _instance.LoadColor();
                colorManager.transform.parent = GameManager.Instance.managersLair;
                return _instance;
            }
        }
    }
    [SerializeField] Material[] male;
    [SerializeField] Material[] female;
    [SerializeField] Material[] staff;
    const int NbMatInGenderByArmy = 3;
    bool ColorLoaded = false;
    private void Awake()
    {
        if(!ColorLoaded)
        {
            LoadColor();
        }
        _instance = this;
        gameObject.transform.parent = GameManager.Instance.managersLair;
    }
    private void LoadColor()
    {
        male = new Material[12];
        female = new Material[12];
        staff = new Material[4];
        foreach (var army in GameManager.Instance.armies)
        {
            if (male[(int)army.color * NbMatInGenderByArmy] == null)
            {
                List<Material> maleMat = new List<Material>();
                List<Material> femMat = new List<Material>();
                List<Material> staffMat = new List<Material>();
                maleMat.AddRange(Resources.LoadAll("MaleMat/" + army.color).Cast<Material>().ToArray());
                femMat.AddRange(Resources.LoadAll("FemaleMat/" + army.color).Cast<Material>().ToArray());
                staffMat.AddRange(Resources.LoadAll("StaffMat/" + army.color).Cast<Material>().ToArray());

                for (int i = 0; i < NbMatInGenderByArmy; i++)
                {
                    male[(int)army.color * NbMatInGenderByArmy + i] = maleMat[i];
                    female[(int)army.color * NbMatInGenderByArmy + i] = femMat[i];
                }
                staff[(int)army.color] = staffMat[0];
            }
        }
        ColorLoaded = true;
    }
    public Material GetColor( Army.ArmyColor _color, bool _isMale, int _torsoIndex)
    {
        return _isMale ? male[(int)_color * NbMatInGenderByArmy + _torsoIndex - 1] : female[(int)_color * NbMatInGenderByArmy + _torsoIndex - 1];
    }
    public Material GetStaffColor(Army.ArmyColor _color)
    {
        return staff[(int)_color];
    }
}
