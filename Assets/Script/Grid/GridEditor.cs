using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    string saveName;
    public override void OnInspectorGUI()
    {
        Grid grid = (Grid)target;
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("LoadMap"))
            {
                grid.LoadMap();
            }
            saveName = EditorGUILayout.TextField(saveName);
            if (GUILayout.Button("SaveMap") && saveName != "")
            {
                grid.SaveMap(saveName);
            }
        }
    }
}
