using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
     public override void ChangeColor(Army.ArmyColor _color) 
    {
        base.ChangeColor(_color);
        GetComponentInChildren<CharacterPart>().ChangeStaffColor(_color);
    }
}
