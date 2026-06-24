using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField,Header("瓦片可视化器")]
    protected TileMapVisualizer visualizer;
    [SerializeField, Header("起始位置")]
    protected Vector2Int startPos = Vector2Int.zero;

    public void DungeonGenerator()
    {
        visualizer.ClearTile();
        RunProduralGeneration();
    }

    public abstract void RunProduralGeneration();
}
