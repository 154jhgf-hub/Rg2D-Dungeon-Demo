using System.Collections.Generic;
using UnityEngine;

public static class Direction2D 
{
    public static List<Vector2Int> dir = new List<Vector2Int>
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1)
    };

    public static List<Vector2Int> dir8 = new List<Vector2Int>
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };

    public static Vector2Int RandomDir()
    {
        int index = Random.Range(0, dir8.Count);
        return dir8[index];
    }
}
