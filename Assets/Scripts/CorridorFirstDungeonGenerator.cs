using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : RandomWalkDungeonGenerator
{
    [SerializeField, Header("走廊长度")]
    private int corridorLength=14;
    [SerializeField, Header("走廊数量")]
    private int corridorCount = 5;
    [SerializeField, Header("房间占比")]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    public override void RunProduralGeneration()
    {
        CorridorFirstGenerator();
    }
    private List<List<Vector2Int>> CreatCorridor(HashSet<Vector2Int> floorPos,HashSet<Vector2Int> roomPath)
    {
        var currentPos = startPos;
        roomPath.Add(currentPos);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        for(int i = 0; i < corridorCount; i++)
        {
           var corridorPath=ProceduralGeneration.RandomWalkCorridor(currentPos, corridorLength);
            currentPos = corridorPath[corridorPath.Count - 1];
            roomPath.Add(currentPos);
            floorPos.UnionWith(corridorPath);
            corridors.Add(corridorPath);
        }
        return corridors;
    }

    private void CorridorFirstGenerator()
    {
        HashSet<Vector2Int> floorPath = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPath = new HashSet<Vector2Int>();
        List<List<Vector2Int>> corridors= CreatCorridor(floorPath,roomPath);
        List<Vector2Int> deadPath=FindAllDeadEnds(floorPath);
        HashSet<Vector2Int> roomList = CreatRoom(roomPath);
        CreatDeadEndRoom(deadPath, roomList);
        for (int i = 0; i < corridors.Count; i++)
        {
            var newCorridor = IncreaseCorridorSize(corridors[i], 1);
            floorPath.UnionWith(newCorridor);
        }
        floorPath.UnionWith(roomList);
        ItemPlaceHelp itemPlaceHelp = new ItemPlaceHelp(floorPath,roomList);
        ItemPlaceManager itemPlaceManager = FindObjectOfType<ItemPlaceManager>();
        itemPlaceManager.SetItem(itemPlaceHelp);
        itemPlaceManager.SetPlayer(itemPlaceHelp);
        itemPlaceManager.SetEnemy(itemPlaceHelp);
        visualizer.PaintFloorTile(floorPath);
        WallGenerator.CraetWalls(visualizer, floorPath);
    }

    private HashSet<Vector2Int> CreatRoom(HashSet<Vector2Int> roomPath)
    {
        var roomPoss = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(roomPath.Count * roomPercent);
        List<Vector2Int> roomList = roomPath.OrderBy(x => UnityEngine.Random.value).Take(roomCount).ToList(); 
        foreach(Vector2Int roomPos in roomList)
        {
            var roomFloor = RunRandomWalk(randomWalkSO, roomPos);
            roomPoss.UnionWith(roomFloor);
        }
        return roomPoss;
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPath)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        
        foreach(Vector2Int pos in floorPath)
        {
            int a = 0;
            foreach (Vector2Int dir in Direction2D.dir)
            {
                if (floorPath.Contains(pos + dir))
                {
                    a++;
                }
            }
            if (a == 1)
            {
                deadEnds.Add(pos);
            }
        }
        return deadEnds;
    }

    private void CreatDeadEndRoom(List<Vector2Int> deadPath,HashSet<Vector2Int> roomPath)
    {
        foreach(Vector2Int pos in deadPath)
        {
            if (roomPath.Contains(pos) == false)
            {
                var roomPoss = RunRandomWalk(randomWalkSO, pos);
                roomPath.UnionWith(roomPoss);
            }
        }
    }

    public HashSet<Vector2Int> IncreaseCorridorSize(List<Vector2Int> corridor, int radius)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();
        foreach (var pos in corridor)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    newCorridor.Add(pos + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }
}
