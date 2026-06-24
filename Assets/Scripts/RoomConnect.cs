using System.Collections.Generic;
using UnityEngine;

public class RoomConnect
{
    public static Dictionary<Vector2Int, List<Vector2Int>> adjacencyList = 
        new Dictionary<Vector2Int, List<Vector2Int>>();

    // 添加节点
    public void AddNode(Vector2Int nodeId)
    {
        if (!adjacencyList.ContainsKey(nodeId))
        {
            adjacencyList[nodeId] = new List<Vector2Int>();
        }
    }

    // 添加连接（双向）
    public void AddEdge(Vector2Int from, Vector2Int to)
    {
        AddNode(from);
        AddNode(to);
        adjacencyList[from].Add(to);
        adjacencyList[to].Add(from);  // 双向连接
    }
    
    // 获取相邻节点
    public List<Vector2Int> GetNeighbors(Vector2Int nodeId)
    {
        if (adjacencyList.ContainsKey(nodeId))
        {
            return adjacencyList[nodeId];
        }
        return new List<Vector2Int>();
    }

    // 检查是否相连
    public bool AreConnected(Vector2Int nodeA, Vector2Int nodeB)
    {
        if (!adjacencyList.ContainsKey(nodeA)) return false;
        return adjacencyList[nodeA].Contains(nodeB);
    }
}

public class BFSRoomConnect
{
    public Dictionary<Vector2Int, List<Vector2Int>> connect = 
        new Dictionary<Vector2Int, List<Vector2Int>>();
    public Vector2Int  BFSFarthest(Vector2Int startNode)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> dis = new Dictionary<Vector2Int, int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int farthers = startNode;
        queue.Enqueue(startNode);
        dis[startNode] = 0;
        visited.Add(startNode);
        int maxDis = 0;
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int d = dis[current];
            if (d > maxDis)
            {
                maxDis = d;
                farthers = current;
            }
            foreach(Vector2Int pos in connect[current])
            {
                if (!visited.Contains(pos))
                {
                    visited.Add(pos);
                    queue.Enqueue(pos);
                    dis[pos] =d+1;
                }
            }
        }
        return farthers;
    }
}
