using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField, Header("地牢生成参数")]
    protected RandomWalkSO randomWalkSO;
    public override void RunProduralGeneration()
    {
        var floorPositions = RunRandomWalk(randomWalkSO,startPos);
        visualizer.PaintFloorTile(floorPositions);
        WallGenerator.CraetWalls(visualizer, floorPositions);
    }

    public HashSet<Vector2Int> RunRandomWalk(RandomWalkSO randomWalkSO,Vector2Int initPos)
    {
        Vector2Int currentPos = initPos;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for(int i = 0; i <randomWalkSO.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGeneration.RandomPath(currentPos,randomWalkSO.walkLength);
            floorPositions.UnionWith(path);
            if (randomWalkSO.startRandomPos)
            {
                currentPos = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }
}
