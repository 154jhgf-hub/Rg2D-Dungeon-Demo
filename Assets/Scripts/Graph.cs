using System.Collections.Generic;
using UnityEngine;

public class Graph 
{
    public HashSet<Vector2Int> graph;
    public static List<Vector2Int> dir4 = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0),
        new Vector2Int(0,-1)
    };
    public static List<Vector2Int> dir8 = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(1,1),
        new Vector2Int(-1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,-1)
    };
    public Graph(IEnumerable<Vector2Int> vertices)
    {
        graph =new HashSet<Vector2Int>(vertices);
    }

    public  List<Vector2Int> Get4Dir(Vector2Int startPos)
    {
        return GetNeighbours(startPos, dir4);
    }

    public List<Vector2Int> Get8Dir(Vector2Int startPos)
    {
        return GetNeighbours(startPos, dir8);
    }
    
    private List<Vector2Int> GetNeighbours(Vector2Int startpos,List<Vector2Int> dirlist)
    {
        List<Vector2Int> dir = new List<Vector2Int>();
        foreach(Vector2Int pos in dirlist)
        {
            Vector2Int temp = startpos + pos;
            if (graph.Contains(temp))
            {
                dir.Add(temp);
            }
        }
        return dir;
    }
    
}
