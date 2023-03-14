using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MapDataReader
{
    struct MapUnit
    {
        public Vector2Int pos;
        public Army.UnitClass unitClass;
        public int armyIndex;
        public MapUnit(Vector2Int _pos, Army.UnitClass _unitClass, int _armyindex)
        {
            pos = _pos;
            unitClass = _unitClass;
            armyIndex = _armyindex;
        }
    }
    public void ReadGrid(Grid _grid, TextAsset _data)
    {
        string[] sizeStr = _data.text.Split(new char[] { ',', '{' }, 3);
        Vector2Int size = new Vector2Int(int.Parse(sizeStr[0]), int.Parse(sizeStr[1]));
        string[] firstDataGrid = sizeStr[2].Split(new char[] { ',', '}', '\n' });
        List<string> dataGrid = new List<string>();
        foreach (var str in firstDataGrid)
        {
            string str2 = str;
            str2 = str2.Replace("\n", "");
            str2 = str2.Replace("\t", "");
            str2 = str2.Replace("\r", "");
            str2 = str2.Replace(" ", "");
            if (str2.Length > 0)
            {
                dataGrid.Add(str2);
            }
        }
        List<MapUnit> mapUnits = new List<MapUnit>();
        int count = 0;
        foreach (string value in dataGrid)
        {
            if (value[0] != '0')
            {
                int y = count / size.x;
                int x = count % size.x;
                switch (value[0])
                {
                    case ('S'):
                        mapUnits.Add(new MapUnit(new Vector2Int(x, y), Army.UnitClass.Swordman, int.Parse(value[1].ToString())));
                        break;
                    case ('D'):
                        mapUnits.Add(new MapUnit(new Vector2Int(x, y), Army.UnitClass.Door, int.Parse(value[1].ToString())));
                        break;
                    case ('L'):
                        mapUnits.Add(new MapUnit(new Vector2Int(x, y), Army.UnitClass.Spearman, int.Parse(value[1].ToString())));
                        break;
                    case ('W'):
                        mapUnits.Add(new MapUnit(new Vector2Int(x, y), Army.UnitClass.Wizard, int.Parse(value[1].ToString())));
                        break;
                    default:
                        Debug.Log("Ununderstandable Unit Name : S, D, L, W " + x + size.x * y);
                        break;
                }
            }
            count++;
        }
        _grid.LoadGrid(size);
        foreach (var unit in mapUnits)
        {
            GameManager.Instance.armies[unit.armyIndex].SpawnUnit(unit.unitClass, unit.pos);
        }
    }
    public void SaveGrid(Grid grid, string name)
    {
        using (StreamWriter writer = new StreamWriter(File.OpenWrite(Application.dataPath + "/Map/" + name + ".txt")))
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine(grid.size.x.ToString() + "," + grid.size.y.ToString())
                .AppendLine("{");
            for (int i = 0; i < grid.size.y; i++)
            {
                stringBuilder.Append('\t');
                for (int y = 0; y < grid.size.x; y++)
                {
                    Tile tile = grid.GetTile(new Vector2Int(y, i));
                    if (tile.IsUnitOnTile())
                    {
                        Unit unit = tile.GetUnitOnTile();
                        switch (unit.GetType().Name)
                        {
                            case ("Swordman"):
                                stringBuilder.Append('S');
                                break;
                            case ("Door"):
                                stringBuilder.Append('D');
                                break;
                            case ("Spearman"):
                                stringBuilder.Append('L');
                                break;
                            case ("Wizard"):
                                stringBuilder.Append('W');
                                break;
                        }
                        stringBuilder.Append(unit.army.GetArmyIndex());
                    }
                    else
                    {
                        stringBuilder.Append('0');
                    }
                    stringBuilder.Append(" , ");
                }
                stringBuilder.Append('\n');
            }
            stringBuilder.AppendLine("}");
            writer.Write(stringBuilder.ToString());
        }
    }
}
