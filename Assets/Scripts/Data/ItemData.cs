using System;
using UnityEngine;


public enum PlaceType
{
    OpenSpace,
    NearWall,
    Wall
}
[CreateAssetMenu(fileName ="物品数据",menuName ="Dungeon/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("物品预制体")]
    public GameObject prefab;
    [Header("物品名称")]
    public new string name;
    [Header("物品图标")]
    public Sprite sprite;
    [Header("物品放置类型")]
    public PlaceType placeType;
    [Header("物品大小")]
    public Vector2Int size;
    [Header("是否添加偏移量")]
    public bool offset;

}
[Serializable]
public class ItemInfo
{
    public ItemData itemData;
    public int maxQuantity;
    public int minQuantity;
}
