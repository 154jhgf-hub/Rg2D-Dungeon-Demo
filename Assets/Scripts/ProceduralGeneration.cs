using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGeneration 
{
    public static HashSet<Vector2Int> RandomPath(Vector2Int startPos,int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPos);
        Vector2Int prePos = startPos;
        for(int i = 0; i < walkLength; i++)
        {
            Vector2Int newPos = prePos + Direction2D.RandomDir();
            path.Add(newPos);
            prePos = newPos;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos,int corridorLength)
    {
        var corridor = new List<Vector2Int>();
        Vector2Int currentPos = startPos;
        Vector2Int dir = Direction2D.RandomDir();
        corridor.Add(currentPos);
        for(int i = 0; i < corridorLength; i++)
        {
            var newPos = currentPos + dir;
            currentPos = newPos;
            corridor.Add(newPos);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpace(BoundsInt startSpace,int minHeight,int minWidth)
    {
        Queue<BoundsInt> spliteSpace = new Queue<BoundsInt>();
        List<BoundsInt> rooms = new List<BoundsInt>();
        spliteSpace.Enqueue(startSpace);
        while (spliteSpace.Count > 0)
        {
            BoundsInt tempSpace = spliteSpace.Dequeue();
            if (tempSpace.size.x >= minWidth && tempSpace.size.y >= minHeight)
            {
                if (Random.value > 0.5f)
                {
                    if (tempSpace.size.x >= minWidth * 2)
                    {
                        SpliteVertically(minWidth, spliteSpace, tempSpace);
                    }
                    else if (tempSpace.size.y >= minHeight * 2)
                    {
                        SpliteHorizontally(minHeight, spliteSpace, tempSpace);
                    }
                    else if (tempSpace.size.x >= minWidth && tempSpace.size.y >= minHeight)
                    {
                        rooms.Add(tempSpace);
                    }
                }
                else
                {
                    if (tempSpace.size.y >= minHeight * 2)
                    {
                        SpliteHorizontally(minHeight, spliteSpace, tempSpace);
                    }
                    else if (tempSpace.size.x >= minWidth * 2)
                    {
                        SpliteVertically(minWidth, spliteSpace, tempSpace);
                    }
                    else if (tempSpace.size.x >= minWidth && tempSpace.size.y >= minHeight)
                    {
                        rooms.Add(tempSpace);
                    }
                }
            }
        }
        return rooms;
    }

    public static void SpliteVertically(int minWidth,Queue<BoundsInt> spliteSpace,BoundsInt room)
    {
        int xSplite = Random.Range(minWidth, room.size.x - minWidth);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplite, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplite, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplite, room.size.y, room.size.z));
        spliteSpace.Enqueue(room1);
        spliteSpace.Enqueue(room2);
    }

    public static void SpliteHorizontally(int minHeight, Queue<BoundsInt> spliteSpace, BoundsInt room)
    {
        int ySplite = Random.Range(minHeight, room.size.y - minHeight);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplite, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y+ySplite, room.min.z),
            new Vector3Int(room.size.x, room.size.y-ySplite, room.size.z));
        spliteSpace.Enqueue(room1);
        spliteSpace.Enqueue(room2);
    }
}
