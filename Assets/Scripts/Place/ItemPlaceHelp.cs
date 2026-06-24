using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlaceHelp 
{
    public Dictionary<PlaceType, HashSet<Vector2Int>> itemPlacePos = 
        new Dictionary<PlaceType, HashSet<Vector2Int>>();
    public HashSet<Vector2Int> floorNoCorridor;
    public ItemPlaceHelp(HashSet<Vector2Int> floor,HashSet<Vector2Int> floorNoCorridor)
    {
        this.floorNoCorridor = floorNoCorridor;
        Graph graph = new Graph(floor);
        foreach(Vector2Int pos in floorNoCorridor)
        {
            int number = graph.Get8Dir(pos).Count;
            PlaceType type = number == 8 ? PlaceType.OpenSpace : PlaceType.NearWall;
            if (itemPlacePos.ContainsKey(type)==false)
            {
                itemPlacePos[type] = new HashSet<Vector2Int>();
            }
            if (type == PlaceType.NearWall && number == 4)
            {
                continue;
            }
            itemPlacePos[type].Add(pos);
        }
    }

    
    public Vector2Int? GetItemPlacePos(PlaceType type,Vector2Int size,int iterator,bool isoffset)
    {
        int area = size.x * size.y;
        if (itemPlacePos[type].Count < area)
        {
            return null;
        }
        int i = 0;
        while (i < iterator)
        {
            i++;
            int index = Random.Range(0, itemPlacePos[type].Count);
            Vector2Int pos = itemPlacePos[type].ElementAtOrDefault(index);
            if (pos == null)
            {
                continue;
            }
            if (area > 1)
            {
                var (result, bigItemPos) = PlaceBigItem(pos, size, isoffset);
                if (result == false)
                {
                    continue;
                }
                itemPlacePos[type].ExceptWith(bigItemPos);
                itemPlacePos[PlaceType.NearWall].ExceptWith(bigItemPos);
            }
            else
            {
                itemPlacePos[type].Remove(pos);
            }
            return pos;
        }
        return null;
    }

    public void PlaceBoss(Vector2Int pos)
    {
        var (result, poslist) = PlaceBigItem(pos, new Vector2Int(1, 2), false);
        if (result)
        {
            itemPlacePos[PlaceType.OpenSpace].ExceptWith(poslist);
        }
    }

    private (bool p ,HashSet<Vector2Int>) PlaceBigItem(Vector2Int originpos,Vector2Int size,bool isoffset)
    {
        HashSet<Vector2Int> bigItemPos = new HashSet<Vector2Int>();
        bigItemPos.Add(originpos);
        int maxX = isoffset ? size.x + 1 : size.x;
        int maxY = isoffset ? size.y + 1 : size.y;
        int minX = isoffset ? -1: 0;
        int minY = isoffset ? -1: 0;
        for(int i = minX; i <= maxX; i++)
        {
            for(int j = minY; j <= maxY; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int pos = originpos + new Vector2Int(i, j);
                if (floorNoCorridor.Contains(pos) == false)
                {
                    return (false, bigItemPos);
                }
                bigItemPos.Add(pos);
            }
        }
        return (true, bigItemPos);
    }
}
