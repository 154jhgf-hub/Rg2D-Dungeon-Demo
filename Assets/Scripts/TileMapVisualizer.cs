using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTileMap;
    [SerializeField]
    private Tilemap wallTileMap;
    [SerializeField,Header("µØ°åÍßÆ¬")]
    private TileBase floorTile;
    [SerializeField, Header("Ç½±ÚÍßÆ¬")]
    private TileBase wallTile;
    public void PaintSingleTile(Tilemap tilemap,TileBase tileBase,Vector2Int Paintpos)
    {
        var pos = tilemap.WorldToCell((Vector3Int)Paintpos);
        tilemap.SetTile(pos, tileBase);
    }

    public void PaintTile(IEnumerable<Vector2Int> positions, Tilemap tilemap,TileBase tile)
    {
        foreach(Vector2Int pos in positions)
        {
            PaintSingleTile(tilemap, tile, pos);
        }
    }

    public void PaintFloorTile(IEnumerable<Vector2Int> floorPoss)
    {
        PaintTile(floorPoss, floorTileMap, floorTile);
    }

    public void PaintSingleWall(Vector2Int pos)
    {
        PaintSingleTile(wallTileMap, wallTile, pos);
    }
    public void ClearTile()
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
    }
}
