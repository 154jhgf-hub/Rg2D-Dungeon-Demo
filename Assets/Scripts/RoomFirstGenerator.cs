using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFirstGenerator : RandomWalkDungeonGenerator
{
    [SerializeField, Header("房间最小宽度")]
    private int minWidth = 5;
    [SerializeField, Header("房间最小高度")]
    private int minHeight = 5;
    [SerializeField, Header("区域宽度")]
    private int Width = 20;
    [SerializeField, Header("区域高度")]
    private int Height = 20;
    [SerializeField, Header("房间间隔")]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField, Header("是否随机生成房间")]
    private bool randomRoom;
    public HashSet<Vector2Int> enemyPath=new HashSet<Vector2Int>();
    public List<Vector2Int> roomCenter = new List<Vector2Int>();
    public RoomConnect roomConnect = new RoomConnect();
    public HashSet<Vector2Int> allFloor = new HashSet<Vector2Int>();
    public override void RunProduralGeneration()
    {
        CreatRooms();
    }
    public HashSet<Vector2Int> CraetSimpleRoom(List<BoundsInt> roomlist)
    {
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();
        foreach(BoundsInt room in roomlist)
        {
            for(int i = offset; i < room.size.x - offset; i++)
            {
                for(int j = offset; j < room.size.y - offset; j++)
                {
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(i, j);
                    floorPos.Add(pos);
                }
            }
        }
        return floorPos;
    }
    
    public void CreatRooms()
    {
        BoundsInt space = new BoundsInt(new Vector3Int(startPos.x, startPos.y, 0), 
            new Vector3Int(Width, Height, 0));
        List<BoundsInt> roomList = ProceduralGeneration.BinarySpace(space, minHeight, minWidth);
        HashSet<Vector2Int> floorPoss = new HashSet<Vector2Int>();
        roomCenter = new List<Vector2Int>();
        HashSet<Vector2Int> floorNoCorrider = new HashSet<Vector2Int>();
        if (randomRoom)
        {
            floorPoss = CreatRandomRoom(roomList);
        }
        else
        {
            floorPoss= CraetSimpleRoom(roomList);
        }
        foreach (BoundsInt room in roomList)
        {
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenter.Add(center);
        }
        HashSet<Vector2Int> corridors= ConnectRoom(roomCenter);
        HashSet<Vector2Int> newcorridors = IncreaseCorridorSize(corridors.ToList(), 1);

        floorNoCorrider.UnionWith(floorPoss);
        floorPoss.UnionWith(newcorridors);
        ItemPlaceHelp itemPlaceHelp = new ItemPlaceHelp(floorPoss, floorNoCorrider);
        ItemPlaceManager itemPlaceManager =GameObject.FindObjectOfType<ItemPlaceManager>();
       
        itemPlaceManager.SetPlayer(itemPlaceHelp);
        itemPlaceManager.SetBoss(itemPlaceHelp);
        itemPlaceManager.SetEnemy(itemPlaceHelp);
        itemPlaceManager.SetItem(itemPlaceHelp);
        //

        enemyPath.UnionWith(floorPoss);

        allFloor.UnionWith(floorPoss);
        HashSet<Vector2Int> walls = WallGenerator.FindWallPos(floorPoss, Direction2D.dir);
        itemPlaceManager.SetWallItem(walls, floorPoss);
        visualizer.PaintFloorTile(floorPoss);
        WallGenerator.CraetWalls(visualizer, floorPoss);
    }

    public void LoadMap(HashSet<Vector2Int> map)
    {
        visualizer.ClearTile();
        visualizer.PaintFloorTile(map);
        WallGenerator.CraetWalls(visualizer, map);
    }

    private Vector2Int FindClosestPoint(Vector2Int currentCenter,List<Vector2Int> centers)
    {
        float Dis = float.MaxValue;
        Vector2Int closePoint = Vector2Int.zero;
        foreach(Vector2Int center in centers)
        {
            float distance = Vector2Int.Distance(currentCenter, center);
            if (distance < Dis)
            {
                Dis = distance;
                closePoint = center;
            }
        }
        return closePoint;
    }

    private HashSet<Vector2Int> ConnectRoom(List<Vector2Int> centers)
    {
        HashSet<Vector2Int> floorPoss = new HashSet<Vector2Int>();
        int index = Random.Range(0, centers.Count);
        Vector2Int currentCenter =centers[index];
        centers.Remove(currentCenter);
        while (centers.Count > 0)
        {
            Vector2Int closePoint = FindClosestPoint(currentCenter, centers);
            roomConnect.AddEdge(currentCenter, closePoint);
            centers.Remove(closePoint);
            HashSet<Vector2Int> corridos = CreatCorridor(currentCenter, closePoint);
            currentCenter = closePoint;
            floorPoss.UnionWith(corridos);
        }
        return floorPoss;
    }

    private HashSet<Vector2Int> CreatCorridor(Vector2Int currentPoint,Vector2Int targetPonit)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        Vector2Int pos = currentPoint;
        corridors.Add(pos);
        while (pos.x != targetPonit.x)
        {
            if (targetPonit.x > pos.x)
            {
                pos = pos + Vector2Int.right;
           }
            else if (targetPonit.x < pos.x)
            {
                pos = pos + Vector2Int.left;
            }
            corridors.Add(pos);
        }
        while (pos.y != targetPonit.y)
        {
            if (targetPonit.y > pos.y)
            {
                pos = pos + Vector2Int.up;
            }
            else if (targetPonit.y < pos.y)
            {
                pos = pos + Vector2Int.down;
                
            }
            corridors.Add(pos);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreatRandomRoom(List<BoundsInt> roomlist)
    {
        HashSet<Vector2Int> floorPoss = new HashSet<Vector2Int>();
        foreach(BoundsInt room in roomlist)
        {
            Vector2Int center = new Vector2Int
                (Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y));
            HashSet<Vector2Int> temp = RunRandomWalk(randomWalkSO, center);
            foreach(Vector2Int pos in temp)
            {
                if(pos.x>=(room.min.x+offset)&&pos.x<=(room.max.x-offset)&&
                    pos.y >= (room.min.y + offset) && pos.y <= (room.max.y - offset))
                {
                    floorPoss.Add(pos);
                }
            }
            RoomData.roomCenter.Add(center);
            int i = RoomData.roomCenter.Count - 1;
            if (i >= 0)
            {
                RoomData.map.Add(i, floorPoss.ToList<Vector2Int>());
            }
        }
        return floorPoss;
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
