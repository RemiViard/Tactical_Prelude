using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform transfo;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CharacterPart>().EquipCharacter(Army.UnitClass.Door);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
