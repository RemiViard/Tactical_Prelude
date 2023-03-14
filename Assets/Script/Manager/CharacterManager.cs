using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        transform.parent = GameManager.Instance.managersLair;

    }
    [SerializeField] List<GameObject> characterPrefabs = new List<GameObject>();
    public GameObject  CreateCharacter(GameObject _prefabUnit, Army.UnitClass _class)
    {
        GameObject unit = Instantiate(_prefabUnit);
        GameObject character = Instantiate(characterPrefabs[Random.Range(0, characterPrefabs.Count)], unit.transform);
        character.GetComponent<CharacterPart>().EquipCharacter(_class);
        unit.GetComponent<Unit>().torso = character.GetComponent<CharacterPart>().torso;
        return unit;
    }
}
