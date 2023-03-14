using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Army))]
public class ArmyEditor : Editor
{
    Vector2Int pos = new Vector2Int(5,5);
    Army.UnitClass index = Army.UnitClass.Swordman;
    bool selectedPos = false;
    public override void OnInspectorGUI()
    {
        Army army = (Army)target;
        GUILayout.FlexibleSpace();
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("In Game");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            pos = EditorGUILayout.Vector2IntField("Position", pos);
            pos = Grid.Instance.RangePosInGrid(pos);
            index = (Army.UnitClass)EditorGUILayout.EnumPopup("Unit Name", index);
            selectedPos = EditorGUILayout.Toggle("SelectedPos", selectedPos);
            if (GUILayout.Button("Spawn Unit"))
            {
                if (selectedPos)
                {
                    army.SpawnUnit(index, TurnManager.Instance.playerController.GetSelectedTilePos());
                }
                else
                {
                    army.SpawnUnit(index, pos);
                }
            }
        }
        
    }
}
