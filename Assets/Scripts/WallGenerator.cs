using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static HashSet<Vector2Int> FindWallPos(HashSet<Vector2Int> floorPoss,List<Vector2Int> dir)
    {
        var wallPoss = new HashSet<Vector2Int>();
        foreach(Vector2Int floorPos in floorPoss)
        {
            
            foreach(Vector2Int dirPos in dir)
            {
                var temp = floorPos + dirPos;
                if (!floorPoss.Contains(temp))
                {
                    
                    wallPoss.Add(temp);
                }
            }
        }
        return wallPoss;
    }

    public static void CraetWalls(TileMapVisualizer tileMapVisualizer,HashSet<Vector2Int> floorPath)
    {
        
        var wallPath = FindWallPos(floorPath, Direction2D.dir);
        foreach(Vector2Int wallPos in wallPath)
        {
            tileMapVisualizer.PaintSingleWall(wallPos);
        }
    }
}
